using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class AddQuestionsToQuizDTO
    {
        [Required(ErrorMessage = "Quiz ID is required.")]
        public int QuizId { get; set; }

        [MinLength(1, ErrorMessage = "At least one Question ID is required.")]
        public List<int> QuestionIds { get; set; }
    }
}
