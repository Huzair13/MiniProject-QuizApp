namespace QuizApp.Exceptions
{
    public class NoSuchQuizException : Exception
    {
        string ExceptionMessage;
        public NoSuchQuizException()
        {
            ExceptionMessage = "Quiz Not Found";
        }
        public NoSuchQuizException(int quizId)
        {
            ExceptionMessage = $"Quiz with the QuizId : {quizId} not found";
        }
        public NoSuchQuizException(string message)
        {
            ExceptionMessage = message;
        }
        public override string Message => ExceptionMessage;
    }
}
