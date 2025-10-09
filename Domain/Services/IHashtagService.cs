using Domain.Models;

namespace Domain.Services;

public interface IHashtagService
{
    Task<Hashtag?> GetHashtagByIdAsync(int id);
    Task<Hashtag?> GetHashtagByNameAsync(string name, Guid companyId);
    Task<List<Hashtag>> GetAllHashtagsAsync();
    Task<List<Hashtag>> GetHashtagsByCompanyIdAsync(Guid companyId);
    Task<List<Hashtag>> GetHashtagsByCreatedByAsync(Guid createdBy);
    Task<Hashtag?> CreateHashtagAsync(string name, string description, Guid companyId, Guid createdBy);
    Task<Hashtag?> UpdateHashtagAsync(int id, string name, string description, Guid modifiedBy);
    Task<bool> DeleteHashtagAsync(int id);
}
