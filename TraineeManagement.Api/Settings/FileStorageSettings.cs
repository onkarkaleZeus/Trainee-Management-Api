namespace TraineeManagement.Api.Settings
{
    public class FileStorageSettings
    {
        public string RootPath { get; set; } = string.Empty;
        public long MaxFileSizeBytes { get; set; }
        public List<string> AllowedExtentions { get; set; } = [];
        public List<string> AllowedContentTypes { get; set; } = [];
    }
}