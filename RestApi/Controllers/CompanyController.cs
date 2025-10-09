using Cherish.RestApi.Models.Requests;
using Cherish.RestApi.Models.Responses;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cherish.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> CreateCompany([FromBody] CreateCompanyRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new ApiResponse<CompanyResponse>
                {
                    Success = false,
                    Message = "Company name is required"
                });
            }

            var company = await _companyService.CreateCompanyAsync(request.Name);
            
            if (company == null)
            {
                return Conflict(new ApiResponse<CompanyResponse>
                {
                    Success = false,
                    Message = "Company with this name already exists"
                });
            }

            var response = new CompanyResponse
            {
                Id = company.Id,
                Name = company.Name,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };

            return Ok(new ApiResponse<CompanyResponse>
            {
                Success = true,
                Message = "Company created successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during company creation for name: {Name}", request.Name);
            return StatusCode(500, new ApiResponse<CompanyResponse>
            {
                Success = false,
                Message = "An error occurred during company creation"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> GetCompany(Guid id)
    {
        try
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            
            if (company == null)
            {
                return NotFound(new ApiResponse<CompanyResponse>
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            var response = new CompanyResponse
            {
                Id = company.Id,
                Name = company.Name,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };

            return Ok(new ApiResponse<CompanyResponse>
            {
                Success = true,
                Message = "Company retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company with ID: {Id}", id);
            return StatusCode(500, new ApiResponse<CompanyResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving the company"
            });
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<List<CompanyResponse>>>> GetAllCompanies()
    {
        try
        {
            var companies = await _companyService.GetAllCompaniesAsync();

            var response = companies.Select(c => new CompanyResponse
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();

            return Ok(new ApiResponse<List<CompanyResponse>>
            {
                Success = true,
                Message = "Companies retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all companies");
            return StatusCode(500, new ApiResponse<List<CompanyResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving companies"
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> UpdateCompany(Guid id, [FromBody] UpdateCompanyRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new ApiResponse<CompanyResponse>
                {
                    Success = false,
                    Message = "Company name is required"
                });
            }

            // Check if company exists
            var existingCompany = await _companyService.GetCompanyByIdAsync(id);
            if (existingCompany == null)
            {
                return NotFound(new ApiResponse<CompanyResponse>
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            // Check if another company with the same name exists
            var companyWithSameName = await _companyService.GetCompanyByNameAsync(request.Name);
            if (companyWithSameName != null && companyWithSameName.Id != id)
            {
                return Conflict(new ApiResponse<CompanyResponse>
                {
                    Success = false,
                    Message = "Another company with this name already exists"
                });
            }

            var updatedCompany = await _companyService.UpdateCompanyAsync(id, request.Name);

            var response = new CompanyResponse
            {
                Id = updatedCompany.Id,
                Name = updatedCompany.Name,
                CreatedAt = updatedCompany.CreatedAt,
                UpdatedAt = updatedCompany.UpdatedAt
            };

            return Ok(new ApiResponse<CompanyResponse>
            {
                Success = true,
                Message = "Company updated successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company with ID: {Id}", id);
            return StatusCode(500, new ApiResponse<CompanyResponse>
            {
                Success = false,
                Message = "An error occurred while updating the company"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteCompany(Guid id)
    {
        try
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            var success = await _companyService.DeleteCompanyAsync(id);
            
            return Ok(new ApiResponse
            {
                Success = success,
                Message = success ? "Company deleted successfully" : "Failed to delete company"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company with ID: {Id}", id);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while deleting the company"
            });
        }
    }
}
