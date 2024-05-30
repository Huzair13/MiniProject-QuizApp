using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;

namespace QuizApp.Interfaces
{
    public interface IQuestionViewServices
    {
        public Task<IEnumerable<QuestionReturnDTO>> GetAllQuestionsAsync();
        public Task<IEnumerable<QuestionReturnDTO>> GetAllMCQQuestionsAsync();
        public Task<IEnumerable<FillUpsReturnDTO>> GetAllFillUpsQuestionsAsync();
        public Task<List<QuestionDTO>> GetAllSoftDeletedQuestionsAsync();
        public Task<List<QuestionDTO>> GetAllQuestionsCreatedByLoggedInTeacherAsync(int userId);
        public Task<IEnumerable<QuestionDTO>> GetAllHardQuestions ();
        public Task<IEnumerable<QuestionDTO>> GetAllMediumQuestions();
        public Task<IEnumerable<QuestionDTO>> GetAllEasyQuestions();
    }
}
