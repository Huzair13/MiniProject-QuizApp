using QuizApp.Models;
using QuizApp.Models.DTOs;

namespace QuizApp.Interfaces
{
    public interface IQuestionServices
    {
        public Task<IEnumerable<QuestionReturnDTO>> GetAllQuestionsAsync();
        public Task<IEnumerable<QuestionReturnDTO>> GetAllMCQQuestionsAsync();
        public Task<IEnumerable<FillUpsReturnDTO>> GetAllFillUpsQuestionsAsync();
        public Task<QuestionReturnDTO> AddMCQQuestion(MCQDTO mcq);
        public Task<FillUpsReturnDTO> AddFillUpsQuestion(FillUps fillUps);

    }
}
