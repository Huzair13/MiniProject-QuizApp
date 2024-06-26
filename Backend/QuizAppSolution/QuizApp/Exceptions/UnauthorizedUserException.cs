using System.Runtime.Serialization;

namespace QuizApp.Exceptions
{
    [Serializable]
    public class UnauthorizedUserException : Exception
    {
        public UnauthorizedUserException()
        {
        }

        public UnauthorizedUserException(string? message) : base(message)
        {
        }
    }
}