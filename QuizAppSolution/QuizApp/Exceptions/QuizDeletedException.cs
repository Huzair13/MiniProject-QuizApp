namespace QuizApp.Exceptions
{
    public class QuizDeletedException : Exception
    {
        string ExceptionMessage;
        public QuizDeletedException() 
        {
            ExceptionMessage = "Given Quiz Has been Deleted by You already";
        }
        public override string Message => ExceptionMessage; 
    }
}
