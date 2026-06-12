using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Tasks;

public interface ITaskService
{
    Task CreateTaskAsync(Guid userId, CreateTaskDto dto);
    Task<List<TaskItem>> GetUserTasksAsync(Guid userId);
    Task<TaskItem?> GetByIdAsync(Guid id);
}