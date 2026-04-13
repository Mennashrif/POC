using Booking.Application.Abstractions;
using Booking.Application.DTOs;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using System.Transactions;

namespace Booking.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly ITransactionRepository _transactionRepository ;
    private const int HotelCapacityPerRoomType = 10;

    public ReservationService(IReservationRepository repository, ITransactionRepository transactionRepository)
    {
        _repository = repository;
        _transactionRepository  = transactionRepository;
    }

    public async Task<Result<Guid>> CreateAsync(
        GuestDetails guest,
        DateTime checkIn,
        DateTime checkOut,
        List<RoomRequest> roomRequests)
    {
        var stayDate = new StayDate(checkIn, checkOut);

        var overlapping = await _repository.GetOverlappingAsync(stayDate);

        var takenRooms = new Dictionary<RoomTypeEnum, int>();
        foreach (var res in overlapping)
            foreach (var req in res.RoomRequests)
                takenRooms[req.RoomType] = takenRooms.GetValueOrDefault(req.RoomType) + req.Quantity;

        foreach (var requested in roomRequests)
        {
            if (takenRooms.GetValueOrDefault(requested.RoomType) + requested.Quantity > HotelCapacityPerRoomType)
                return Result<Guid>.Failure("The hotel is fully booked for the requested dates and room type!");
        }

        var reservation = new Reservation(guest, stayDate, roomRequests);
        var transaction = new Domain.Models.Transaction("ReservisionConfirmed");
        reservation.Confirm();

        // ============================================================
        // SCENARIO 1: Outer = Serializable, Inner = Serializable
        // ============================================================
        // RESULT: TIMEOUT / DEADLOCK 
        // WHY: Outer INSERT takes exclusive lock on new row.
        //      Inner SELECT (Serializable) needs a RANGE lock on the entire table.
        //      Range lock conflicts with the exclusive lock on the new row.
        //      Inner waits for outer to commit → outer waits for inner to finish → DEADLOCK.
        var Outeroptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadUncommitted,
        };
        var Ineroptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadUncommitted
        };


        // ============================================================
        // SCENARIO 2: Outer = Serializable, Inner = RepeatableRead
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only, without the new reservation)
        // WHY: Outer INSERT takes exclusive lock on the new row only.
        //      Inner SELECT (Repeatable Read) takes shared locks on INDIVIDUAL ROWS only.
        //      It tries to read existing rows → no conflict.
        //      It encounters the new uncommitted row → it's uncommitted, so it SKIPS it
        //      (Repeatable Read only reads committed data).
        //      No range lock needed → no conflict with the exclusive lock.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Serializable
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.RepeatableRead
        // };


        // ============================================================
        // SCENARIO 3: Outer = Serializable, Inner = ReadCommitted
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only, without the new reservation)
        // WHY: Outer INSERT takes exclusive lock on the new row.
        //      Inner SELECT (Read Committed) takes shared lock on each row, releases immediately.
        //      It skips the uncommitted new row (only reads committed data).
        //      No range lock → no conflict.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Serializable
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadCommitted
        // };


        // ============================================================
        // SCENARIO 4: Outer = Serializable, Inner = ReadUncommitted
        // ============================================================
        // RESULT: WORKS  (Inner returns ALL data INCLUDING the uncommitted new reservation)
        // WHY: Outer INSERT takes exclusive lock on the new row.
        //      Inner SELECT (Read Uncommitted) takes NO lock at all.
        //      It reads raw memory, ignoring all locks.
        //      It SEES the uncommitted row → DIRTY READ!
        //      If outer rolls back, inner already used data that never existed.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Serializable
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadUncommitted
        // };


        // ============================================================
        // SCENARIO 5: Outer = Serializable, Inner = Snapshot
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only — snapshot taken before INSERT)
        // WHY: Outer INSERT takes exclusive lock on the new row.
        //      Inner SELECT (Snapshot) takes NO lock at all — uses versioning.
        //      It reads from a snapshot taken at the start of the inner transaction.
        //      The new row didn't exist in the snapshot → not visible.
        //      No lock conflict at all.
        // NOTE: Requires ALTER DATABASE SET ALLOW_SNAPSHOT_ISOLATION ON first!
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Serializable
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Snapshot
        // };


        // ============================================================
        // SCENARIO 6: Outer = RepeatableRead, Inner = Serializable
        // ============================================================
        // RESULT: TIMEOUT / DEADLOCK 
        // WHY: Outer INSERT takes exclusive lock on new row.
        //      Inner SELECT (Serializable) needs a RANGE lock on the table.
        //      Range lock conflicts with exclusive lock → BLOCKED → TIMEOUT.
        //      Same problem as Scenario 1 — the inner Serializable is the issue.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.RepeatableRead
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Serializable
        // };


        // ============================================================
        // SCENARIO 7: Outer = RepeatableRead, Inner = RepeatableRead
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only, without the new reservation)
        // WHY: Outer INSERT takes exclusive lock on new row only.
        //      Inner SELECT (Repeatable Read) locks individual rows, not ranges.
        //      Existing committed rows → reads fine.
        //      New uncommitted row → skipped (only reads committed data).
        //      No range lock → no conflict.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.RepeatableRead
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.RepeatableRead
        // };


        // ============================================================
        // SCENARIO 8: Outer = RepeatableRead, Inner = ReadCommitted
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only, without the new reservation)
        // WHY: Outer INSERT takes exclusive lock on new row.
        //      Inner SELECT (Read Committed) takes shared lock per row, releases immediately.
        //      Skips the uncommitted new row.
        //      No conflict.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.RepeatableRead
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadCommitted
        // };


        // ============================================================
        // SCENARIO 9: Outer = RepeatableRead, Inner = ReadUncommitted
        // ============================================================
        // RESULT: WORKS  (Inner returns ALL data INCLUDING the uncommitted reservation)
        // WHY: Outer INSERT takes exclusive lock on new row.
        //      Inner SELECT (Read Uncommitted) ignores all locks.
        //      Reads the uncommitted row → DIRTY READ!
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.RepeatableRead
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadUncommitted
        // };


        // ============================================================
        // SCENARIO 10: Outer = RepeatableRead, Inner = Snapshot
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only — snapshot before INSERT)
        // WHY: No locks needed for snapshot reads. Versioning handles it.
        // NOTE: Requires ALTER DATABASE SET ALLOW_SNAPSHOT_ISOLATION ON first!
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.RepeatableRead
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Snapshot
        // };


        // ============================================================
        // SCENARIO 11: Outer = ReadCommitted, Inner = Serializable
        // ============================================================
        // RESULT: TIMEOUT / DEADLOCK 
        // WHY: Outer INSERT takes exclusive lock on new row.
        //      Inner SELECT (Serializable) needs range lock → conflicts → BLOCKED.
        //      The outer isolation level doesn't matter here — the problem is
        //      always the inner Serializable trying to range-lock against the
        //      outer's exclusive lock.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadCommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Serializable
        // };


        // ============================================================
        // SCENARIO 12: Outer = ReadCommitted, Inner = RepeatableRead
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only)
        // WHY: No range lock needed. Individual row locks don't conflict
        //      with the exclusive lock on the new uncommitted row.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadCommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.RepeatableRead
        // };


        // ============================================================
        // SCENARIO 13: Outer = ReadCommitted, Inner = ReadCommitted
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only)
        // WHY: Inner takes shared lock per row, releases immediately.
        //      Skips uncommitted new row. No conflict.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadCommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadCommitted
        // };


        // ============================================================
        // SCENARIO 14: Outer = ReadCommitted, Inner = ReadUncommitted
        // ============================================================
        // RESULT: WORKS  (Inner sees uncommitted data → DIRTY READ)
        // WHY: No locks on read. Reads raw memory.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadCommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadUncommitted
        // };


        // ============================================================
        // SCENARIO 15: Outer = ReadCommitted, Inner = Snapshot
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only)
        // WHY: Snapshot uses versioning, no locks needed.
        // NOTE: Requires ALTER DATABASE SET ALLOW_SNAPSHOT_ISOLATION ON first!
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadCommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Snapshot
        // };


        // ============================================================
        // SCENARIO 16: Outer = ReadUncommitted, Inner = Serializable
        // ============================================================
        // RESULT: TIMEOUT / DEADLOCK 
        // WHY: Even though outer is ReadUncommitted, the INSERT still takes
        //      an exclusive lock (writes ALWAYS lock regardless of isolation level).
        //      Inner Serializable needs range lock → conflicts → BLOCKED.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadUncommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Serializable
        // };


        // ============================================================
        // SCENARIO 17: Outer = ReadUncommitted, Inner = RepeatableRead
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only)
        // WHY: Inner locks individual rows only, skips uncommitted row.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadUncommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.RepeatableRead
        // };


        // ============================================================
        // SCENARIO 18: Outer = ReadUncommitted, Inner = ReadCommitted
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only)
        // WHY: Inner skips uncommitted data, no range lock needed.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadUncommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadCommitted
        // };


        // ============================================================
        // SCENARIO 19: Outer = ReadUncommitted, Inner = ReadUncommitted
        // ============================================================
        // RESULT: WORKS  (Inner sees uncommitted data → DIRTY READ)
        // WHY: No locks at all on reads. Both ignore all locks.
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadUncommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadUncommitted
        // };


        // ============================================================
        // SCENARIO 20: Outer = ReadUncommitted, Inner = Snapshot
        // ============================================================
        // RESULT: WORKS  (Inner returns old data only)
        // WHY: Snapshot uses versioning, no lock conflict.
        // NOTE: Requires ALTER DATABASE SET ALLOW_SNAPSHOT_ISOLATION ON first!
        // var Outeroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.ReadUncommitted
        // };
        // var Ineroptions = new TransactionOptions
        // {
        //     IsolationLevel = IsolationLevel.Snapshot
        // };

        using var scope = new TransactionScope(TransactionScopeOption.Required, Outeroptions, TransactionScopeAsyncFlowOption.Enabled);

        await _repository.AddAsync(reservation);
        await _repository.SaveChangesAsync();

        // Same transaction → will see the uncommitted reservation
        var sameTransactionResult = await _repository.GetAllAsync();

        // Brand new independent transaction → will NOT see the uncommitted reservation
        //List<ReservationDto> newTransactionResult;
        using (var innerScope = new TransactionScope(TransactionScopeOption.RequiresNew, Ineroptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            var newTransactionResult = await _repository.GetAllAsync();
            innerScope.Complete();
        }
        //Make this add throw exception to test the rollback 
        await _transactionRepository.AddAsync(transaction);
        await _transactionRepository.SaveChangesAsync();

        scope.Complete();

        return Result<Guid>.Success(reservation.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid reservationId, GuestDetails guest, DateTime checkIn, DateTime checkOut)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        if (reservation is null)
            return Result<bool>.Failure($"Reservation {reservationId} not found.");

        var stayDate = new StayDate(checkIn, checkOut);
        reservation.Update(guest, stayDate);

        _repository.Update(reservation);
        await _repository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> CheckInAsync(Guid reservationId, List<string> physicalRoomIds)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        if (reservation is null)
            return Result<bool>.Failure($"Reservation {reservationId} not found.");

        reservation.CheckIn(physicalRoomIds);
        await _repository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<ReservationDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetDetailsByIdAsync(id);
    }
}
