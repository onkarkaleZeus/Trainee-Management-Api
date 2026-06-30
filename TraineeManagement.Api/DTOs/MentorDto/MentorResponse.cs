using TraineeManagement.Api.Models; 

namespace TraineeManagement.Api.DTOs.MentorDto
{
    public class MentorResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty ;

        public string LastName { get; set; } = string.Empty ;

        public string Email { get; set; } = string.Empty ;

        public string Expertise { get; set; } = string.Empty ;

        public MentorStatus Status { get; set; } = MentorStatus.Active;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

    }
}