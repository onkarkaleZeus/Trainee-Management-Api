namespace TraineeManagement.Api.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream stream, string extension);
        Task<Stream> OpenReadAsync(string GeneratedFileName);
        Task<bool> ExistsAsync(string GeneratedFileName);
        Task DeleteAsync(string GeneratedFileName);
    }
}