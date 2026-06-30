
namespace TrainingDirectory.Api.DTOs.TraineeProfileDto
{
    public class TraineeProfileResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty ;

        public string Designation { get; set; } = string.Empty ;
    }
}