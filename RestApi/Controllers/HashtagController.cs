using Cherish.RestApi.Models.Requests;
using Cherish.RestApi.Models.Responses;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cherish.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HashtagController : ControllerBase
{
    private readonly IHashtagService _hashtagService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<HashtagController> _logger;

    public HashtagController(
        IHashtagService hashtagService, 
        ITokenService tokenService,
        ILogger<HashtagController> logger)
    {
        _hashtagService = hashtagService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<HashtagResponse>>> CreateHashtag([FromBody] CreateHashtagRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new ApiResponse<HashtagResponse>
                {
                    Success = false,
                    Message = "Hashtag name is required"
                });
            }

            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (userId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<HashtagResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var hashtag = await _hashtagService.CreateHashtagAsync(request.Name, request.Description, companyId.Value, userId.Value);
            
            if (hashtag == null)
            {
                return Conflict(new ApiResponse<HashtagResponse>
                {
                    Success = false,
                    Message = "Hashtag with this name already exists in your company"
                });
            }

            var response = new HashtagResponse
            {
                Id = hashtag.Id,
                Name = hashtag.Name,
                Description = hashtag.Description,
                CompanyId = hashtag.CompanyId,
                CreatedBy = hashtag.CreatedBy,
                CreatedDate = hashtag.CreatedDate,
                ModifiedBy = hashtag.ModifiedBy,
                ModifiedDate = hashtag.ModifiedDate
            };

            return Ok(new ApiResponse<HashtagResponse>
            {
                Success = true,
                Message = "Hashtag created successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during hashtag creation for name: {Name}", request.Name);
            return StatusCode(500, new ApiResponse<HashtagResponse>
            {
                Success = false,
                Message = "An error occurred during hashtag creation"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<HashtagResponse>>> GetHashtag(int id)
    {
        try
        {
            var hashtag = await _hashtagService.GetHashtagByIdAsync(id);
            
            if (hashtag == null)
            {
                return NotFound(new ApiResponse<HashtagResponse>
                {
                    Success = false,
                    Message = "Hashtag not found"
                });
            }

            var response = new HashtagResponse
            {
                Id = hashtag.Id,
                Name = hashtag.Name,
                Description = hashtag.Description,
                CompanyId = hashtag.CompanyId,
                CreatedBy = hashtag.CreatedBy,
                CreatedDate = hashtag.CreatedDate,
                ModifiedBy = hashtag.ModifiedBy,
                ModifiedDate = hashtag.ModifiedDate
            };

            return Ok(new ApiResponse<HashtagResponse>
            {
                Success = true,
                Message = "Hashtag retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hashtag with ID: {Id}", id);
            return StatusCode(500, new ApiResponse<HashtagResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving the hashtag"
            });
        }
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<ApiResponse<HashtagResponse>>> GetHashtagByName(string name)
    {
        try
        {
            var token = GetTokenFromRequest();
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<HashtagResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var hashtag = await _hashtagService.GetHashtagByNameAsync(name, companyId.Value);
            
            if (hashtag == null)
            {
                return NotFound(new ApiResponse<HashtagResponse>
                {
                    Success = false,
                    Message = "Hashtag not found in your company"
                });
            }

            var response = new HashtagResponse
            {
                Id = hashtag.Id,
                Name = hashtag.Name,
                Description = hashtag.Description,
                CompanyId = hashtag.CompanyId,
                CreatedBy = hashtag.CreatedBy,
                CreatedDate = hashtag.CreatedDate,
                ModifiedBy = hashtag.ModifiedBy,
                ModifiedDate = hashtag.ModifiedDate
            };

            return Ok(new ApiResponse<HashtagResponse>
            {
                Success = true,
                Message = "Hashtag retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hashtag with name: {Name}", name);
            return StatusCode(500, new ApiResponse<HashtagResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving the hashtag"
            });
        }
    }

    [HttpGet("company")]
    public async Task<ActionResult<ApiResponse<List<HashtagResponse>>>> GetHashtagsByCompany()
    {
        try
        {
            var token = GetTokenFromRequest();
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<List<HashtagResponse>>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var hashtags = await _hashtagService.GetHashtagsByCompanyIdAsync(companyId.Value);

            var response = hashtags.Select(h => new HashtagResponse
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                CompanyId = h.CompanyId,
                CreatedBy = h.CreatedBy,
                CreatedDate = h.CreatedDate,
                ModifiedBy = h.ModifiedBy,
                ModifiedDate = h.ModifiedDate
            }).ToList();

            return Ok(new ApiResponse<List<HashtagResponse>>
            {
                Success = true,
                Message = "Hashtags retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hashtags for company");
            return StatusCode(500, new ApiResponse<List<HashtagResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving hashtags"
            });
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<List<HashtagResponse>>>> GetAllHashtags()
    {
        try
        {
            var hashtags = await _hashtagService.GetAllHashtagsAsync();

            var response = hashtags.Select(h => new HashtagResponse
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                CompanyId = h.CompanyId,
                CreatedBy = h.CreatedBy,
                CreatedDate = h.CreatedDate,
                ModifiedBy = h.ModifiedBy,
                ModifiedDate = h.ModifiedDate
            }).ToList();

            return Ok(new ApiResponse<List<HashtagResponse>>
            {
                Success = true,
                Message = "Hashtags retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all hashtags");
            return StatusCode(500, new ApiResponse<List<HashtagResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving hashtags"
            });
        }
    }

    [HttpGet("created-by/{createdBy}")]
    public async Task<ActionResult<ApiResponse<List<HashtagResponse>>>> GetHashtagsByCreatedBy(Guid createdBy)
    {
        try
        {
            var hashtags = await _hashtagService.GetHashtagsByCreatedByAsync(createdBy);

            var response = hashtags.Select(h => new HashtagResponse
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                CompanyId = h.CompanyId,
                CreatedBy = h.CreatedBy,
                CreatedDate = h.CreatedDate,
                ModifiedBy = h.ModifiedBy,
                ModifiedDate = h.ModifiedDate
            }).ToList();

            return Ok(new ApiResponse<List<HashtagResponse>>
            {
                Success = true,
                Message = "Hashtags retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hashtags for user: {CreatedBy}", createdBy);
            return StatusCode(500, new ApiResponse<List<HashtagResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving hashtags"
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<HashtagResponse>>> UpdateHashtag(int id, [FromBody] UpdateHashtagRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new ApiResponse<HashtagResponse>
                {
                    Success = false,
                    Message = "Hashtag name is required"
                });
            }

            var token = GetTokenFromRequest();
            var userId = _tokenService.GetUserIdFromToken(token);
            
            if (userId == null)
            {
                return Unauthorized(new ApiResponse<HashtagResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var hashtag = await _hashtagService.UpdateHashtagAsync(id, request.Name, request.Description, userId.Value);
            
            if (hashtag == null)
            {
                return NotFound(new ApiResponse<HashtagResponse>
                {
                    Success = false,
                    Message = "Hashtag not found or another hashtag with this name already exists"
                });
            }

            var response = new HashtagResponse
            {
                Id = hashtag.Id,
                Name = hashtag.Name,
                Description = hashtag.Description,
                CompanyId = hashtag.CompanyId,
                CreatedBy = hashtag.CreatedBy,
                CreatedDate = hashtag.CreatedDate,
                ModifiedBy = hashtag.ModifiedBy,
                ModifiedDate = hashtag.ModifiedDate
            };

            return Ok(new ApiResponse<HashtagResponse>
            {
                Success = true,
                Message = "Hashtag updated successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hashtag with ID: {Id}", id);
            return StatusCode(500, new ApiResponse<HashtagResponse>
            {
                Success = false,
                Message = "An error occurred while updating the hashtag"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteHashtag(int id)
    {
        try
        {
            var success = await _hashtagService.DeleteHashtagAsync(id);
            
            if (!success)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Hashtag not found"
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Hashtag deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hashtag with ID: {Id}", id);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while deleting the hashtag"
            });
        }
    }

    private string GetTokenFromRequest()
    {
        return Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
    }
}
