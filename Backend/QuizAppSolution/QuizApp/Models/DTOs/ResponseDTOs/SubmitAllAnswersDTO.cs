using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.DTOs.ResponseDTO
{
    public class SubmitAllAnswersDTO
    {
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Quiz ID is required.")]
        public int QuizId { get; set; }

        [Required(ErrorMessage = "Question answers are required.")]
        [MinLength(1, ErrorMessage = "At least one question answer is required.")]
        public Dictionary<int, string> QuestionAnswers { get; set; }
    }
}
