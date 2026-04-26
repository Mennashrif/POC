using Billing.Application.Abstractions;
using Billing.Application.DTOs;
using Billing.Domain.Abstractions;
using Billing.Domain.Models;
using System.Security.Cryptography;

namespace Billing.Application.Services;

public class FileService : IFileService
{
    private readonly IFileValidator _fileValidator;
    private readonly IFileStorage _fileStorage;
    private readonly IDataExtractor _dataExtractor;
    private readonly IBillingFileRepository _fileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FileService(
        IFileValidator fileValidator,
        IFileStorage fileStorage,
        IDataExtractor dataExtractor,
        IBillingFileRepository fileRepository,
        IUnitOfWork unitOfWork)
    {
        _fileValidator = fileValidator;
        _fileStorage = fileStorage;
        _dataExtractor = dataExtractor;
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> UploadAsync(Stream fileStream, string fileName, long fileSize, string contentType)
    {
        // 1. Validate size, extension, magic bytes / binary check
        var (isValid, error) = await _fileValidator.ValidateAsync(fileStream, fileName, fileSize);
        if (!isValid)
            return Result<Guid>.Failure(error!);

        fileStream.Position = 0;

        // 2. Compute hash
        var hash = await ComputeHashAsync(fileStream);
        fileStream.Position = 0;

        // 3. Store file
        var storagePath = await _fileStorage.SaveAsync(fileStream, fileName);
        fileStream.Position = 0;

        // 4. Extract data
        var extractedData = await _dataExtractor.ExtractAsync(fileStream, fileName);

        // 5. Save metadata
        var billingFile = new BillingFile(fileName, contentType, fileSize, hash, storagePath);

        if (extractedData is not null)
            billingFile.SetExtractedData(extractedData);

        await _fileRepository.AddAsync(billingFile);
        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(billingFile.Id);
    }

    public async Task<Result<FileDownloadDto>> DownloadAsync(Guid fileId)
    {
        // 1. Find record
        var billingFile = await _fileRepository.GetByIdAsync(fileId);
        if (billingFile is null)
            return Result<FileDownloadDto>.Failure($"File {fileId} not found.");

        // 2. Verify file exists on disk
        if (!_fileStorage.Exists(billingFile.StoragePath))
            return Result<FileDownloadDto>.Failure("File not found on storage.");

        // 3. Get stream and verify hash
        var stream = await _fileStorage.GetAsync(billingFile.StoragePath);
        var actualHash = Convert.ToHexString(await SHA256.HashDataAsync(stream)).ToLowerInvariant();

        if (actualHash != billingFile.Hash)
            return Result<FileDownloadDto>.Failure("File integrity check failed.");

        stream.Position = 0;

        return Result<FileDownloadDto>.Success(new FileDownloadDto(
            stream,
            billingFile.FileName,
            billingFile.ContentType,
            billingFile.Hash
        ));
    }

    private static async Task<string> ComputeHashAsync(Stream stream)
    {
        var hashBytes = await SHA256.HashDataAsync(stream);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
