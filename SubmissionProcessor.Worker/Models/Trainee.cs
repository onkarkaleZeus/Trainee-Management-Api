using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubmissionProcessor.Worker.Models
{
    public class Trainee
    {   
        [Key]   
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName is required.")][MaxLength(50)]
        public string FirstName { get; set; } = "" ;

        [Required(ErrorMessage = "LastName is required.")][MaxLength(50)]
        public string LastName { get; set; } = "" ;

        [Required(ErrorMessage = "Email is required.")][EmailAddress]
        public string Email { get; set; } = "" ;

        [Required(ErrorMessage = "Tech Stack is required.")]
        public string TechStack { get; set; } = "" ;

        [Required(ErrorMessage = "Status is required.")]
        public TraineeStatus Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now ;
        public DateTime UpdatedDate { get; set; } = DateTime.Now ;
    }

    public enum TraineeStatus
    {
        Active,
        Inactive,
        Completed
    }

}

