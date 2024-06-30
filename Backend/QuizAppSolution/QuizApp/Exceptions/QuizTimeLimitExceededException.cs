namespace QuizApp.Exceptions
{
    public class QuizTimeLimitExceededException :Exception
    {
        string exceptionMessage;
        public QuizTimeLimitExceededException()
        {
            exceptionMessage = "Sorry Quiz Time Limit has been exceeded";
        }
        public override string Message => exceptionMessage;
    }
}
