namespace QuizApp.Exceptions
{
    public class QuizNotStartedException : Exception
    {
        string ExceptionMessage;
        public QuizNotStartedException()
        {
            ExceptionMessage = "Quiz Not Started";
        }
        public QuizNotStartedException(int quizId)
        {
            ExceptionMessage = $"Quiz with the QuizId : {quizId} not started";
        }
        public QuizNotStartedException(string message)
        {
            ExceptionMessage = message;
        }
        public override string Message => ExceptionMessage;
    }
}
