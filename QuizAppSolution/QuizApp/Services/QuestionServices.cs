using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
using QuizApp.Repositories;
using System.Linq;

namespace QuizApp.Services
{
    public class QuestionServices : IQuestionServices
    {

        //REPOSITORIES
        private readonly IRepository<int, Question> _repository;
        private readonly IRepository<int, FillUps> _fillUpsRepo;
        private readonly IRepository<int, MultipleChoice> _multipleChoiceRepo;
        private readonly IRepository<int, Teacher> _teacherRepo;


        //INJECTING REPOSITORIES
        public QuestionServices(IRepository<int, Question> reposiroty, 
                                IRepository<int, FillUps> fillUpsRepo, 
                                IRepository<int, MultipleChoice> mcqRepo,
                                 IRepository<int,Teacher> teacherRepo)
        {
            _repository = reposiroty;
            _fillUpsRepo = fillUpsRepo;
            _multipleChoiceRepo = mcqRepo;
            _teacherRepo = teacherRepo;
        }

        //GET ALL FILL UPS QUESTIONS
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


        //GET ALL MCQ QUESTIONS
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

        //GET ALL QUESTIONS
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

        //MAP QUESTION TO QUESTION RETURN DTO
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

        //MAP QUESTION TO MCQ_RETURN_DTO
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

        //MAP FILL UPS TO FILLUPS_RETURN_DTO
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

