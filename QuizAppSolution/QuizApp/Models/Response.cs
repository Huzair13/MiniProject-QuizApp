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

        public int QuizId { get; set; }
        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        [NotMapped]
        public TimeSpan TimeTaken => (EndTime.HasValue) ? EndTime.Value - StartTime : TimeSpan.Zero;

        public ICollection<ResponseAnswer> ResponseAnswers { get; set; } = new List<ResponseAnswer>();
    }
}
