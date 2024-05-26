using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Repositories;

namespace QuizApp.Services
{
    public class QuizServices : IQuizServices
    {
        private readonly IRepository<int, Quiz> _quizRepo;
        private readonly IRepository<int, Question> _questionRepository;

        public QuizServices(IRepository<int, Quiz> quizRepo, IRepository<int, Question> questionRepository)
        {
            _quizRepo = quizRepo;
            _questionRepository = questionRepository;
        }

        public async Task<QuizReturnDTO> AddQuizAsync(QuizDTO quizDTO)
        {
            Quiz quiz = await MapQuizDTOToQuiz(quizDTO);
            var result = await _quizRepo.Add(quiz);
            return await MapQuizToQuizReturnDTO(result);
        }

        private async Task<QuizReturnDTO> MapQuizToQuizReturnDTO(Quiz quiz)
        {
            return new QuizReturnDTO
            {
                QuizName = quiz.QuizName,
                QuizDescription = quiz.QuizDescription,
                QuizType = quiz.QuizType,
                CreatedOn = quiz.CreatedOn,
                NumOfQuestions = quiz.NumOfQuestions,
                QuizCreatedBy = quiz.QuizCreatedBy,
                TotalPoints = quiz.TotalPoints,
                QuizQuestions = quiz.QuizQuestions.Select(q => q.QuestionId).ToList()
            };
        }

        private async Task<Quiz> MapQuizDTOToQuiz(QuizDTO quizDTO)
        {
            Quiz quiz = new Quiz()
            {
                QuizName = quizDTO.QuizName,
                QuizDescription = quizDTO.QuizDescription,
                QuizType = quizDTO.QuizType,
                CreatedOn = DateTime.Now,
                NumOfQuestions = quizDTO.QuestionIds.Count,
                QuizCreatedBy = quizDTO.QuizCreatedBy,
                QuizQuestions = new List<QuizQuestion>()
            };

            decimal totalPoints = 0;
            foreach (var questionId in quizDTO.QuestionIds)
            {
                var question = await _questionRepository.Get(questionId);
                if (question != null)
                {
                    totalPoints += question.Points;
                    quiz.QuizQuestions.Add(new QuizQuestion { QuizId = quiz.Id, QuestionId = questionId });
                }
                else
                {
                    throw new NoSuchQuestionException($"Question with ID {questionId} does not exist.");
                }
            }
            quiz.TotalPoints = totalPoints;
            return quiz;
        }
    }
}
