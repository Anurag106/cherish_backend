using Domain.Services;
using Domain.Providers;
using Domain.Models;

namespace Cherish.RestApi.Services;

public class TeamService : ITeamService
{
    private readonly ITeamProvider _teamProvider;
    private readonly IUserProvider _userProvider;
    private readonly ICompanyProvider _companyProvider;
    private readonly ILogger<TeamService> _logger;

    public TeamService(
        ITeamProvider teamProvider, 
        IUserProvider userProvider, 
        ICompanyProvider companyProvider,
        ILogger<TeamService> logger)
    {
        _teamProvider = teamProvider;
        _userProvider = userProvider;
        _companyProvider = companyProvider;
        _logger = logger;
    }

    public async Task<Team?> GetTeamByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get team with empty GUID");
                return null;
            }

            return await _teamProvider.GetTeamByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team with ID: {Id}", id);
            throw;
        }
    }

    public async Task<Team?> GetTeamByNameAsync(string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Attempted to get team with empty name");
                return null;
            }

            return await _teamProvider.GetTeamByNameAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team with name: {Name}", name);
            throw;
        }
    }

    public async Task<List<Team>> GetTeamsByCompanyIdAsync(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get teams with empty company GUID");
                return new List<Team>();
            }

            // Verify company exists
            var company = await _companyProvider.GetCompanyByIdAsync(companyId);
            if (company == null)
            {
                _logger.LogWarning("Attempted to get teams for non-existent company: {CompanyId}", companyId);
                return new List<Team>();
            }

            return await _teamProvider.GetTeamsByCompanyIdAsync(companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teams for company: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<List<Team>> GetTeamsByManagerIdAsync(Guid managerId)
    {
        try
        {
            if (managerId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get teams with empty manager GUID");
                return new List<Team>();
            }

            // Verify manager exists
            var manager = await _userProvider.GetUserByUsernameAsync(""); // We need a method to get user by ID
            if (manager == null)
            {
                _logger.LogWarning("Attempted to get teams for non-existent manager: {ManagerId}", managerId);
                return new List<Team>();
            }

            return await _teamProvider.GetTeamsByManagerIdAsync(managerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teams for manager: {ManagerId}", managerId);
            throw;
        }
    }

    public async Task<Team?> CreateTeamAsync(string name, Guid managerId, Guid companyId)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Attempted to create team with empty name");
                return null;
            }

            if (managerId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to create team with empty manager GUID");
                return null;
            }

            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to create team with empty company GUID");
                return null;
            }

            // Check if team already exists in the company
            var existingTeams = await _teamProvider.GetTeamsByCompanyIdAsync(companyId);
            if (existingTeams.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Attempted to create team with existing name in company: {Name}", name);
                return null;
            }

            // Verify manager exists (we'll need to add a method to get user by ID)
            // For now, we'll assume the manager exists

            // Verify company exists
            var company = await _companyProvider.GetCompanyByIdAsync(companyId);
            if (company == null)
            {
                _logger.LogWarning("Attempted to create team for non-existent company: {CompanyId}", companyId);
                return null;
            }

            return await _teamProvider.CreateTeamAsync(name, managerId, companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team with name: {Name}", name);
            throw;
        }
    }

    public async Task<Team?> UpdateTeamAsync(Guid id, string name, Guid managerId)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to update team with empty GUID");
                return null;
            }

            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Attempted to update team with empty name");
                return null;
            }

            if (managerId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to update team with empty manager GUID");
                return null;
            }

            // Check if team exists
            var existingTeam = await _teamProvider.GetTeamByIdAsync(id);
            if (existingTeam == null)
            {
                _logger.LogWarning("Attempted to update non-existent team with ID: {Id}", id);
                return null;
            }

            // Check if another team with the same name exists in the same company
            var companyTeams = await _teamProvider.GetTeamsByCompanyIdAsync(existingTeam.CompanyId);
            if (companyTeams.Any(t => t.Id != id && t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Attempted to update team to existing name in company: {Name}", name);
                return null;
            }

            return await _teamProvider.UpdateTeamAsync(id, name, managerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team with ID: {Id} and name: {Name}", id, name);
            throw;
        }
    }

    public async Task<bool> DeleteTeamAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to delete team with empty GUID");
                return false;
            }

            // Check if team exists
            var existingTeam = await _teamProvider.GetTeamByIdAsync(id);
            if (existingTeam == null)
            {
                _logger.LogWarning("Attempted to delete non-existent team with ID: {Id}", id);
                return false;
            }

            return await _teamProvider.DeleteTeamAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting team with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> AddEmployeeToTeamAsync(Guid teamId, Guid employeeId)
    {
        try
        {
            if (teamId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to add employee to team with empty team GUID");
                return false;
            }

            if (employeeId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to add employee with empty GUID to team");
                return false;
            }

            // Check if team exists
            var team = await _teamProvider.GetTeamByIdAsync(teamId);
            if (team == null)
            {
                _logger.LogWarning("Attempted to add employee to non-existent team: {TeamId}", teamId);
                return false;
            }


            // Check if employee is already in the team
            if (team.EmployeeIds.Contains(employeeId))
            {
                _logger.LogWarning("Employee {EmployeeId} is already in team {TeamId}", employeeId, teamId);
                return false;
            }

            return await _teamProvider.AddEmployeeToTeamAsync(teamId, employeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding employee {EmployeeId} to team {TeamId}", employeeId, teamId);
            throw;
        }
    }

    public async Task<bool> RemoveEmployeeFromTeamAsync(Guid teamId, Guid employeeId)
    {
        try
        {
            if (teamId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to remove employee from team with empty team GUID");
                return false;
            }

            if (employeeId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to remove employee with empty GUID from team");
                return false;
            }

            // Check if team exists
            var team = await _teamProvider.GetTeamByIdAsync(teamId);
            if (team == null)
            {
                _logger.LogWarning("Attempted to remove employee from non-existent team: {TeamId}", teamId);
                return false;
            }

            return await _teamProvider.RemoveEmployeeFromTeamAsync(teamId, employeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing employee {EmployeeId} from team {TeamId}", employeeId, teamId);
            throw;
        }
    }

    public async Task<List<Team>> GetAllTeamsAsync()
    {
        try
        {
            return await _teamProvider.GetAllTeamsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all teams");
            throw;
        }
    }
}
