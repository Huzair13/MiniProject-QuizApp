using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models.DTOs.ResponseDTO;

namespace QuizApp.Interfaces
{
    public interface IQuizResponseServices
    {
        public Task<StartQuizResponseDTO> StartQuizAsync(int userId, int quizId);
        public Task<string> SubmitAnswerAsync(SubmitAnswerDTO submitAnswerDTO);
        public Task<string> SubmitAllAnswersAsync(SubmitAllAnswersDTO submitAllAnswersDTO);
        public Task<IList<QuizResultDTO>> GetQuizResultAsync(int userId, int quizId);
        public Task<List<LeaderboardDTO>> GetQuizLeaderboardAsync(int quizId);
        public Task<int> GetStudentPositionInLeaderboardAsync(int userId, int quizId);
        public Task<List<LeaderboardDTO>> GetStudentQuizLeaderboardAsync(int quizId);
        public Task<List<QuizResponseDTO>> GetAllUseresponsesAsync(int userId);
    }
}
