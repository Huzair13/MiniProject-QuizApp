using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Repositories;
using System.Linq;

namespace QuizApp.Services
{
    public class QuestionServices : IQuestionServices
    {
        private readonly IRepository<int, Question> _repository;
        private readonly IRepository<int, FillUps> _fillUpsRepo;
        private readonly IRepository<int, MultipleChoice> _multipleChoiceRepo;

        public QuestionServices(IRepository<int, Question> reposiroty, 
                                IRepository<int, FillUps> fillUpsRepo, 
                                IRepository<int, MultipleChoice> mcqRepo)
        {
            _repository = reposiroty;
            _fillUpsRepo = fillUpsRepo;
            _multipleChoiceRepo = mcqRepo;
        }

        public async Task<IEnumerable<FillUpsReturnDTO>> GetAllFillUpsQuestionsAsync()
        {
            try
            {
                var questions = await _repository.Get();

                IEnumerable<FillUpsReturnDTO> fillUpsReturnDTO = await MapFillUpsToFillUpsReturnDTO(questions);
                return fillUpsReturnDTO;
            }
            catch (NoSuchQuestionException e)
            {
                throw new NoSuchQuestionException(e.Message);
            }
        }

        public async Task<IEnumerable<QuestionReturnDTO>> GetAllMCQQuestionsAsync()
        {
            try
            {
                var questions = await _repository.Get();

                IEnumerable<QuestionReturnDTO> returnRequestDTO =await MapQuestionToMCQReturnDTO(questions);
                return returnRequestDTO;
            }
            catch (NoSuchQuestionException e)
            {
                throw new NoSuchQuestionException(e.Message);
            }
        }

        public async Task<IEnumerable<QuestionReturnDTO>> GetAllQuestionsAsync()
        {
            try
            {
                var questions = await _repository.Get();

                IEnumerable<QuestionReturnDTO> returnRequestDTO = await MapQuestionToQuestionReturnDTO(questions);
                return returnRequestDTO;
            }
            catch(NoSuchQuestionException e)
            {
                throw new NoSuchQuestionException(e.Message);
            }
        }

        private async Task<IEnumerable<QuestionReturnDTO>> MapQuestionToQuestionReturnDTO(IEnumerable<Question> questions)
        {
            IEnumerable<QuestionReturnDTO> questionReturnDTOs = new List<QuestionReturnDTO>();

            foreach (var item in questions)
            {
                QuestionReturnDTO questionReturnDTO = new QuestionReturnDTO
                {
                    Id = item.Id,
                    Category = item.Category,
                    QuestionText = item.QuestionText,
                    QuestionCreatedBy = item.QuestionCreatedBy,
                    DifficultyLevel = item.DifficultyLevel,
                    CreatedDate = item.CreatedDate,
                    Points = item.Points,
                    QuestionType = item.QuestionType,
                };
                if(item is MultipleChoice mcq)
                {
                    questionReturnDTO.Choice1 = mcq.Choice1;
                    questionReturnDTO.Choice2 = mcq.Choice2;
                    questionReturnDTO.Choice3 = mcq.Choice3;
                    questionReturnDTO.Choice4 = mcq.Choice4;
                    questionReturnDTO.CorrectAnswer = mcq.CorrectChoice;
                }
                else if(item is FillUps fillUps)
                {
                    questionReturnDTO.CorrectAnswer = fillUps.CorrectAnswer;
                }

                questionReturnDTOs = questionReturnDTOs.Concat(new[] { questionReturnDTO });
            }
            return questionReturnDTOs;
        }

        private async Task<IEnumerable<QuestionReturnDTO>> MapQuestionToMCQReturnDTO(IEnumerable<Question> questions)
        {
            IEnumerable<QuestionReturnDTO> questionReturnDTOs = new List<QuestionReturnDTO>();

            foreach (var item in questions)
            {
                if (item is MultipleChoice mcq)
                {
                    QuestionReturnDTO questionReturnDTO = new QuestionReturnDTO();


                    questionReturnDTO.Id = item.Id;
                    questionReturnDTO.Category = item.Category;
                    questionReturnDTO.QuestionText = item.QuestionText;
                    questionReturnDTO.QuestionCreatedBy = item.QuestionCreatedBy;
                    questionReturnDTO.DifficultyLevel = item.DifficultyLevel;
                    questionReturnDTO.CreatedDate = item.CreatedDate;
                    questionReturnDTO.Points = item.Points;
                    questionReturnDTO.QuestionType = item.QuestionType;
                    questionReturnDTO.Choice1 = mcq.Choice1;
                    questionReturnDTO.Choice2 = mcq.Choice2;
                    questionReturnDTO.Choice3 = mcq.Choice3;
                    questionReturnDTO.Choice4 = mcq.Choice4;
                    questionReturnDTO.CorrectAnswer = mcq.CorrectChoice;

                    questionReturnDTOs = questionReturnDTOs.Concat(new[] { questionReturnDTO });
                }
            }
            return questionReturnDTOs;
        }


