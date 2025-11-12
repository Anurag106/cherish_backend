using Cherish.RestApi.Models.Responses;
using Cherish.RestApi.Models.Requests;
using Domain.Services;
using Domain.Providers;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cherish.RestApi.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserProvider _userProvider;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserProvider userProvider,
        IUserService userService,
        ITokenService tokenService,
        ILogger<UserController> logger)
    {
        _userProvider = userProvider;
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> GetUserProfile()
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (userId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<UserProfileResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var user = await _userProvider.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return NotFound(new ApiResponse<UserProfileResponse>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            var response = new UserProfileResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PreferredFirstName = user.PreferredFirstName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                UserMode = user.UserMode,
                EmployeeStatus = user.EmployeeStatus,
                TeamId = user.TeamId,
                Department = user.Department,
                JobTitle = user.JobTitle,
                DateHired = user.DateHired,
                DateOfBirth = user.DateOfBirth,
                TotalPoints = user.TotalPoints,
                AvailablePoints = user.AvailablePoints,
                CompanyName = user.CompanyName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, new ApiResponse<UserProfileResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving user profile"
            });
        }
    }

    [HttpPost("add-points")]
    public async Task<ActionResult<ApiResponse>> AddPoints([FromBody] AddPointsRequest request)
    {
        try
        {
            if (request.Points <= 0)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Points must be greater than 0"
                });
            }

            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            
            if (userId == null)
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var user = await _userProvider.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            var newTotalPoints = user.TotalPoints + request.Points;
            var newAvailablePoints = user.AvailablePoints + request.Points;

            var success = await _userProvider.UpdateUserPointsAsync(userId.Value, newTotalPoints, newAvailablePoints);
            
            if (!success)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Failed to update user points"
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"Added {request.Points} points to user. New total: {newTotalPoints}, Available: {newAvailablePoints}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding points to user");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while adding points"
            });
        }
    }

    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<List<UserListResponse>>>> GetUsers([FromQuery] GetUsersRequest request)
    {
        try
        {
            var token = GetTokenFromRequest();
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<List<UserListResponse>>
                {
                    Success = false,
                    Message = "Invalid token or company not found"
                });
            }

            var users = await _userProvider.GetUsersAsync(
                companyId.Value,
                request.UserIds,
                request.Status,
                request.TeamId,
                request.PageNumber,
                request.PageSize
            );

            var response = users.Select(user => new UserListResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserMode = user.UserMode,
                EmployeeStatus = user.EmployeeStatus,
                TeamId = user.TeamId,
                Department = user.Department,
                JobTitle = user.JobTitle,
                TotalPoints = user.TotalPoints,
                AvailablePoints = user.AvailablePoints,
                CreatedAt = user.CreatedAt
            }).ToList();

            return Ok(new ApiResponse<List<UserListResponse>>
            {
                Success = true,
                Message = $"Retrieved {response.Count} users",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, new ApiResponse<List<UserListResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving users"
            });
        }
    }

    [HttpGet("autocomplete")]
    public async Task<ActionResult<ApiResponse<List<UserMentionResponse>>>> GetUserAutocomplete([FromQuery] string search)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return BadRequest(new ApiResponse<List<UserMentionResponse>>
                {
                    Success = false,
                    Message = "Search term is required"
                });
            }

            var token = GetTokenFromRequest();
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<List<UserMentionResponse>>
                {
                    Success = false,
                    Message = "Invalid token or company not found"
                });
            }

            var users = await _userService.GetUserAutocompleteAsync(companyId.Value, search, 3);

            var response = users.Select(user => new UserMentionResponse
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Department = user.Department
            }).ToList();

            return Ok(new ApiResponse<List<UserMentionResponse>>
            {
                Success = true,
                Message = $"Found {response.Count} users",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user autocomplete");
            return StatusCode(500, new ApiResponse<List<UserMentionResponse>>
            {
                Success = false,
                Message = "An error occurred while searching for users"
            });
        }
    }

    [HttpGet("recipients")]
    public async Task<ActionResult<ApiResponse<List<UserMentionResponse>>>> GetRecipients()
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (userId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<List<UserMentionResponse>>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var teammates = await _userService.GetTeammatesAsync(userId.Value, companyId.Value);

            var response = teammates.Select(user => new UserMentionResponse
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Department = user.Department
            }).ToList();

            return Ok(new ApiResponse<List<UserMentionResponse>>
            {
                Success = true,
                Message = $"Found {response.Count} teammates",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recipients");
            return StatusCode(500, new ApiResponse<List<UserMentionResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving teammates"
            });
        }
    }

    private string GetTokenFromRequest()
    {
        return Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
    }
}
