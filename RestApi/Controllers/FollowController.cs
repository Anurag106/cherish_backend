using Cherish.RestApi.Models.Requests;
using Cherish.RestApi.Models.Responses;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FollowResponse = Cherish.RestApi.Models.Responses.FollowResponse;
using FollowListResponse = Cherish.RestApi.Models.Responses.FollowListResponse;
using FollowRequest = Cherish.RestApi.Models.Requests.FollowRequest;

namespace Cherish.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly IFollowService _followService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<FollowController> _logger;

    public FollowController(IFollowService followService, ITokenService tokenService, ILogger<FollowController> logger)
    {
        _followService = followService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("user/{userId}")]
    public async Task<ActionResult<ApiResponse<Cherish.RestApi.Models.Responses.FollowResponse>>> FollowUser(Guid userId, [FromBody] FollowUserRequest request)
    {
        try
        {
            var token = GetTokenFromRequest();
            var followerUserId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (followerUserId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<FollowResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _followService.FollowUserAsync(companyId.Value, followerUserId.Value, userId, request.IsFollowing);

            if (result == null)
            {
                return BadRequest(new ApiResponse<FollowResponse>
                {
                    Success = false,
                    Message = "Failed to follow/unfollow user. User may not exist, be in different company, or trying to follow yourself."
                });
            }

            return Ok(new ApiResponse<FollowResponse>
            {
                Success = true,
                Message = request.IsFollowing ? "User followed successfully" : "User unfollowed successfully",
                Data = new FollowResponse
                {
                    Type = result.Type,
                    TargetId = result.TargetId,
                    IsFollowing = result.IsFollowing,
                    LastModified = result.LastModified
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user {UserId}", userId);
            return StatusCode(500, new ApiResponse<FollowResponse>
            {
                Success = false,
                Message = "An error occurred while processing your request"
            });
        }
    }

    [HttpPost("team/{teamId}")]
    public async Task<ActionResult<ApiResponse<FollowResponse>>> FollowTeam(Guid teamId, [FromBody] FollowTeamRequest request)
    {
        try
        {
            var token = GetTokenFromRequest();
            var followerUserId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (followerUserId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<FollowResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _followService.FollowTeamAsync(companyId.Value, followerUserId.Value, teamId, request.IsFollowing);

            if (result == null)
            {
                return BadRequest(new ApiResponse<FollowResponse>
                {
                    Success = false,
                    Message = "Failed to follow/unfollow team. Team may not exist, be in different company, or trying to follow your own team."
                });
            }

            return Ok(new ApiResponse<FollowResponse>
            {
                Success = true,
                Message = request.IsFollowing ? "Team followed successfully" : "Team unfollowed successfully",
                Data = new FollowResponse
                {
                    Type = result.Type,
                    TargetId = result.TargetId,
                    IsFollowing = result.IsFollowing,
                    LastModified = result.LastModified
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following team {TeamId}", teamId);
            return StatusCode(500, new ApiResponse<FollowResponse>
            {
                Success = false,
                Message = "An error occurred while processing your request"
            });
        }
    }

    [HttpPost("follow")]
    public async Task<ActionResult<ApiResponse<FollowResponse>>> Follow([FromBody] FollowRequest request)
    {
        try
        {
            var token = GetTokenFromRequest();
            var followerUserId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (followerUserId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<FollowResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var domainRequest = new Domain.Models.FollowRequest
            {
                Type = request.Type,
                TargetId = request.TargetId,
                IsFollowing = request.IsFollowing
            };
            var result = await _followService.FollowAsync(companyId.Value, followerUserId.Value, domainRequest);

            if (result == null)
            {
                return BadRequest(new ApiResponse<FollowResponse>
                {
                    Success = false,
                    Message = "Failed to follow/unfollow. Check if target exists and validation rules."
                });
            }

            return Ok(new ApiResponse<FollowResponse>
            {
                Success = true,
                Message = request.IsFollowing ? $"{request.Type} followed successfully" : $"{request.Type} unfollowed successfully",
                Data = new FollowResponse
                {
                    Type = result.Type,
                    TargetId = result.TargetId,
                    IsFollowing = result.IsFollowing,
                    LastModified = result.LastModified
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following {Type} {TargetId}", request.Type, request.TargetId);
            return StatusCode(500, new ApiResponse<FollowResponse>
            {
                Success = false,
                Message = "An error occurred while processing your request"
            });
        }
    }

    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<FollowListResponse>>> GetFollowList()
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (userId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<FollowListResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _followService.GetFollowListAsync(companyId.Value, userId.Value);

            return Ok(new ApiResponse<FollowListResponse>
            {
                Success = true,
                Message = "Follow list retrieved successfully",
                Data = new FollowListResponse
                {
                    FollowingUsers = result.FollowingUsers.Select(u => new UserFollowInfoResponse
                    {
                        UserId = u.UserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Department = u.Department,
                        LastModified = u.LastModified
                    }).ToList(),
                    FollowingTeams = result.FollowingTeams.Select(t => new TeamFollowInfoResponse
                    {
                        TeamId = t.TeamId,
                        TeamName = t.TeamName,
                        ManagerId = t.ManagerId,
                        ManagerName = t.ManagerName,
                        MemberCount = t.MemberCount,
                        LastModified = t.LastModified
                    }).ToList(),
                    Followers = result.Followers.Select(u => new UserFollowInfoResponse
                    {
                        UserId = u.UserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Department = u.Department,
                        LastModified = u.LastModified
                    }).ToList()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follow list");
            return StatusCode(500, new ApiResponse<FollowListResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving follow list"
            });
        }
    }

    [HttpGet("following/users")]
    public async Task<ActionResult<ApiResponse<List<UserFollowInfoResponse>>>> GetFollowingUsers()
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (userId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<List<UserFollowInfoResponse>>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _followService.GetUserFollowingUsersAsync(companyId.Value, userId.Value);

            var response = result.Select(u => new UserFollowInfoResponse
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department,
                LastModified = u.LastModified
            }).ToList();

            return Ok(new ApiResponse<List<UserFollowInfoResponse>>
            {
                Success = true,
                Message = $"Retrieved {response.Count} following users",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following users");
            return StatusCode(500, new ApiResponse<List<UserFollowInfoResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving following users"
            });
        }
    }

    [HttpGet("following/teams")]
    public async Task<ActionResult<ApiResponse<List<TeamFollowInfoResponse>>>> GetFollowingTeams()
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (userId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<List<TeamFollowInfoResponse>>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _followService.GetUserFollowingTeamsAsync(companyId.Value, userId.Value);

            var response = result.Select(t => new TeamFollowInfoResponse
            {
                TeamId = t.TeamId,
                TeamName = t.TeamName,
                ManagerId = t.ManagerId,
                ManagerName = t.ManagerName,
                MemberCount = t.MemberCount,
                LastModified = t.LastModified
            }).ToList();

            return Ok(new ApiResponse<List<TeamFollowInfoResponse>>
            {
                Success = true,
                Message = $"Retrieved {response.Count} following teams",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following teams");
            return StatusCode(500, new ApiResponse<List<TeamFollowInfoResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving following teams"
            });
        }
    }

    [HttpGet("followers")]
    public async Task<ActionResult<ApiResponse<List<UserFollowInfoResponse>>>> GetFollowers()
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (userId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<List<UserFollowInfoResponse>>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _followService.GetUserFollowersAsync(companyId.Value, userId.Value);

            var response = result.Select(u => new UserFollowInfoResponse
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department,
                LastModified = u.LastModified
            }).ToList();

            return Ok(new ApiResponse<List<UserFollowInfoResponse>>
            {
                Success = true,
                Message = $"Retrieved {response.Count} followers",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers");
            return StatusCode(500, new ApiResponse<List<UserFollowInfoResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving followers"
            });
        }
    }

    [HttpGet("status/user/{userId}")]
    public async Task<ActionResult<ApiResponse<bool>>> IsFollowingUser(Guid userId)
    {
        try
        {
            var token = GetTokenFromRequest();
            var followerUserId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (followerUserId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _followService.IsFollowingUserAsync(companyId.Value, followerUserId.Value, userId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = result ? "Following this user" : "Not following this user",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking follow status for user {UserId}", userId);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while checking follow status"
            });
        }
    }

    [HttpGet("status/team/{teamId}")]
    public async Task<ActionResult<ApiResponse<bool>>> IsFollowingTeam(Guid teamId)
    {
        try
        {
            var token = GetTokenFromRequest();
            var followerUserId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (followerUserId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _followService.IsFollowingTeamAsync(companyId.Value, followerUserId.Value, teamId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = result ? "Following this team" : "Not following this team",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking follow status for team {TeamId}", teamId);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while checking follow status"
            });
        }
    }

    private string GetTokenFromRequest()
    {
        return Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
    }
}
