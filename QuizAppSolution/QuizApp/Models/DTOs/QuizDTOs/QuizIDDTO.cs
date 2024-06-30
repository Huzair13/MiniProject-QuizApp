using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizApp.Models.DTOs.QuizDTOs
{
    [ExcludeFromCodeCoverage]
    public class QuizIDDTO
    {
        [Required(ErrorMessage = "Quiz ID is Required")]
        public int QuizId { get; set; }

    }
}
