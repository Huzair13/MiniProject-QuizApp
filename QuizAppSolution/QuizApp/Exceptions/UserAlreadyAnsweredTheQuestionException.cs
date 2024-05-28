namespace QuizApp.Exceptions
{
    public class UserAlreadyAnsweredTheQuestionException : Exception
    {
        string ExceptionMessage;
        public UserAlreadyAnsweredTheQuestionException()
        {
            ExceptionMessage = "User Already Asnwered the Question";
        }
        public UserAlreadyAnsweredTheQuestionException(string message)
        {
            ExceptionMessage = message;
        }
        public override string Message => ExceptionMessage;
    }
}
