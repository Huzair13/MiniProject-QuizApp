namespace QuizApp.Models.DTOs
{
    public class LeaderboardDTO
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal ScoredPoints { get; set; }
        public decimal correctPercentage { get; set; }
        public TimeSpan? TimeTaken { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

}
