using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizIDDTO
    {
        [Required(ErrorMessage = "Quiz ID is Required")]
        public int QuizId { get; set; }

    }
}
