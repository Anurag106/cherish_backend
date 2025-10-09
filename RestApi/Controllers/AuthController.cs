using Cherish.RestApi.Models.Requests;
using Cherish.RestApi.Models.Responses;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cherish.RestApi.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Username and password are required"
                });
            }

            var token = await _authService.LoginAsync(request.Username, request.Password);
            
            if (token == null)
            {
                return Unauthorized(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid username or password"
                });
            }

            var response = new LoginResponse
            {
                Token = token,
                Username = request.Username,
                ExpiresAt = DateTime.UtcNow.AddHours(1) // JWT expiration time
            };

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
            return StatusCode(500, new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "An error occurred during login"
            });
        }
    }

    [HttpPost("update-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Current password and new password are required"
                });
            }

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var success = await _authService.UpdatePasswordAsync(username, request.CurrentPassword, request.NewPassword);
            
            if (!success)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid current password"
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Password updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password update for user: {Username}", User.Identity?.Name);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred during password update"
            });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Logout()
    {
        try
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Authorization token not found"
                });
            }

            var success = await _authService.LogoutAsync(token);
            
            return Ok(new ApiResponse
            {
                Success = success,
                Message = success ? "Logout successful" : "Logout failed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user: {Username}", User.Identity?.Name);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred during logout"
            });
        }
    }

    [HttpPost("validate-token")]
    public async Task<ActionResult<ApiResponse>> ValidateToken([FromBody] ValidateTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Token is required"
                });
            }

            var isValid = await _authService.ValidateTokenAsync(request.Token);

            return Ok(new ApiResponse
            {
                Success = isValid,
                Message = isValid ? "Token is valid" : "Token is invalid"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token validation");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred during token validation"
            });
        }
    }

    [HttpPost("create-user")]
    public async Task<ActionResult<ApiResponse<CreateUserResponse>>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password) || request.CompanyId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<CreateUserResponse>
                {
                    Success = false,
                    Message = "Username, password, and company ID are required"
                });
            }

            var user = await _authService.CreateUserAsync(request.Username, request.Password, request.CompanyId);
            
            if (user == null)
            {
                return Conflict(new ApiResponse<CreateUserResponse>
                {
                    Success = false,
                    Message = "User already exists"
                });
            }

            var response = new CreateUserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Status = user.Status,
                TeamId = user.TeamId,
                Department = user.Department,
                JobTitle = user.JobTitle,
                DateHired = user.DateHired,
                DateOfBirth = user.DateOfBirth,
                TotalPoints = user.TotalPoints,
                AvailablePoints = user.AvailablePoints,
                CreatedAt = user.CreatedAt
            };

            return Ok(new ApiResponse<CreateUserResponse>
            {
                Success = true,
                Message = "User created successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user creation for username: {Username}", request.Username);
            return StatusCode(500, new ApiResponse<CreateUserResponse>
            {
                Success = false,
                Message = "An error occurred during user creation"
            });
        }
    }
}
