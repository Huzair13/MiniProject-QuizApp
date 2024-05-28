using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class ResponseAnswer
    {
        [Key]
        public int Id { get; set; }

        public int ResponseId { get; set; }
        [ForeignKey("ResponseId")]
        public Response Response { get; set; }

        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        public string SubmittedAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
