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

        public UnauthorizedUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public UnauthorizedUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}