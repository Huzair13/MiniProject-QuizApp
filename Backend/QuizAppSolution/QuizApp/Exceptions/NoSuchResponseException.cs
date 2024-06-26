namespace QuizApp.Exceptions
{
    public class NoSuchResponseException : Exception
    {
        string ExceptionMessage;
        public NoSuchResponseException()
        {
            ExceptionMessage = "Response Not Found";
        }
        public NoSuchResponseException(int responseId)
        {
            ExceptionMessage = $"Response with the QuizId : {responseId} not found";
        }
        public NoSuchResponseException(string message)
        {
            ExceptionMessage = message;
        }
        public override string Message => ExceptionMessage;
    }
}
