namespace QuizApp.Exceptions
{
    public class NoSuchQuestionException :Exception
    {
        string ExceptionMessage;
        public NoSuchQuestionException()
        {
            ExceptionMessage = "Question Not Found";
        }
        public NoSuchQuestionException(int QuestionId)
        {
            ExceptionMessage = $"No Question found for the QuestionId {QuestionId}";
        }
        public NoSuchQuestionException(string message)
        {
            ExceptionMessage = message;
        }
        public override string Message => ExceptionMessage;
    }
}
