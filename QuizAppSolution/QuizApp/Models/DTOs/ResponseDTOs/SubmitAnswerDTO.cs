using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.DTOs.ResponseDTO
{
    public class SubmitAnswerDTO
    {
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Quiz ID is required.")]
        public int QuizId { get; set; }

        [Required(ErrorMessage = "Question ID is required.")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Answer is required.")]
        public string Answer { get; set; }
    }
}
