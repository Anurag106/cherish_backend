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
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<CommentController> _logger;

    public CommentController(
        ICommentService commentService,
        ITokenService tokenService,
        ILogger<CommentController> logger)
    {
        _commentService = commentService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CommentResponse>>> CreateComment([FromBody] CreateCommentRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Content))
            {
                return BadRequest(new ApiResponse<CommentResponse>
                {
                    Success = false,
                    Message = "Comment content is required"
                });
            }

            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);

            if (userId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<CommentResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var comment = await _commentService.CreateCommentAsync(
                userId.Value,
                request.PostedByAdded,
                companyId.Value,
                request.Content,
                request.PostId
            );

            if (comment == null)
            {
                return BadRequest(new ApiResponse<CommentResponse>
                {
                    Success = false,
                    Message = "Failed to create comment. Please check if you have enough points and the post exists."
                });
            }

            var response = new CommentResponse
            {
                Id = comment.Id,
                UserId = comment.UserId,
                PostedByAdded = comment.PostedByAdded,
                CompanyId = comment.CompanyId,
                Content = comment.Content,
                Points = comment.Points,
                PostId = comment.PostId,
                Hashtags = comment.Hashtags,
                CreatedAt = comment.CreatedAt,
                Metadata = comment.Metadata
            };

            return Ok(new ApiResponse<CommentResponse>
            {
                Success = true,
                Message = "Comment created successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during comment creation for post: {PostId}", request.PostId);
            return StatusCode(500, new ApiResponse<CommentResponse>
            {
                Success = false,
                Message = "An error occurred during comment creation"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CommentResponse>>> GetComment(Guid id)
    {
        try
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound(new ApiResponse<CommentResponse>
                {
                    Success = false,
                    Message = "Comment not found"
                });
            }

            var response = new CommentResponse
            {
                Id = comment.Id,
                UserId = comment.UserId,
                PostedByAdded = comment.PostedByAdded,
                CompanyId = comment.CompanyId,
                Content = comment.Content,
                Points = comment.Points,
                PostId = comment.PostId,
                Hashtags = comment.Hashtags,
                CreatedAt = comment.CreatedAt,
                Metadata = comment.Metadata
            };

            return Ok(new ApiResponse<CommentResponse>
            {
                Success = true,
                Message = "Comment retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comment: {CommentId}", id);
            return StatusCode(500, new ApiResponse<CommentResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving the comment"
            });
        }
    }

    [HttpGet("post/{postId}")]
    public async Task<ActionResult<ApiResponse<List<CommentResponse>>>> GetCommentsByPost(Guid postId)
    {
        try
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);
            var response = comments.Select(comment => new CommentResponse
            {
                Id = comment.Id,
                UserId = comment.UserId,
                PostedByAdded = comment.PostedByAdded,
                CompanyId = comment.CompanyId,
                Content = comment.Content,
                Points = comment.Points,
                PostId = comment.PostId,
                Hashtags = comment.Hashtags,
                CreatedAt = comment.CreatedAt,
                Metadata = comment.Metadata
            }).ToList();

            return Ok(new ApiResponse<List<CommentResponse>>
            {
                Success = true,
                Message = "Comments retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments for post: {PostId}", postId);
            return StatusCode(500, new ApiResponse<List<CommentResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving comments"
            });
        }
    }

    [HttpGet("post/{postId}/paged")]
    public async Task<ActionResult<ApiResponse<List<CommentResponse>>>> GetCommentsByPostPaged(
        Guid postId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var comments = await _commentService.GetCommentsByPostIdWithPaginationAsync(postId, page, pageSize);
            var response = comments.Select(comment => new CommentResponse
            {
                Id = comment.Id,
                UserId = comment.UserId,
                PostedByAdded = comment.PostedByAdded,
                CompanyId = comment.CompanyId,
                Content = comment.Content,
                Points = comment.Points,
                PostId = comment.PostId,
                Hashtags = comment.Hashtags,
                CreatedAt = comment.CreatedAt,
                Metadata = comment.Metadata
            }).ToList();

            return Ok(new ApiResponse<List<CommentResponse>>
            {
                Success = true,
                Message = "Comments retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated comments for post: {PostId}", postId);
            return StatusCode(500, new ApiResponse<List<CommentResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving comments"
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CommentResponse>>> UpdateComment(Guid id, [FromBody] UpdateCommentRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Content))
            {
                return BadRequest(new ApiResponse<CommentResponse>
                {
                    Success = false,
                    Message = "Comment content is required"
                });
            }

            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new ApiResponse<CommentResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var comment = await _commentService.UpdateCommentContentAsync(id, request.Content, userId.Value);

            if (comment == null)
            {
                return BadRequest(new ApiResponse<CommentResponse>
                {
                    Success = false,
                    Message = "Update failed: Comment not found, you don't have permission, or points were changed (only content text and hashtags can be modified)"
                });
            }

            var response = new CommentResponse
            {
                Id = comment.Id,
                UserId = comment.UserId,
                PostedByAdded = comment.PostedByAdded,
                CompanyId = comment.CompanyId,
                Content = comment.Content,
                Points = comment.Points,
                PostId = comment.PostId,
                Hashtags = comment.Hashtags,
                CreatedAt = comment.CreatedAt,
                Metadata = comment.Metadata,
                Deleted = comment.Deleted
            };

            return Ok(new ApiResponse<CommentResponse>
            {
                Success = true,
                Message = "Comment updated successfully (content and hashtags can be modified - mentions and points remain unchanged)",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment: {CommentId}", id);
            return StatusCode(500, new ApiResponse<CommentResponse>
            {
                Success = false,
                Message = "An error occurred during comment update"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteComment(Guid id)
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

            var success = await _commentService.SoftDeleteCommentAsync(id, userId.Value);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Comment not found or you don't have permission to delete it"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Comment soft deleted successfully (transactions remain unchanged)"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment: {CommentId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred during comment deletion"
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
