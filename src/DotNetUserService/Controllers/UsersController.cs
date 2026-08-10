using DotNetUserService.DTOs;
using DotNetUserService.Models;
using DotNetUserService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetUserService.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();

        var response = users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        });

        return Ok(response);
    }

    // GET: api/users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        });
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(
        UserCreateDto request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email
        };

        var createdUser = await _userService.CreateAsync(user);

        var response = new UserResponseDto
        {
            Id = createdUser.Id,
            Username = createdUser.Username,
            Email = createdUser.Email
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    // PUT: api/users/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> Update(
        int id,
        UserUpdateDto request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email
        };

        var updatedUser = await _userService.UpdateAsync(id, user);

        if (updatedUser == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new UserResponseDto
        {
            Id = updatedUser.Id,
            Username = updatedUser.Username,
            Email = updatedUser.Email
        });
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "User not found" });
        }

        return NoContent();
    }
}