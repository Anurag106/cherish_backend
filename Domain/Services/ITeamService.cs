using Domain.Models;

namespace Domain.Services;

public interface ITeamService
{
    Task<Team?> GetTeamByIdAsync(Guid id);
    Task<Team?> GetTeamByNameAsync(string name);
    Task<List<Team>> GetTeamsByCompanyIdAsync(Guid companyId);
    Task<List<Team>> GetTeamsByManagerIdAsync(Guid managerId);
    Task<Team?> CreateTeamAsync(string name, Guid managerId, Guid companyId);
    Task<Team?> UpdateTeamAsync(Guid id, string name, Guid managerId);
    Task<bool> DeleteTeamAsync(Guid id);
    Task<bool> AddEmployeeToTeamAsync(Guid teamId, Guid employeeId);
    Task<bool> RemoveEmployeeFromTeamAsync(Guid teamId, Guid employeeId);
    Task<List<Team>> GetAllTeamsAsync();
}
