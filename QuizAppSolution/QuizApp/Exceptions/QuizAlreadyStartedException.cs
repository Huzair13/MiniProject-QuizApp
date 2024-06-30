namespace QuizApp.Exceptions
{
    public class QuizAlreadyStartedException :Exception
    {
        string ExceptionMessage;
        public QuizAlreadyStartedException()
        {
            ExceptionMessage = "Quiz Already started";
        }
        public QuizAlreadyStartedException(int quizId)
        {
            ExceptionMessage = $"Quiz with the QuizId : {quizId} already started by you";
        }
        public QuizAlreadyStartedException(string message)
        {
            ExceptionMessage = message;
        }
        public override string Message => ExceptionMessage;
    }
}
