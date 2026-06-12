using Application.DTOs;
using Domain.Entities;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Tasks;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
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
    }

    public async Task<List<TaskItem>> GetUserTasksAsync(Guid userId)
    {
        var tasks = await _taskRepository.GetByUserIdAsync(userId);

        return tasks
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.CreatedAt)
            .ToList();
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _taskRepository.GetByIdAsync(id);
    }
}
