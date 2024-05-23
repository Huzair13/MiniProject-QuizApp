namespace QuizApp.Models
{
    public class MultipleChoice :Question
    {
        public string Choice1 { get; set; }
        public string Choice2 { get; set; }
        public string Choice3 { get; set; }
        public string Choice4 { get; set; }
        public string CorrectChoice { get; set; }
    }
}
