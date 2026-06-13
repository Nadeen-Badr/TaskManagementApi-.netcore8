using Application.DTOs;
using Application.Services.Auth;
using Application.Services.Tasks;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Auth;
using Infrastructure.BackgroundJobs;
using Infrastructure.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Tests;

public class Tests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();

    private TaskService CreateService()
    {
        return new TaskService(
            _taskRepo.Object,
            cache: null!,
            logger: null!,
            taskChannel: new TaskChannel()
        );
    }
    [Fact]
    public async Task Should_Throw_When_Duplicate_Task_Today()
    {
        // Arrange
        var service = CreateService();

        var userId = Guid.NewGuid();

        _taskRepo.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<TaskItem>
            {
            new TaskItem
            {
                Title = "Test",
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            }
            });

        var dto = new CreateTaskDto
        {
            Title = "Test",
            Description = "desc",
            Priority = TaskPriority.High
        };

        // Act
        Func<Task> act = async () =>
            await service.CreateTaskAsync(userId, dto);

        // Assert
        await Assert.ThrowsAsync<Exception>(act);
    }

    [Fact]
    public async Task Should_Create_Task_When_Not_Duplicate()
    {
        // Arrange
        var service = CreateService();

        var userId = Guid.NewGuid();

        _taskRepo.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<TaskItem>());

        _taskRepo.Setup(x => x.AddAsync(It.IsAny<TaskItem>()))
            .Returns(Task.CompletedTask);

        _taskRepo.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var dto = new CreateTaskDto
        {
            Title = "New Task",
            Description = "desc",
            Priority = TaskPriority.High
        };

        // Act
        await service.CreateTaskAsync(userId, dto);

        // Assert
        _taskRepo.Verify(x => x.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        _taskRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

}