using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizUpdateDTO
    {
        [Required(ErrorMessage = "Quiz ID is required.")]
        public int QuizID { get; set; }

        [StringLength(100, MinimumLength = 5, ErrorMessage = "Quiz Name must be between 5 and 100 characters.")]
        public string? QuizName { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Quiz Description must be between 10 and 500 characters.")]
        public string? QuizDescription { get; set; }

        [StringLength(50, ErrorMessage = "Quiz Type must be up to 50 characters.")]
        public string? QuizType { get; set; }

        public bool? IsMultipleAttemptAllowed { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "24:00:00", ErrorMessage = "Time Limit must be a valid time span between 00:00:00 and 24:00:00.")]
        public TimeSpan? TimeLimit { get; set; }

        public List<int>? QuestionIds { get; set; }

    }
}
