using API.Helpers;
using Application.DTOs;
using Application.Services.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var userId = UserContext.GetUserId(User);

        await _taskService.CreateTaskAsync(userId, dto);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetMyTasks()
    {
        var userId = UserContext.GetUserId(User);

        var tasks = await _taskService.GetUserTasksAsync(userId);

        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }
}