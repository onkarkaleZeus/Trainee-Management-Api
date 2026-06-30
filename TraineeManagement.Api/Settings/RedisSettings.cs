namespace TraineeManagement.Api.Settings
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty ;
        public string InstanceName { get; set; } = string.Empty ;
        public int TTL { get; set; }
        public int SlidingExpiration { get; set; }
    }
}