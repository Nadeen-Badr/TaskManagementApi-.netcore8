
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<TaskItem>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Tasks
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}