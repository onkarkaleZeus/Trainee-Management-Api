using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubmissionProcessor.Worker.Models
{
    public class SubmissionFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "File name is required")]
        public string OriginalFileName { get; set; } = string.Empty ;

        public string GeneratedFileName { get; set; } = string.Empty ;

        public string ContentType { get; set; } = string.Empty ;

        [Required(ErrorMessage = "File size is required")]
        public long FileSize { get; set; } 


        [Required(ErrorMessage = "CheckSum is required")]
        public string CheckSum { get; set; } = string.Empty ;

        [ForeignKey(nameof(Submission))]
        public int SubmissionId { get; set; } 
        public Submission Submission { get; set; } = null! ;

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; } = null!; 

        public DateTime CreatedAt { get; set; } = DateTime.Now ;

        public DateTime UpdatedAt { get; set; } = DateTime.Now ;

    }
}