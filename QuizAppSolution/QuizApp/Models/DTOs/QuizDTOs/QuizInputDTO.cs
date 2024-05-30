using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizInputDTO
    {
        [Required(ErrorMessage = "Quiz Name is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Quiz Name must be between 5 and 100 characters.")]
        public string QuizName { get; set; }
        
        [Required(ErrorMessage = "Quiz Description is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Quiz Description must be between 10 and 500 characters.")]
        public string QuizDescription { get; set; }

        [Required(ErrorMessage = "Quiz Type is required.")]
        [StringLength(50, ErrorMessage = "Quiz Type must be up to 50 characters.")]
        public string QuizType { get; set; }
        public bool IsMultipleAttemptAllowed { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "24:00:00", ErrorMessage = "Time Limit must be a valid time span between 00:00:00 and 24:00:00.")]
        public TimeSpan TimeLimit { get; set; }

        [Required(ErrorMessage = "At least one Question ID is required.")]
        [MinLength(1, ErrorMessage = "At least one Question ID is required.")]
        public List<int> QuestionIds { get; set; }
    }
}
