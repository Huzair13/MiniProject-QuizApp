using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Models
{
    public class Response
    {
        [Key]
        public int Id { get; set; }
        public decimal ScoredPoints { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public TimeSpan TimeTaken { get; set; }
    }
}
