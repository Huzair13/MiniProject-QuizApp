using QuizApp.Models;
using QuizApp.Models.DTOs.QuizDTOs;

namespace QuizApp.Interfaces
{
    public interface IQuizServices
    {
        public Task<QuizReturnDTO> AddQuizAsync(QuizDTO quizDTO);
        public Task<QuizReturnDTO> EditQuizByIDAsync(QuizUpdateDTO quizDTO,int userId);
        public Task<QuizReturnDTO> DeleteQuizByIDAsync(int quizID,int userId);
        public Task<List<QuizReturnDTO>> GetAllQuizzesAsync();
        public Task<List<QuizReturnDTO>> GetAllQuizzesCreatedByLoggedInTeacherAsync(int userId);
        public Task<List<QuizReturnDTO>> GetAllSoftDeletedQuizzesAsync();
        public Task<QuizReturnDTO> SoftDeleteQuizByIDAsync(int quizID, int userId);
        public Task<QuizReturnDTO> UndoSoftDeleteQuizByIDAsync(int quizID, int userId);
        public Task<QuizReturnDTO> CreateQuizFromExistingQuiz(int quizID, int userId);
        public Task<QuizReturnDTO> AddQuestionsToQuizAsync(AddQuestionsToQuizDTO inputDTO, int userID);

    }
}
