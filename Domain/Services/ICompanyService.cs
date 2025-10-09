using Domain.Models;

namespace Domain.Services;

public interface ICompanyService
{
    Task<Company?> GetCompanyByIdAsync(Guid id);
    Task<Company?> GetCompanyByNameAsync(string name);
    Task<Company?> CreateCompanyAsync(string name);
    Task<Company?> UpdateCompanyAsync(Guid id, string name);
    Task<bool> DeleteCompanyAsync(Guid id);
    Task<List<Company>> GetAllCompaniesAsync();
}
