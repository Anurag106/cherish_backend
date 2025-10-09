using Cherish.RestApi.Models.Requests;
using Cherish.RestApi.Models.Responses;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cherish.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(
        ITransactionService transactionService, 
        ITokenService tokenService,
        ILogger<TransactionController> logger)
    {
        _transactionService = transactionService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<TransactionResponse>>> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        try
        {
            if (request.ToUserId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<TransactionResponse>
                {
                    Success = false,
                    Message = "To user ID is required"
                });
            }

            if (request.Points <= 0)
            {
                return BadRequest(new ApiResponse<TransactionResponse>
                {
                    Success = false,
                    Message = "Points must be greater than 0"
                });
            }

            var token = GetTokenFromRequest();
            var fromUserId = _tokenService.GetUserIdFromToken(token);
            var companyId = _tokenService.GetCompanyIdFromToken(token);
            
            if (fromUserId == null || companyId == null)
            {
                return Unauthorized(new ApiResponse<TransactionResponse>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            if (fromUserId == request.ToUserId)
            {
                return BadRequest(new ApiResponse<TransactionResponse>
                {
                    Success = false,
                    Message = "Cannot send points to yourself"
                });
            }

            var transaction = await _transactionService.CreateTransactionAsync(
                companyId.Value, 
                fromUserId.Value, 
                request.ToUserId, 
                request.Points, 
                request.Description);
            
            if (transaction == null)
            {
                return BadRequest(new ApiResponse<TransactionResponse>
                {
                    Success = false,
                    Message = "Transaction failed. Check if you have enough points or if the recipient exists in your company"
                });
            }

            var response = new TransactionResponse
            {
                Id = transaction.Id,
                CompanyId = transaction.CompanyId,
                FromUserId = transaction.FromUserId,
                ToUserId = transaction.ToUserId,
                Points = transaction.Points,
                DateAndTime = transaction.DateAndTime,
                Description = transaction.Description
            };

            return Ok(new ApiResponse<TransactionResponse>
            {
                Success = true,
                Message = "Transaction created successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during transaction creation");
            return StatusCode(500, new ApiResponse<TransactionResponse>
            {
                Success = false,
                Message = "An error occurred during transaction creation"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TransactionResponse>>> GetTransaction(Guid id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            
            if (transaction == null)
            {
                return NotFound(new ApiResponse<TransactionResponse>
                {
                    Success = false,
                    Message = "Transaction not found"
                });
            }

            var response = new TransactionResponse
            {
                Id = transaction.Id,
                CompanyId = transaction.CompanyId,
                FromUserId = transaction.FromUserId,
                ToUserId = transaction.ToUserId,
                Points = transaction.Points,
                DateAndTime = transaction.DateAndTime,
                Description = transaction.Description
            };

            return Ok(new ApiResponse<TransactionResponse>
            {
                Success = true,
                Message = "Transaction retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction with ID: {Id}", id);
            return StatusCode(500, new ApiResponse<TransactionResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving the transaction"
            });
        }
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<ApiResponse<List<TransactionResponse>>>> GetTransactionsByCompany(Guid companyId)
    {
        try
        {
            var transactions = await _transactionService.GetTransactionsByCompanyIdAsync(companyId);

            var response = transactions.Select(t => new TransactionResponse
            {
                Id = t.Id,
                CompanyId = t.CompanyId,
                FromUserId = t.FromUserId,
                ToUserId = t.ToUserId,
                Points = t.Points,
                DateAndTime = t.DateAndTime,
                Description = t.Description
            }).ToList();

            return Ok(new ApiResponse<List<TransactionResponse>>
            {
                Success = true,
                Message = "Transactions retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for company: {CompanyId}", companyId);
            return StatusCode(500, new ApiResponse<List<TransactionResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving transactions"
            });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<TransactionResponse>>>> GetTransactionsByUser(Guid userId)
    {
        try
        {
            var transactions = await _transactionService.GetTransactionsByUserIdAsync(userId);

            var response = transactions.Select(t => new TransactionResponse
            {
                Id = t.Id,
                CompanyId = t.CompanyId,
                FromUserId = t.FromUserId,
                ToUserId = t.ToUserId,
                Points = t.Points,
                DateAndTime = t.DateAndTime,
                Description = t.Description
            }).ToList();

            return Ok(new ApiResponse<List<TransactionResponse>>
            {
                Success = true,
                Message = "Transactions retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for user: {UserId}", userId);
            return StatusCode(500, new ApiResponse<List<TransactionResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving transactions"
            });
        }
    }

    [HttpGet("sent/{userId}")]
    public async Task<ActionResult<ApiResponse<List<TransactionResponse>>>> GetTransactionsSentByUser(Guid userId)
    {
        try
        {
            var transactions = await _transactionService.GetTransactionsByFromUserIdAsync(userId);

            var response = transactions.Select(t => new TransactionResponse
            {
                Id = t.Id,
                CompanyId = t.CompanyId,
                FromUserId = t.FromUserId,
                ToUserId = t.ToUserId,
                Points = t.Points,
                DateAndTime = t.DateAndTime,
                Description = t.Description
            }).ToList();

            return Ok(new ApiResponse<List<TransactionResponse>>
            {
                Success = true,
                Message = "Transactions retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions sent by user: {UserId}", userId);
            return StatusCode(500, new ApiResponse<List<TransactionResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving transactions"
            });
        }
    }

    [HttpGet("received/{userId}")]
    public async Task<ActionResult<ApiResponse<List<TransactionResponse>>>> GetTransactionsReceivedByUser(Guid userId)
    {
        try
        {
            var transactions = await _transactionService.GetTransactionsByToUserIdAsync(userId);

            var response = transactions.Select(t => new TransactionResponse
            {
                Id = t.Id,
                CompanyId = t.CompanyId,
                FromUserId = t.FromUserId,
                ToUserId = t.ToUserId,
                Points = t.Points,
                DateAndTime = t.DateAndTime,
                Description = t.Description
            }).ToList();

            return Ok(new ApiResponse<List<TransactionResponse>>
            {
                Success = true,
                Message = "Transactions retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions received by user: {UserId}", userId);
            return StatusCode(500, new ApiResponse<List<TransactionResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving transactions"
            });
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<List<TransactionResponse>>>> GetAllTransactions()
    {
        try
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();

            var response = transactions.Select(t => new TransactionResponse
            {
                Id = t.Id,
                CompanyId = t.CompanyId,
                FromUserId = t.FromUserId,
                ToUserId = t.ToUserId,
                Points = t.Points,
                DateAndTime = t.DateAndTime,
                Description = t.Description
            }).ToList();

            return Ok(new ApiResponse<List<TransactionResponse>>
            {
                Success = true,
                Message = "Transactions retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all transactions");
            return StatusCode(500, new ApiResponse<List<TransactionResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving transactions"
            });
        }
    }

    private string GetTokenFromRequest()
    {
        return Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
    }
}
