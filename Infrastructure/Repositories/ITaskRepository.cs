using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<List<TaskItem>> GetByUserIdAsync(Guid userId);
    Task AddAsync(TaskItem task);
    Task SaveChangesAsync();
}