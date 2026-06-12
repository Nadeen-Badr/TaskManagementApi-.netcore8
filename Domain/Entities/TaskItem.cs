using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Domain.Enums;

namespace Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Enums.TaskStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}