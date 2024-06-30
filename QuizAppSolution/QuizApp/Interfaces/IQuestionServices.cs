using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;

namespace QuizApp.Interfaces
{
    public interface IQuestionServices
    {

        public Task<QuestionReturnDTO> AddMCQQuestion(MCQDTO mcq);
        public Task<FillUpsReturnDTO> AddFillUpsQuestion(FillUpsDTO fillUps);

        public Task<FillUpsReturnDTO> EditFillUpsQuestionById(FillUpsUpdateDTO fillUpsUpdateDTO,int userId);
        public Task<QuestionReturnDTO> EditMCQByQuestionID(MCQUpdateDTO mCQUpdateDTO,int userId);
        public Task<QuestionReturnDTO> EditQuestionByID(UpdateQuestionDTO updateQuestionDTO,int userId);
        public Task<QuestionDTO> DeleteQuestionByID(int QuestionID,int userId);
        public Task<QuestionDTO> SoftDeleteQuestionByIDAsync(int quizID, int userId);
        public Task<QuestionDTO> UndoSoftDeleteQuestionByIDAsync(int questionID, int userId);


    }
}
