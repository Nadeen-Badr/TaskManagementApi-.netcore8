using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackgroundJobs;

public class TaskProcessingWorker : BackgroundService
{
    private readonly TaskChannel _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TaskProcessingWorker> _logger;

    public TaskProcessingWorker(
        TaskChannel channel,
        IServiceScopeFactory scopeFactory,
        ILogger<TaskProcessingWorker> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await foreach (var queuedTask in _channel.Queue.Reader.ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation(
                "Processing task {TaskId}",
                queuedTask.Id);

            await Task.Delay(
                TimeSpan.FromSeconds(3),
                stoppingToken);

            using var scope =
                _scopeFactory.CreateScope();

            var db =
                scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var task =
                await db.Tasks.FirstOrDefaultAsync(
                    t => t.Id == queuedTask.Id,
                    stoppingToken);

            if (task == null)
                continue;

            task.Status = (Domain.Enums.TaskStatus)TaskStatus.Running;

            await db.SaveChangesAsync(stoppingToken);

            _logger.LogInformation(
                "Task {TaskId} marked as InProgress",
                task.Id);
        }
    }
}