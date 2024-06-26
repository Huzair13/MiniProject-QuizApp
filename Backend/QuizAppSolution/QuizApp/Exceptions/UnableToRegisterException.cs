using System.Runtime.Serialization;

namespace QuizApp.Exceptions
{
    [Serializable]
    internal class UnableToRegisterException : Exception
    {
        public UnableToRegisterException()
        {
        }

        public UnableToRegisterException(string? message) : base(message)
        {
        }
    }
}