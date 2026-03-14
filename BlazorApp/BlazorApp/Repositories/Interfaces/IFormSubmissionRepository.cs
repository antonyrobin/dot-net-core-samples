using BlazorApp.Models;

namespace BlazorApp.Repositories.Interfaces
{
    public interface IFormSubmissionRepository
    {
        Task<List<FormSubmission>> GetAllAsync();
        Task<FormSubmission?> GetByIdAsync(string id);
        Task SaveAsync(FormSubmission submission);
        Task UpdateAsync(FormSubmission submission);
        Task DeleteAsync(string id);
    }
}
