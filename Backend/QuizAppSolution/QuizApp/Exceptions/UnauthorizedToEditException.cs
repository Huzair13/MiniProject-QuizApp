namespace QuizApp.Exceptions
{
    public class UnauthorizedToEditException : Exception
    {
        string ExceptionMessage;
        public UnauthorizedToEditException()
        {
            ExceptionMessage = "You cant edit this. Only Creator can Edit";
        }
        public override string Message => ExceptionMessage; 
    }
}
