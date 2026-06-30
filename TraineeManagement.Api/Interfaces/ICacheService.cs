namespace TraineeManagement.Api.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T t);

        Task DeleteAsync(string key);
    }
}