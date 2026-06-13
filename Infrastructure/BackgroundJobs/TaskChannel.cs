using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Infrastructure.BackgroundJobs;

public class TaskChannel
{
    public Channel<TaskItem> Queue { get; } =
        Channel.CreateUnbounded<TaskItem>();
}