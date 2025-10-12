using Cherish.RestApi.Models.Requests;
using Cherish.RestApi.Models.Responses;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cherish.RestApi.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ReactionController : ControllerBase
{
    private readonly IReactionService _reactionService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ReactionController> _logger;

    public ReactionController(
        IReactionService reactionService,
        ITokenService tokenService,
        ILogger<ReactionController> logger)
    {
        _reactionService = reactionService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost()]
    public async Task<ActionResult<ApiResponse<ReactionResponse>>> CreateOrUpdateReaction([FromBody] CreateReactionRequest request)
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new ApiResponse<ReactionResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            // Convert string emoji type to enum
            if (!Enum.TryParse<Domain.Models.ReactionType>(request.ReactionType, true, out var emojiType))
            {
                return BadRequest(new ApiResponse<ReactionResponse>
                {
                    Success = false,
                    Message = $"Invalid emoji type. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Domain.Models.ReactionType)))}"
                });
            }

            var reaction = await _reactionService.CreateOrUpdateReactionAsync(
                userId.Value,
                request.PostId,
                emojiType
            );

            if (reaction == null)
            {
                return BadRequest(new ApiResponse<ReactionResponse>
                {
                    Success = false,
                    Message = "Failed to create/update reaction. Please check if the post exists and belongs to your company."
                });
            }

            var response = new ReactionResponse
            {
                Id = reaction.Id,
                CompanyId = reaction.CompanyId,
                UserId = reaction.UserId,
                PostId = reaction.PostId,
                EmojiType = reaction.EmojiType.ToString(),
                LastModifiedAt = reaction.LastModifiedAt
            };

            return Ok(new ApiResponse<ReactionResponse>
            {
                Success = true,
                Message = "Reaction created/updated successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during reaction creation/update for post: {PostId}", request.PostId);
            return StatusCode(500, new ApiResponse<ReactionResponse>
            {
                Success = false,
                Message = "An error occurred during reaction creation/update"
            });
        }
    }

    [HttpDelete("{postId}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveReaction(Guid postId)
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var success = await _reactionService.RemoveReactionAsync(userId.Value, postId);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Reaction not found or you don't have permission to remove it"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Reaction removed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing reaction for post: {PostId}", postId);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred during reaction removal"
            });
        }
    }

    [HttpGet("post/{postId}")]
    public async Task<ActionResult<ApiResponse<ReactionCountsResponse>>> GetReactionCounts(Guid postId)
    {
        try
        {
            var counts = await _reactionService.GetReactionCountsByPostIdAsync(postId);
            var response = new ReactionCountsResponse
            {
                PostId = postId,
                Counts = counts
            };

            return Ok(new ApiResponse<ReactionCountsResponse>
            {
                Success = true,
                Message = "Reaction counts retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reaction counts for post: {PostId}", postId);
            return StatusCode(500, new ApiResponse<ReactionCountsResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving reaction counts"
            });
        }
    }

    [HttpGet("user/{postId}")]
    public async Task<ActionResult<ApiResponse<ReactionResponse>>> GetUserReaction(Guid postId)
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new ApiResponse<ReactionResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var reaction = await _reactionService.GetUserReactionForPostAsync(userId.Value, postId);

            if (reaction == null)
            {
                return NotFound(new ApiResponse<ReactionResponse>
                {
                    Success = false,
                    Message = "No reaction found for this post"
                });
            }

            var response = new ReactionResponse
            {
                Id = reaction.Id,
                CompanyId = reaction.CompanyId,
                UserId = reaction.UserId,
                PostId = reaction.PostId,
                EmojiType = reaction.EmojiType.ToString(),
                LastModifiedAt = reaction.LastModifiedAt
            };

            return Ok(new ApiResponse<ReactionResponse>
            {
                Success = true,
                Message = "User reaction retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user reaction for post: {PostId}", postId);
            return StatusCode(500, new ApiResponse<ReactionResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving user reaction"
            });
        }
    }

    private string GetTokenFromRequest()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return string.Empty;
    }
}
