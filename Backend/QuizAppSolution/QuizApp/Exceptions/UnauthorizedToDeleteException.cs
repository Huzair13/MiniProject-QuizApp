namespace QuizApp.Exceptions
{
    public class UnauthorizedToDeleteException :Exception
    {
        string ExceptionMessage;
        public UnauthorizedToDeleteException()
        {
            ExceptionMessage = "You cant delete this. Only Creator can Delete";
        }
        public override string Message => ExceptionMessage;
    }
}
