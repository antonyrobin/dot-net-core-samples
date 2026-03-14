using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> StoreAsync(IBrowserFile file);
    }
}
