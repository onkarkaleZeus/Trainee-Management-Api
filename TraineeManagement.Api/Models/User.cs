using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagement.Api.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = "" ;

        [Required(ErrorMessage = "Email is required.")][EmailAddress]
        public string Email { get; set; } = "" ;

        [Required( ErrorMessage = "Password is required." )]
        public string PasswordHash { get; set; } = "" ;

        [Required(ErrorMessage = "Role is required.")]
        public UserRole Role { get; set; } = UserRole.Trainee ;

        public DateTime CreatedDate { get; set; } = DateTime.Now ;
        public DateTime UpdatedDate { get; set; } = DateTime.Now ;
    }

    public enum UserRole
    {
        Admin,
        Mentor,
        Trainee
    }
}