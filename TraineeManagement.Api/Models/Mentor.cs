using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagement.Api.Models
{
    public class Mentor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Firstname is Required")]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Lastname is Required")]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Expertise is required")]
        public string Expertise { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Mentor Status is required")]
        public MentorStatus Status { get; set; } = MentorStatus.Active;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

    }

    public enum MentorStatus
    {
        Active,
        Inactive
    }
}