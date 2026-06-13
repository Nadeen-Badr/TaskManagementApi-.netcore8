using Application.DTOs;
using Domain.Entities;
using Infrastructure.BackgroundJobs;
using Infrastructure.Caching;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Tasks;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly RedisCacheService _cache;
    private readonly ILogger<TaskService> _logger;
    private readonly TaskChannel _taskChannel;
    public TaskService(
        ITaskRepository taskRepository,
        RedisCacheService cache,
        ILogger<TaskService> logger,
        TaskChannel taskChannel)
    {
        _taskRepository = taskRepository;
        _cache = cache;
        _logger = logger;
        _taskChannel = taskChannel;
    }

    public async Task CreateTaskAsync(Guid userId, CreateTaskDto dto)
    {
        var today = DateTime.UtcNow.Date;

        var tasks = await _taskRepository.GetByUserIdAsync(userId);

        var exists = tasks.Any(t =>
            t.Title == dto.Title &&
            t.CreatedAt.Date == today);

        if (exists)
            throw new Exception("Duplicate task not allowed");

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = Domain.Enums.TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();
        await _taskChannel.Queue.Writer.WriteAsync(task);
    }

    public async Task<List<TaskItem>> GetUserTasksAsync(Guid userId)
    {
        var tasks = await _taskRepository.GetByUserIdAsync(userId);

        return tasks
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.CreatedAt)
            .ToList();
    }

    public async Task<TaskItem?> GetByIdAsync(
        Guid taskId,
        Guid userId)
    {
        var cacheKey = $"task:{taskId}";

        var cachedTask =
            await _cache.GetAsync<TaskItem>(cacheKey);

        if (cachedTask != null)
        {
            _logger.LogInformation(
                "Redis HIT for task {TaskId}",
                taskId);

            if (cachedTask.UserId != userId)
                return null;

            return cachedTask;
        }

        _logger.LogInformation(
            "Redis MISS for task {TaskId}",
            taskId);

        var task =
            await _taskRepository.GetByIdAsync(
                taskId);

        if (task == null)
            return null;

        _logger.LogInformation(
            "SQL query executed for task {TaskId}",
            taskId);

        await _cache.SetAsync(
            cacheKey,
            task,
            TimeSpan.FromMinutes(5));

        return task;
    }
    public async Task UpdateStatusAsync(
     Guid taskId,
     Guid userId,
     UpdateTaskStatusDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        if (task.UserId != userId)
            throw new Exception("You cannot update another user's task");

        task.Status = (Domain.Enums.TaskStatus)dto.Status;

        await _taskRepository.SaveChangesAsync();
        await _cache.RemoveAsync($"task:{taskId}");
    }
}
