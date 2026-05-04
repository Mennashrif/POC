using Hangfire;
using Hangfire.SqlServer;
using RoomManagement.API.Endpoints;
using RoomManagement.Application;
using RoomManagement.Infrastructure;
using RoomManagement.Infrastructure.Data;
using RoomManagement.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("RoomManagementDb"),
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

builder.Services.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomManagementDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard("/hangfire");
app.MapRoomEndpoints();
app.MapRoomTypeEndpoints();
app.UseHttpsRedirection();

RecurringJob.AddOrUpdate<OutboxProcessorJob>(
    "room-outbox-processor",
    job => job.ProcessAsync(),
    Cron.Minutely());

app.Run();
