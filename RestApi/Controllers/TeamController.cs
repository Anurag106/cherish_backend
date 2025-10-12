using Cherish.RestApi.Models.Requests;
using Cherish.RestApi.Models.Responses;
using Domain.Services;
using Domain.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cherish.RestApi.Controllers;


[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class TeamController : ControllerBase
{
    private readonly ITeamService _teamService;
    private readonly IUserProvider _userProvider;
    private readonly ILogger<TeamController> _logger;

    public TeamController(ITeamService teamService, IUserProvider userProvider, ILogger<TeamController> logger)
    {
        _teamService = teamService;
        _userProvider = userProvider;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<TeamResponse>>> CreateTeam([FromBody] CreateTeamRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Name) || request.ManagerId == Guid.Empty || request.CompanyId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<TeamResponse>
                {
                    Success = false,
                    Message = "Team name, manager ID, and company ID are required"
                });
            }

            var team = await _teamService.CreateTeamAsync(request.Name, request.ManagerId, request.CompanyId);
            
            if (team == null)
            {
                return Conflict(new ApiResponse<TeamResponse>
                {
                    Success = false,
                    Message = "Team with this name already exists in the company"
                });
            }

            // Fetch employee information (though newly created team might not have employees yet)
            var employees = new List<EmployeeInfo>();
            foreach (var employeeId in team.EmployeeIds)
            {
                var employee = await _userProvider.GetUserByIdAsync(employeeId);
                if (employee != null)
                {
                    employees.Add(new EmployeeInfo
                    {
                        Id = employee.Id,
                        FullName = $"{employee.FirstName} {employee.LastName}".Trim()
                    });
                }
            }

            var response = new TeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                ManagerId = team.ManagerId,
                CompanyId = team.CompanyId,
                EmployeeIds = team.EmployeeIds,
                Employees = employees,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt
            };

            return Ok(new ApiResponse<TeamResponse>
            {
                Success = true,
                Message = "Team created successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during team creation for name: {Name}", request.Name);
            return StatusCode(500, new ApiResponse<TeamResponse>
            {
                Success = false,
                Message = "An error occurred during team creation"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TeamResponse>>> GetTeam(Guid id)
    {
        try
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            
            if (team == null)
            {
                return NotFound(new ApiResponse<TeamResponse>
                {
                    Success = false,
                    Message = "Team not found"
                });
            }

            // Fetch employee information
            var employees = new List<EmployeeInfo>();
            foreach (var employeeId in team.EmployeeIds)
            {
                var employee = await _userProvider.GetUserByIdAsync(employeeId);
                if (employee != null)
                {
                    employees.Add(new EmployeeInfo
                    {
                        Id = employee.Id,
                        FullName = $"{employee.FirstName} {employee.LastName}".Trim()
                    });
                }
            }

            var response = new TeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                ManagerId = team.ManagerId,
                CompanyId = team.CompanyId,
                EmployeeIds = team.EmployeeIds,
                Employees = employees,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt
            };

            return Ok(new ApiResponse<TeamResponse>
            {
                Success = true,
                Message = "Team retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team with ID: {Id}", id);
            return StatusCode(500, new ApiResponse<TeamResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving the team"
            });
        }
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<ApiResponse<List<TeamResponse>>>> GetTeamsByCompany(Guid companyId)
    {
        try
        {
            var teams = await _teamService.GetTeamsByCompanyIdAsync(companyId);

            var response = new List<TeamResponse>();
            foreach (var team in teams)
            {
                // Fetch employee information for each team
                var employees = new List<EmployeeInfo>();
                foreach (var employeeId in team.EmployeeIds)
                {
                    var employee = await _userProvider.GetUserByIdAsync(employeeId);
                    if (employee != null)
                    {
                        employees.Add(new EmployeeInfo
                        {
                            Id = employee.Id,
                            FullName = $"{employee.FirstName} {employee.LastName}".Trim()
                        });
                    }
                }

                response.Add(new TeamResponse
                {
                    Id = team.Id,
                    Name = team.Name,
                    ManagerId = team.ManagerId,
                    CompanyId = team.CompanyId,
                    EmployeeIds = team.EmployeeIds,
                    Employees = employees,
                    CreatedAt = team.CreatedAt,
                    UpdatedAt = team.UpdatedAt
                });
            }

            return Ok(new ApiResponse<List<TeamResponse>>
            {
                Success = true,
                Message = "Teams retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teams for company: {CompanyId}", companyId);
            return StatusCode(500, new ApiResponse<List<TeamResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving teams"
            });
        }
    }

    [HttpGet("manager/{managerId}")]
    public async Task<ActionResult<ApiResponse<List<TeamResponse>>>> GetTeamsByManager(Guid managerId)
    {
        try
        {
            var teams = await _teamService.GetTeamsByManagerIdAsync(managerId);

            var response = new List<TeamResponse>();
            foreach (var team in teams)
            {
                // Fetch employee information for each team
                var employees = new List<EmployeeInfo>();
                foreach (var employeeId in team.EmployeeIds)
                {
                    var employee = await _userProvider.GetUserByIdAsync(employeeId);
                    if (employee != null)
                    {
                        employees.Add(new EmployeeInfo
                        {
                            Id = employee.Id,
                            FullName = $"{employee.FirstName} {employee.LastName}".Trim()
                        });
                    }
                }

                response.Add(new TeamResponse
                {
                    Id = team.Id,
                    Name = team.Name,
                    ManagerId = team.ManagerId,
                    CompanyId = team.CompanyId,
                    EmployeeIds = team.EmployeeIds,
                    Employees = employees,
                    CreatedAt = team.CreatedAt,
                    UpdatedAt = team.UpdatedAt
                });
            }

            return Ok(new ApiResponse<List<TeamResponse>>
            {
                Success = true,
                Message = "Teams retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teams for manager: {ManagerId}", managerId);
            return StatusCode(500, new ApiResponse<List<TeamResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving teams"
            });
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<List<TeamResponse>>>> GetAllTeams()
    {
        try
        {
            var teams = await _teamService.GetAllTeamsAsync();

            var response = new List<TeamResponse>();
            foreach (var team in teams)
            {
                // Fetch employee information for each team
                var employees = new List<EmployeeInfo>();
                foreach (var employeeId in team.EmployeeIds)
                {
                    var employee = await _userProvider.GetUserByIdAsync(employeeId);
                    if (employee != null)
                    {
                        employees.Add(new EmployeeInfo
                        {
                            Id = employee.Id,
                            FullName = $"{employee.FirstName} {employee.LastName}".Trim()
                        });
                    }
                }

                response.Add(new TeamResponse
                {
                    Id = team.Id,
                    Name = team.Name,
                    ManagerId = team.ManagerId,
                    CompanyId = team.CompanyId,
                    EmployeeIds = team.EmployeeIds,
                    Employees = employees,
                    CreatedAt = team.CreatedAt,
                    UpdatedAt = team.UpdatedAt
                });
            }

            return Ok(new ApiResponse<List<TeamResponse>>
            {
                Success = true,
                Message = "Teams retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all teams");
            return StatusCode(500, new ApiResponse<List<TeamResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving teams"
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TeamResponse>>> UpdateTeam(Guid id, [FromBody] UpdateTeamRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Name) || request.ManagerId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<TeamResponse>
                {
                    Success = false,
                    Message = "Team name and manager ID are required"
                });
            }

            var team = await _teamService.UpdateTeamAsync(id, request.Name, request.ManagerId);
            
            if (team == null)
            {
                return NotFound(new ApiResponse<TeamResponse>
                {
                    Success = false,
                    Message = "Team not found or another team with this name already exists in the company"
                });
            }

            // Fetch employee information
            var employees = new List<EmployeeInfo>();
            foreach (var employeeId in team.EmployeeIds)
            {
                var employee = await _userProvider.GetUserByIdAsync(employeeId);
                if (employee != null)
                {
                    employees.Add(new EmployeeInfo
                    {
                        Id = employee.Id,
                        FullName = $"{employee.FirstName} {employee.LastName}".Trim()
                    });
                }
            }

            var response = new TeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                ManagerId = team.ManagerId,
                CompanyId = team.CompanyId,
                EmployeeIds = team.EmployeeIds,
                Employees = employees,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt
            };

            return Ok(new ApiResponse<TeamResponse>
            {
                Success = true,
                Message = "Team updated successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team with ID: {Id}", id);
            return StatusCode(500, new ApiResponse<TeamResponse>
            {
                Success = false,
                Message = "An error occurred while updating the team"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteTeam(Guid id)
    {
        try
        {
            var success = await _teamService.DeleteTeamAsync(id);
            
            if (!success)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Team not found"
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Team deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting team with ID: {Id}", id);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while deleting the team"
            });
        }
    }

    [HttpPost("{id}/employees")]
    public async Task<ActionResult<ApiResponse>> AddEmployeeToTeam(Guid id, [FromBody] AddEmployeeToTeamRequest request)
    {
        try
        {
            if (request.EmployeeId == Guid.Empty)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Employee ID is required"
                });
            }

            var success = await _teamService.AddEmployeeToTeamAsync(id, request.EmployeeId);
            
            return Ok(new ApiResponse
            {
                Success = success,
                Message = success ? "Employee added to team successfully" : "Employee already in team or team not found"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding employee {EmployeeId} to team {TeamId}", request.EmployeeId, id);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while adding employee to team"
            });
        }
    }

    [HttpDelete("{id}/employees/{employeeId}")]
    public async Task<ActionResult<ApiResponse>> RemoveEmployeeFromTeam(Guid id, Guid employeeId)
    {
        try
        {
            var success = await _teamService.RemoveEmployeeFromTeamAsync(id, employeeId);
            
            return Ok(new ApiResponse
            {
                Success = success,
                Message = success ? "Employee removed from team successfully" : "Employee not in team or team not found"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing employee {EmployeeId} from team {TeamId}", employeeId, id);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while removing employee from team"
            });
        }
    }
}
