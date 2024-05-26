using QuizApp.Models;
using QuizApp.Models.DTOs;

namespace QuizApp.Interfaces
{
    public interface IQuizServices
    {
        public Task<QuizReturnDTO> AddQuizAsync(QuizDTO quizDTO);
    }
}
