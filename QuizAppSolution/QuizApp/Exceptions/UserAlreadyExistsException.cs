namespace QuizApp.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        string exceptionMessage;
        public UserAlreadyExistsException()
        {
            exceptionMessage = "User with the same EmailID or Mobile Number is already Registered. Please try to login";
        }
        public override string Message => exceptionMessage;
    }
}