        //ADD MCQ QUESTION
        public async Task<QuestionReturnDTO> AddMCQQuestion(MCQDTO mcq)
        {
            try
            {
                MultipleChoice multipleChoice = await MapMCQInputDTOToMCQ(mcq);
                var result = await _multipleChoiceRepo.Add(multipleChoice);

                Teacher teacher = await _teacherRepo.Get(multipleChoice.QuestionCreatedBy);

                var numOfQuesCreatedByTeacher = teacher.NumsOfQuestionsCreated;
                if (numOfQuesCreatedByTeacher == null)
                {
                    teacher.NumsOfQuestionsCreated = 1;
                }
                else
                {
                    teacher.NumsOfQuestionsCreated++;
                }
                await _teacherRepo.Update(teacher);

                var returnResult = await MapMCQToMCQReturnDTO(result);
                return returnResult;
            }
            catch(NoSuchUserException ex)
            {
                throw new NoSuchUserException(ex.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //ADD FILL UPS QUESTION
        public async Task<FillUpsReturnDTO> AddFillUpsQuestion(FillUpsDTO fillUps)
        {
            try
            {
                FillUps fillups = await MapFillUpsInputDTOToFillUps(fillUps);
                var result = await _fillUpsRepo.Add(fillups);

                Teacher teacher = await _teacherRepo.Get(fillups.QuestionCreatedBy);

                var numOfQuesCreatedByTeacher = teacher.NumsOfQuestionsCreated;
                if (numOfQuesCreatedByTeacher == null)
                {
                    teacher.NumsOfQuestionsCreated = 1;
                }
                else
                {
                    teacher.NumsOfQuestionsCreated++;
                }
                await _teacherRepo.Update(teacher);

                var returnResult = await MapFillUpsToFillUpsReturnDTO(result);
                return returnResult;
            }
            catch (NoSuchUserException ex)
            {
                throw new NoSuchUserException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<FillUpsReturnDTO> MapFillUpsToFillUpsReturnDTO(FillUps fillUps)
        {
            FillUpsReturnDTO fillUpsReturnDTO = new FillUpsReturnDTO();

            fillUpsReturnDTO.Id = fillUps.Id;
            fillUpsReturnDTO.Category = fillUps.Category;
            fillUpsReturnDTO.QuestionText = fillUps.QuestionText;
            fillUpsReturnDTO.QuestionCreatedBy = fillUps.QuestionCreatedBy;
            fillUpsReturnDTO.DifficultyLevel = fillUps.DifficultyLevel;
            fillUpsReturnDTO.CreatedDate = fillUps.CreatedDate;
            fillUpsReturnDTO.Points = fillUps.Points;
            fillUpsReturnDTO.QuestionType = fillUps.QuestionType;
            fillUpsReturnDTO.CorrectAnswer = fillUps.CorrectAnswer;
            return fillUpsReturnDTO;
        }

        private async Task<FillUps> MapFillUpsInputDTOToFillUps(FillUpsDTO fillUpsDTO)
        {
            FillUps fillUps = new FillUps();
            fillUps.QuestionText = fillUpsDTO.QuestionText;
            fillUps.DifficultyLevel = fillUpsDTO.DifficultyLevel;
            fillUps.Category = fillUpsDTO.Category;
            fillUps.Points = fillUpsDTO.Points;
            fillUps.CorrectAnswer = fillUpsDTO.CorrectAnswer;
            fillUps.CreatedDate = fillUpsDTO.CreatedDate;
            fillUps.QuestionCreatedBy = fillUpsDTO.CreatedBy;

            return fillUps;
        }

        //MAP MCQ TO MCQ_RETURN_DTO
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

        //MAP MCQ_INPUT_DTO TO MCQ
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

        //EDIT FILL UPS BY ID
        public async Task<FillUpsReturnDTO> EditFillUpsQuestionById(FillUpsUpdateDTO fillUpsUpdateDTO, int userId)
        {
            try
            {
                FillUps fillUps = await _fillUpsRepo.Get(fillUpsUpdateDTO.QuestionId);
                if (userId != fillUps.QuestionCreatedBy)
                {
                    throw new UnauthorizedToEditException();
                }

                if (fillUpsUpdateDTO.QuestionText != null)
                {
                    fillUps.QuestionText = fillUpsUpdateDTO.QuestionText;
                }

                if (fillUpsUpdateDTO.Points.HasValue)
                {
                    fillUps.Points = fillUpsUpdateDTO.Points.Value;
                }

                if (fillUpsUpdateDTO.Category != null)
                {
                    fillUps.Category = fillUpsUpdateDTO.Category;
                }

                if (fillUpsUpdateDTO.DifficultyLevel.HasValue)
                {
                    fillUps.DifficultyLevel = fillUpsUpdateDTO.DifficultyLevel.Value;
                }

                if (fillUpsUpdateDTO.CorrectAnswer != null)
                {
                    fillUps.CorrectAnswer = fillUpsUpdateDTO.CorrectAnswer;
                }

                //UPDATE 
                await _fillUpsRepo.Update(fillUps);

                return await MapFillUpsToFillUpsReturnDTO(fillUps);


            }
            catch(NoSuchQuestionException e)
            {
                throw new NoSuchQuestionException(e.Message);
            }
            catch(UnauthorizedToEditException ex)
            {
                throw ex ;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }


        //EDIT MCQ BY QUESTIONID
        public async Task<QuestionReturnDTO> EditMCQByQuestionID(MCQUpdateDTO mCQUpdateDTO,int userId)
        {
            try
            {
                MultipleChoice mcq = await _multipleChoiceRepo.Get(mCQUpdateDTO.QuestionId);
                if(userId != mcq.QuestionCreatedBy)
                {
                    throw new UnauthorizedToEditException();
                }

                if (mCQUpdateDTO.QuestionText != null)
                {
                    mcq.QuestionText = mCQUpdateDTO.QuestionText;
                }

                if (mCQUpdateDTO.Points.HasValue)
                {
                    mcq.Points = mCQUpdateDTO.Points.Value;
                }

                if (mCQUpdateDTO.Category != null)
                {
                    mcq.Category = mCQUpdateDTO.Category;
                }

                if (mCQUpdateDTO.DifficultyLevel.HasValue)
                {
                    mcq.DifficultyLevel = mCQUpdateDTO.DifficultyLevel.Value;
                }
                if(mCQUpdateDTO.Choice1 != null)
                {
                    mcq.Choice1 = mCQUpdateDTO.Choice1;
                }
                if (mCQUpdateDTO.Choice2 != null)
                {
                    mcq.Choice2 = mCQUpdateDTO.Choice2;
                }
                if (mCQUpdateDTO.Choice3 != null)
                {
                    mcq.Choice3 = mCQUpdateDTO.Choice3;
                }
                if (mCQUpdateDTO.Choice4 != null)
                {
                    mcq.Choice4 = mCQUpdateDTO.Choice4;
                }

                if (mCQUpdateDTO.CorrectAnswer != null)
                {
                    mcq.CorrectChoice = mCQUpdateDTO.CorrectAnswer;
                }

                //UPDATE 
                await _multipleChoiceRepo.Update(mcq);

                return await MapMCQToMCQReturnDTO(mcq);


            }
            catch (NoSuchQuestionException e)
            {
                throw new NoSuchQuestionException(e.Message);
            }
            catch(UnauthorizedToEditException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        //EDIT QUESTION BY QUESTION ID
        public async Task<QuestionReturnDTO> EditQuestionByID(UpdateQuestionDTO updateQuestionDTO,int userId)
        {
            Question question = await _repository.Get(updateQuestionDTO.Id);
            if (question is MultipleChoice )
            {
                MCQUpdateDTO mCQUpdateDTO = new MCQUpdateDTO()
                {
                    QuestionId = updateQuestionDTO.Id,
                    QuestionText = updateQuestionDTO.QuestionText,
                    Points = updateQuestionDTO.Points,
                    Category = updateQuestionDTO.Category,
                    DifficultyLevel = updateQuestionDTO.DifficultyLevel,
                    Choice1 = updateQuestionDTO.Choice1,
                    Choice2 = updateQuestionDTO.Choice2,
                    Choice3 = updateQuestionDTO.Choice3,
                    Choice4 = updateQuestionDTO.Choice4,
                    CorrectAnswer = updateQuestionDTO.CorrectAnswer
                };
                var result = await EditMCQByQuestionID(mCQUpdateDTO, userId);
                return result;
            }
            else 
            {
                FillUpsUpdateDTO fillUpsUpdateDTO = new FillUpsUpdateDTO()
                {
                    QuestionId = updateQuestionDTO.Id,
                    QuestionText = updateQuestionDTO.QuestionText,
                    Points = updateQuestionDTO.Points,
                    Category = updateQuestionDTO.Category,
                    DifficultyLevel = updateQuestionDTO.DifficultyLevel,
                    CorrectAnswer = updateQuestionDTO.CorrectAnswer
                };
                var result = await EditFillUpsQuestionById(fillUpsUpdateDTO, userId);

                QuestionReturnDTO questionReturnDTO = new QuestionReturnDTO()
                {
                    Id = result.Id,
                    QuestionText = result.QuestionText,
                    Points = result.Points,
                    Category = result.Category,
                    DifficultyLevel = result.DifficultyLevel,
                    CorrectAnswer = result.CorrectAnswer,
                    CreatedDate = result.CreatedDate,
                    QuestionCreatedBy = result.QuestionCreatedBy,
                    QuestionType = result.QuestionType
                };
                return questionReturnDTO;
            }
        }

        public async Task<QuestionDTO> DeleteQuestionByID(int QuestionID,int userId)
        {
            try
            {
                var question = await _repository.Get(QuestionID);
                if(question.QuestionCreatedBy== userId)
                {
                    var deletedQuestion = await _repository.Delete(QuestionID);

                    QuestionDTO questionDTO = await MapQuestionToQuestionDTO(deletedQuestion);
                    return questionDTO;
                }
                else
                {
                    throw new UnauthorizedToDeleteException();
                }
                

            }
            catch(NoSuchQuestionException ex)
            {
                throw new NoSuchQuestionException(ex.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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
    }
}
