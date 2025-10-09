using Domain.Services;
using Domain.Providers;
using Domain.Models;

namespace Cherish.RestApi.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyProvider _companyProvider;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(ICompanyProvider companyProvider, ILogger<CompanyService> logger)
    {
        _companyProvider = companyProvider;
        _logger = logger;
    }

    public async Task<Company?> GetCompanyByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get company with empty GUID");
                return null;
            }

            return await _companyProvider.GetCompanyByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company with ID: {Id}", id);
            throw;
        }
    }

    public async Task<Company?> GetCompanyByNameAsync(string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Attempted to get company with empty name");
                return null;
            }

            return await _companyProvider.GetCompanyByNameAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company with name: {Name}", name);
            throw;
        }
    }

    public async Task<Company?> CreateCompanyAsync(string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Attempted to create company with empty name");
                return null;
            }

            // Check if company already exists
            var existingCompany = await _companyProvider.GetCompanyByNameAsync(name);
            if (existingCompany != null)
            {
                _logger.LogWarning("Attempted to create company with existing name: {Name}", name);
                return null;
            }

            return await _companyProvider.CreateCompanyAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company with name: {Name}", name);
            throw;
        }
    }

    public async Task<Company?> UpdateCompanyAsync(Guid id, string name)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to update company with empty GUID");
                return null;
            }

            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Attempted to update company with empty name");
                return null;
            }

            // Check if company exists
            var existingCompany = await _companyProvider.GetCompanyByIdAsync(id);
            if (existingCompany == null)
            {
                _logger.LogWarning("Attempted to update non-existent company with ID: {Id}", id);
                return null;
            }

            // Check if another company with the same name exists
            var companyWithSameName = await _companyProvider.GetCompanyByNameAsync(name);
            if (companyWithSameName != null && companyWithSameName.Id != id)
            {
                _logger.LogWarning("Attempted to update company to existing name: {Name}", name);
                return null;
            }

            return await _companyProvider.UpdateCompanyAsync(id, name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company with ID: {Id} and name: {Name}", id, name);
            throw;
        }
    }

    public async Task<bool> DeleteCompanyAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to delete company with empty GUID");
                return false;
            }

            // Check if company exists
            var existingCompany = await _companyProvider.GetCompanyByIdAsync(id);
            if (existingCompany == null)
            {
                _logger.LogWarning("Attempted to delete non-existent company with ID: {Id}", id);
                return false;
            }

            return await _companyProvider.DeleteCompanyAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company with ID: {Id}", id);
            throw;
        }
    }

    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        try
        {
            return await _companyProvider.GetAllCompaniesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all companies");
            throw;
        }
    }
}
