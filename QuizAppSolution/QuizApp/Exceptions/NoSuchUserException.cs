namespace QuizApp.Exceptions
{
    public class NoSuchUserException : Exception
    {
        string exceptionMessage;
        public NoSuchUserException(int message)
        {
            exceptionMessage = $"No user with the given ID : {message}";
        }
        public NoSuchUserException()
        {
            exceptionMessage = "No such user found";
        }
        public override string Message => exceptionMessage;
    }
}
