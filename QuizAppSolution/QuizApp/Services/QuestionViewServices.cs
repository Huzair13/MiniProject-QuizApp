using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
using QuizApp.Models.DTOs.QuizDTOs;

namespace QuizApp.Services
{
    public class QuestionViewServices : IQuestionViewServices
    {

        //REPOSITORIES
        private readonly IRepository<int, Question> _repository;


        //INJECTING REPOSITORIES
        public QuestionViewServices(IRepository<int, Question> reposiroty)
        {
            _repository = reposiroty;
        }

        //GET ALL FILL UPS QUESTIONS
        public async Task<IEnumerable<FillUpsReturnDTO>> GetAllFillUpsQuestionsAsync()
        {
            try
            {
                var questions = await _repository.Get();
                if (questions.Count() == 0)
                {
                    throw new NoSuchQuestionException();
                }

                IEnumerable<FillUpsReturnDTO> questionReturnDTOs = new List<FillUpsReturnDTO>();


                foreach (var item in questions)
                {
                    if (!item.IsDeleted && item is FillUps fillUps)
                    {
                        FillUpsReturnDTO questionReturnDTO = await MapFillUpsToFillUpsReturnDTO(item);
                        questionReturnDTO.CorrectAnswer = fillUps.CorrectAnswer;

                        questionReturnDTOs = questionReturnDTOs.Concat(new[] { questionReturnDTO });
                    }
                }
                return questionReturnDTOs;

            }
            catch (NoSuchQuestionException e)
            {
                throw new NoSuchQuestionException(e.Message);
            }
        }


