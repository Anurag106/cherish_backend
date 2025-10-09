using Cherish.RestApi.Models.Requests;
using Cherish.RestApi.Models.Responses;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cherish.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<PostController> _logger;

    public PostController(
        IPostService postService, 
        ITokenService tokenService,
        ILogger<PostController> logger)
    {
        _postService = postService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<PostResponse>>> CreatePost([FromBody] CreatePostRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Context))
            {
                return BadRequest(new ApiResponse<PostResponse>
                {
                    Success = false,
                    Message = "Context is required"
                });
            }

            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (userId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<PostResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _postService.ValidateAndCreatePostAsync(userId.Value, companyId.Value, request.Context, request.Visibility);
            
            if (!result.IsValid)
            {
                var errorMessages = new List<string>();
                if (result.MissingItems.Any())
                {
                    errorMessages.Add($"Missing: {string.Join(", ", result.MissingItems)}");
                }
                if (result.InvalidItems.Any())
                {
                    errorMessages.Add($"Invalid: {string.Join(", ", result.InvalidItems)}");
                }

                return BadRequest(new ApiResponse<PostResponse>
                {
                    Success = false,
                    Message = string.Join("; ", errorMessages)
                });
            }

            var response = new PostResponse
            {
                Id = result.Post!.Id,
                UserId = result.Post.UserId,
                CompanyId = result.Post.CompanyId,
                Context = result.Post.Context,
                UserMentioned = result.Post.UserMentioned,
                CreatedAt = result.Post.CreatedAt,
                Hashtags = result.Post.Hashtags,
                Metadata = result.Post.Metadata,
                TotalPoints = result.Post.TotalPoints,
                Visibility = result.Post.Visibility
            };

            return Ok(new ApiResponse<PostResponse>
            {
                Success = true,
                Message = "Post created successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during post creation");
            return StatusCode(500, new ApiResponse<PostResponse>
            {
                Success = false,
                Message = "An error occurred during post creation"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PostResponse>>> GetPost(Guid id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            
            if (post == null)
            {
                return NotFound(new ApiResponse<PostResponse>
                {
                    Success = false,
                    Message = "Post not found"
                });
            }

            var response = new PostResponse
            {
                Id = post.Id,
                UserId = post.UserId,
                CompanyId = post.CompanyId,
                Context = post.Context,
                UserMentioned = post.UserMentioned,
                CreatedAt = post.CreatedAt,
                Hashtags = post.Hashtags,
                Metadata = post.Metadata,
                TotalPoints = post.TotalPoints,
                Visibility = post.Visibility
            };

            return Ok(new ApiResponse<PostResponse>
            {
                Success = true,
                Message = "Post retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving post with ID: {Id}", id);
            return StatusCode(500, new ApiResponse<PostResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving the post"
            });
        }
    }

    [HttpGet("company")]
    public async Task<ActionResult<ApiResponse<List<PostResponse>>>> GetPostsByCompany()
    {
        try
        {
            var token = GetTokenFromRequest();
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<List<PostResponse>>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var posts = await _postService.GetPostsByCompanyIdAsync(companyId.Value);

            var response = posts.Select(p => new PostResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                CompanyId = p.CompanyId,
                Context = p.Context,
                UserMentioned = p.UserMentioned,
                CreatedAt = p.CreatedAt,
                Hashtags = p.Hashtags,
                Metadata = p.Metadata,
                TotalPoints = p.TotalPoints,
                Visibility = p.Visibility
            }).ToList();

            return Ok(new ApiResponse<List<PostResponse>>
            {
                Success = true,
                Message = "Posts retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts for company");
            return StatusCode(500, new ApiResponse<List<PostResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving posts"
            });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<PostResponse>>>> GetPostsByUser(Guid userId)
    {
        try
        {
            var posts = await _postService.GetPostsByUserIdAsync(userId);

            var response = posts.Select(p => new PostResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                CompanyId = p.CompanyId,
                Context = p.Context,
                UserMentioned = p.UserMentioned,
                CreatedAt = p.CreatedAt,
                Hashtags = p.Hashtags,
                Metadata = p.Metadata,
                TotalPoints = p.TotalPoints,
                Visibility = p.Visibility
            }).ToList();

            return Ok(new ApiResponse<List<PostResponse>>
            {
                Success = true,
                Message = "Posts retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts for user: {UserId}", userId);
            return StatusCode(500, new ApiResponse<List<PostResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving posts"
            });
        }
    }

    [HttpGet("mentioned/{userId}")]
    public async Task<ActionResult<ApiResponse<List<PostResponse>>>> GetPostsMentioningUser(Guid userId)
    {
        try
        {
            var posts = await _postService.GetPostsByMentionedUserAsync(userId);

            var response = posts.Select(p => new PostResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                CompanyId = p.CompanyId,
                Context = p.Context,
                UserMentioned = p.UserMentioned,
                CreatedAt = p.CreatedAt,
                Hashtags = p.Hashtags,
                Metadata = p.Metadata,
                TotalPoints = p.TotalPoints,
                Visibility = p.Visibility
            }).ToList();

            return Ok(new ApiResponse<List<PostResponse>>
            {
                Success = true,
                Message = "Posts retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts mentioning user: {UserId}", userId);
            return StatusCode(500, new ApiResponse<List<PostResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving posts"
            });
        }
    }

    [HttpGet("hashtag/{hashtagId}")]
    public async Task<ActionResult<ApiResponse<List<PostResponse>>>> GetPostsByHashtag(int hashtagId)
    {
        try
        {
            var posts = await _postService.GetPostsByHashtagIdAsync(hashtagId);

            var response = posts.Select(p => new PostResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                CompanyId = p.CompanyId,
                Context = p.Context,
                UserMentioned = p.UserMentioned,
                CreatedAt = p.CreatedAt,
                Hashtags = p.Hashtags,
                Metadata = p.Metadata,
                TotalPoints = p.TotalPoints,
                Visibility = p.Visibility
            }).ToList();

            return Ok(new ApiResponse<List<PostResponse>>
            {
                Success = true,
                Message = "Posts retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts for hashtag: {HashtagId}", hashtagId);
            return StatusCode(500, new ApiResponse<List<PostResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving posts"
            });
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<List<PostResponse>>>> GetAllPosts()
    {
        try
        {
            var posts = await _postService.GetAllPostsAsync();

            var response = posts.Select(p => new PostResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                CompanyId = p.CompanyId,
                Context = p.Context,
                UserMentioned = p.UserMentioned,
                CreatedAt = p.CreatedAt,
                Hashtags = p.Hashtags,
                Metadata = p.Metadata,
                TotalPoints = p.TotalPoints,
                Visibility = p.Visibility
            }).ToList();

            return Ok(new ApiResponse<List<PostResponse>>
            {
                Success = true,
                Message = "Posts retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all posts");
            return StatusCode(500, new ApiResponse<List<PostResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving posts"
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<PostResponse>>> UpdatePost(Guid id, [FromBody] UpdatePostRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Context))
            {
                return BadRequest(new ApiResponse<PostResponse>
                {
                    Success = false,
                    Message = "Post content is required"
                });
            }

            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new ApiResponse<PostResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var post = await _postService.UpdatePostContentAsync(id, request.Context, userId.Value);

            if (post == null)
            {
                return BadRequest(new ApiResponse<PostResponse>
                {
                    Success = false,
                    Message = "Update failed: Post not found, you don't have permission, or mentions/points were changed (only content text and hashtags can be modified)"
                });
            }

            var response = new PostResponse
            {
                Id = post.Id,
                UserId = post.UserId,
                CompanyId = post.CompanyId,
                Context = post.Context,
                UserMentioned = post.UserMentioned,
                CreatedAt = post.CreatedAt,
                Hashtags = post.Hashtags,
                Metadata = post.Metadata,
                TotalPoints = post.TotalPoints,
                Visibility = post.Visibility,
                Deleted = post.Deleted
            };

            return Ok(new ApiResponse<PostResponse>
            {
                Success = true,
                Message = "Post updated successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post: {PostId}", id);
            return StatusCode(500, new ApiResponse<PostResponse>
            {
                Success = false,
                Message = "An error occurred during post update"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePost(Guid id)
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

            var success = await _postService.SoftDeletePostAsync(id, userId.Value);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Post not found or you don't have permission to delete it"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Post deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post: {PostId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred during post deletion"
            });
        }
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<ApiResponse<PostWithDetailsResponse>>> GetPostWithDetails(Guid id)
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new ApiResponse<PostWithDetailsResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var postWithDetails = await _postService.GetPostWithDetailsAsync(id, userId.Value);

            if (postWithDetails == null)
            {
                return NotFound(new ApiResponse<PostWithDetailsResponse>
                {
                    Success = false,
                    Message = "Post not found"
                });
            }

            var response = new PostWithDetailsResponse
            {
                Id = postWithDetails.Post.Id,
                UserId = postWithDetails.Post.UserId,
                CompanyId = postWithDetails.Post.CompanyId,
                Context = postWithDetails.Post.Context,
                UserMentioned = postWithDetails.Post.UserMentioned,
                CreatedAt = postWithDetails.Post.CreatedAt,
                Hashtags = postWithDetails.Post.Hashtags,
                Metadata = postWithDetails.Post.Metadata,
                TotalPoints = postWithDetails.Post.TotalPoints,
                Visibility = postWithDetails.Post.Visibility,
                Deleted = postWithDetails.Post.Deleted,
                LatestComments = postWithDetails.LatestComments.Select(c => new CommentResponse
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    PostedByAdded = c.PostedByAdded,
                    CompanyId = c.CompanyId,
                    Content = c.Content,
                    Points = c.Points,
                    PostId = c.PostId,
                    Hashtags = c.Hashtags,
                    CreatedAt = c.CreatedAt,
                    Metadata = c.Metadata,
                    Deleted = c.Deleted
                }).ToList(),
                ReactionCounts = new ReactionCountsResponse
                {
                    PostId = postWithDetails.Post.Id,
                    Counts = postWithDetails.ReactionCounts
                },
                UserReaction = postWithDetails.UserReaction != null ? new ReactionResponse
                {
                    Id = postWithDetails.UserReaction.Id,
                    CompanyId = postWithDetails.UserReaction.CompanyId,
                    UserId = postWithDetails.UserReaction.UserId,
                    PostId = postWithDetails.UserReaction.PostId,
                    EmojiType = postWithDetails.UserReaction.EmojiType,
                    LastModifiedAt = postWithDetails.UserReaction.LastModifiedAt
                } : null
            };

            return Ok(new ApiResponse<PostWithDetailsResponse>
            {
                Success = true,
                Message = "Post with details retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post with details: {PostId}", id);
            return StatusCode(500, new ApiResponse<PostWithDetailsResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving post details"
            });
        }
    }

    [HttpPost("filter")]
    public async Task<ActionResult<ApiResponse<PaginatedPostResponse>>> GetFilteredPosts([FromBody] Domain.Models.PostFilterRequest request)
    {
        try
        {
            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new ApiResponse<PaginatedPostResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _postService.GetFilteredPostsWithDetailsAsync(userId.Value, request);

            var response = new PaginatedPostResponse
            {
                Posts = result.Posts.Select(p => new PostWithDetailsResponse
                {
                    Id = p.Post.Id,
                    UserId = p.Post.UserId,
                    CompanyId = p.Post.CompanyId,
                    Context = p.Post.Context,
                    UserMentioned = p.Post.UserMentioned,
                    CreatedAt = p.Post.CreatedAt,
                    Hashtags = p.Post.Hashtags,
                    Metadata = p.Post.Metadata,
                    TotalPoints = p.Post.TotalPoints,
                    Visibility = p.Post.Visibility,
                    Deleted = p.Post.Deleted,
                    LatestComments = p.LatestComments.Select(c => new CommentResponse
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        PostedByAdded = c.PostedByAdded,
                        CompanyId = c.CompanyId,
                        Content = c.Content,
                        Points = c.Points,
                        PostId = c.PostId,
                        Hashtags = c.Hashtags,
                        CreatedAt = c.CreatedAt,
                        Metadata = c.Metadata,
                        Deleted = c.Deleted
                    }).ToList(),
                    ReactionCounts = new ReactionCountsResponse
                    {
                        PostId = p.Post.Id,
                        Counts = p.ReactionCounts
                    },
                    UserReaction = p.UserReaction != null ? new ReactionResponse
                    {
                        Id = p.UserReaction.Id,
                        CompanyId = p.UserReaction.CompanyId,
                        UserId = p.UserReaction.UserId,
                        PostId = p.UserReaction.PostId,
                        EmojiType = p.UserReaction.EmojiType,
                        LastModifiedAt = p.UserReaction.LastModifiedAt
                    } : null
                }).ToList(),
                Pagination = new PaginationInfo
                {
                    PageSize = result.PageSize,
                    NextCursor = result.NextCursor,
                    PreviousCursor = result.PreviousCursor,
                    HasNextPage = result.HasNextPage,
                    HasPreviousPage = result.HasPreviousPage
                },
                AppliedFilters = new FilterInfo
                {
                    FilterByTeam = request.FilterByTeam ?? false,
                    FilterByUserId = request.FilterByUserId,
                    HashtagIds = request.HashtagIds ?? new List<int>(),
                    HashtagNames = request.HashtagNames ?? new List<string>(),
                    SortOrder = request.SortOrder
                }
            };

            return Ok(new ApiResponse<PaginatedPostResponse>
            {
                Success = true,
                Message = $"Retrieved {result.Posts.Count} posts with details",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered posts");
            return StatusCode(500, new ApiResponse<PaginatedPostResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving filtered posts"
            });
        }
    }

    private string GetTokenFromRequest()
    {
        return Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
    }
}