        private async Task<IEnumerable<FillUpsReturnDTO>> MapFillUpsToFillUpsReturnDTO(IEnumerable<Question> questions)
        {
            IEnumerable<FillUpsReturnDTO> questionReturnDTOs = new List<FillUpsReturnDTO>();

            foreach (var item in questions)
            {
                if (item is FillUps fillUps)
                {
                    FillUpsReturnDTO questionReturnDTO = new FillUpsReturnDTO();

                    questionReturnDTO.Id = item.Id;
                    questionReturnDTO.Category = item.Category;
                    questionReturnDTO.QuestionText = item.QuestionText;
                    questionReturnDTO.QuestionCreatedBy = item.QuestionCreatedBy;
                    questionReturnDTO.DifficultyLevel = item.DifficultyLevel;
                    questionReturnDTO.CreatedDate = item.CreatedDate;
                    questionReturnDTO.Points = item.Points;
                    questionReturnDTO.QuestionType = item.QuestionType;
                    questionReturnDTO.CorrectAnswer = fillUps.CorrectAnswer;

                    questionReturnDTOs = questionReturnDTOs.Concat(new[] { questionReturnDTO });
                }
            }
            return questionReturnDTOs;
        }

        public async Task<QuestionReturnDTO> AddMCQQuestion(MCQDTO mcq)
        {
            MultipleChoice multipleChoice = await MapMCQInputDTOToMCQ(mcq);
            var result = await _multipleChoiceRepo.Add(multipleChoice);

            var returnResult = await MapMCQToMCQReturnDTO(result);
            return returnResult;
        }

        private async Task<QuestionReturnDTO> MapMCQToMCQReturnDTO(MultipleChoice item)
        {
            QuestionReturnDTO questionReturnDTO = new QuestionReturnDTO();

            questionReturnDTO.Id = item.Id;
            questionReturnDTO.Category = item.Category;
            questionReturnDTO.QuestionText = item.QuestionText;
            questionReturnDTO.QuestionCreatedBy = item.QuestionCreatedBy;
            questionReturnDTO.DifficultyLevel = item.DifficultyLevel;
            questionReturnDTO.CreatedDate = item.CreatedDate;
            questionReturnDTO.Points = item.Points;
            questionReturnDTO.QuestionType = item.QuestionType;
            questionReturnDTO.Choice1 = item.Choice1;
            questionReturnDTO.Choice2 = item.Choice2;
            questionReturnDTO.Choice3 = item.Choice3;
            questionReturnDTO.Choice4 = item.Choice4;
            questionReturnDTO.CorrectAnswer = item.CorrectChoice;
            return questionReturnDTO;
        }

        private async Task<MultipleChoice> MapMCQInputDTOToMCQ(MCQDTO mcq)
        {
            MultipleChoice multipleChoice = new MultipleChoice();
            multipleChoice.QuestionText = mcq.QuestionText;
            multipleChoice.DifficultyLevel = mcq.DifficultyLevel;
            multipleChoice.Category = mcq.Category;
            multipleChoice.Points = mcq.Points;
            multipleChoice.Choice1 = mcq.Choice1;
            multipleChoice.Choice2 = mcq.Choice2;
            multipleChoice.Choice3 = mcq.Choice3;
            multipleChoice.Choice4 = mcq.Choice4;
            multipleChoice.CorrectChoice = mcq.CorrectAnswer;
            multipleChoice.CreatedDate = mcq.CreatedDate;
            multipleChoice.QuestionCreatedBy = mcq.CreatedBy;

            return multipleChoice;

        }

        public Task<FillUpsReturnDTO> AddFillUpsQuestion(FillUps fillUps)
        {
            throw new NotImplementedException();
        }
    }
}