        //GET ALL MCQ QUESTIONS
        public async Task<IEnumerable<QuestionReturnDTO>> GetAllMCQQuestionsAsync()
        {
            try
            {
                var questions = await _repository.Get();
                if (questions.Count() == 0)
                {
                    throw new NoSuchQuestionException();
                }

                IEnumerable<QuestionReturnDTO> questionReturnDTOs = new List<QuestionReturnDTO>();

                foreach (var item in questions)
                {
                    if (!item.IsDeleted && item is MultipleChoice mcq)
                    {
                        QuestionReturnDTO questionReturnDTO = await MapQuestionToMCQReturnDTO(item);
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
            catch (NoSuchQuestionException e)
            {
                throw new NoSuchQuestionException(e.Message);
            }
        }

        //GET ALL QUESTIONS
        public async Task<IEnumerable<QuestionReturnDTO>> GetAllQuestionsAsync()
        {
            try
            {
                var questions = await _repository.Get();
                if(questions.Count() == 0)
                {
                    throw new NoSuchQuestionException();
                }

                IEnumerable<QuestionReturnDTO> questionReturnDTOs = new List<QuestionReturnDTO>();

                foreach (var item in questions)
                {
                    if (!item.IsDeleted)
                    {
                        QuestionReturnDTO questionReturnDTO = await MapQuestionToQuestionReturnDTO(item);
                        questionReturnDTOs = questionReturnDTOs.Concat(new[] { questionReturnDTO });
                    }
                }
                return questionReturnDTOs;
            }
            catch (NoSuchQuestionException e)
            {
                throw new NoSuchQuestionException(e.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //GET ALL SOFT DELETED QUESTIONS
        public async Task<List<QuestionDTO>> GetAllSoftDeletedQuestionsAsync()
        {
            try
            {
                var AllQuestions = await _repository.Get();
                var questions = AllQuestions.Where(q => q.IsDeleted == true).ToList();
                if(questions.Count() == 0)
                {
                    throw new NoSuchQuestionException();
                }
                List<QuestionDTO> questionDTOs = new List<QuestionDTO>();

                foreach (var question in questions)
                {
                    var questionDTO = await MapQuestionToQuestionDTO(question);
                    questionDTOs.Add(questionDTO);
                }

                return questionDTOs;
            }
            catch (NoSuchQuestionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GET ALL QUESTIONS CREATED BY THE PARTICULAR TEACHER
        public async Task<List<QuestionDTO>> GetAllQuestionsCreatedByLoggedInTeacherAsync(int userId)
        {
            try
            {
                var AllQuestions = await _repository.Get();
                var questions = AllQuestions.Where(q => q.QuestionCreatedBy == userId).ToList();
                if(questions.Count() == 0)
                {
                    throw new NoSuchQuestionException();
                }
                List<QuestionDTO> questionDTOs = new List<QuestionDTO>();

                foreach (var question in questions)
                {
                    var quesDTO = await MapQuestionToQuestionDTO(question);
                    questionDTOs.Add(quesDTO);
                }

                return questionDTOs;
            }
            catch (NoSuchQuestionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GET ALL HARD QUESTIONS
        public async Task<IEnumerable<QuestionDTO>> GetAllHardQuestions()
        {
            try
            {
                var AllQuestions = await _repository.Get();
                var questions = AllQuestions.Where(q => q.DifficultyLevel == DifficultyLevel.Hard).ToList();
                if (questions.Count() == 0)
                {
                    throw new NoSuchQuestionException();
                }
                List<QuestionDTO> questionDTOs = new List<QuestionDTO>();

                foreach (var question in questions)
                {
                    var quesDTO = await MapQuestionToQuestionDTO(question);
                    questionDTOs.Add(quesDTO);
                }
                return questionDTOs;
            }
            catch (NoSuchQuestionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GET ALL MEDIUM QUESTIONS
        public async Task<IEnumerable<QuestionDTO>> GetAllMediumQuestions()
        {
            try
            {
                var AllQuestions = await _repository.Get();
                var questions = AllQuestions.Where(q => q.DifficultyLevel == DifficultyLevel.Medium).ToList();
                if (questions.Count() == 0)
                {
                    throw new NoSuchQuestionException();
                }
                List<QuestionDTO> questionDTOs = new List<QuestionDTO>();

                foreach (var question in questions)
                {
                    var quesDTO = await MapQuestionToQuestionDTO(question);
                    questionDTOs.Add(quesDTO);
                }
                return questionDTOs;
            }
            catch (NoSuchQuestionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GET ALL EASY QUESTIONS
        public async Task<IEnumerable<QuestionDTO>> GetAllEasyQuestions()
        {
            try
            {
                var AllQuestions = await _repository.Get();
                var questions = AllQuestions.Where(q => q.DifficultyLevel == DifficultyLevel.Easy).ToList();
                if (questions.Count() == 0)
                {
                    throw new NoSuchQuestionException();
                }
                List<QuestionDTO> questionDTOs = new List<QuestionDTO>();

                foreach (var question in questions)
                {
                    var quesDTO = await MapQuestionToQuestionDTO(question);
                    questionDTOs.Add(quesDTO);
                }
                return questionDTOs;
            }
            catch (NoSuchQuestionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //MAP QUESTION TO QUESTION DTO
        private async Task<QuestionDTO> MapQuestionToQuestionDTO(Question question)
        {
            return new QuestionDTO()
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                Points = question.Points,
                Category = question.Category,
                DifficultyLevel = question.DifficultyLevel,
                CreatedDate = question.CreatedDate,
                QuestionCreatedBy = question.QuestionCreatedBy,
                QuestionType = question.QuestionType
            };
        }

        //MAP FILL UPS TO FILLUPS_RETURN_DTO
        private async Task<FillUpsReturnDTO> MapFillUpsToFillUpsReturnDTO(Question item)
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
           
            return questionReturnDTO;
        }

        //MAP QUESTION TO QUESTION RETURN DTO
        private async Task<QuestionReturnDTO> MapQuestionToQuestionReturnDTO(Question item)
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
            if (item is MultipleChoice mcq)
            {
                questionReturnDTO.Choice1 = mcq.Choice1;
                questionReturnDTO.Choice2 = mcq.Choice2;
                questionReturnDTO.Choice3 = mcq.Choice3;
                questionReturnDTO.Choice4 = mcq.Choice4;
                questionReturnDTO.CorrectAnswer = mcq.CorrectChoice;
            }
            else if (item is FillUps fillUps)
            {
                questionReturnDTO.CorrectAnswer = fillUps.CorrectAnswer;
            }
            return questionReturnDTO;
        }

        //MAP QUESTION TO MCQ_RETURN_DTO
        private async Task<QuestionReturnDTO> MapQuestionToMCQReturnDTO(Question item)
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

            return questionReturnDTO;
        }

        
    }
}
