using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Repositories;

namespace QuizApp.Services
{
    public class QuizServices : IQuizServices
    {

        //REPOSITORIES
        private readonly IRepository<int, Quiz> _quizRepo;
        private readonly IRepository<int, Question> _questionRepository;
        private readonly IRepository<int, Teacher> _teacherRepo;
        private readonly IRepository<int,User> _userRepo;


        //INJECTING REPOSITORIES
        public QuizServices(IRepository<int, Quiz> quizRepo, IRepository<int, Question> questionRepository
            , IRepository<int, Teacher> teacherRepo, IRepository<int, User> userRepo)
        {
            _quizRepo = quizRepo;
            _questionRepository = questionRepository;
            _teacherRepo = teacherRepo;
            _userRepo = userRepo;
        }

        //ADD QUIZ 
        public async Task<QuizReturnDTO> AddQuizAsync(QuizDTO quizDTO)
        {
            try
            {
                Quiz quiz = await MapQuizDTOToQuiz(quizDTO);
                var result = await _quizRepo.Add(quiz);

                User user = await _userRepo.Get(quizDTO.QuizCreatedBy);
                if (user is Teacher teacher)
                {
                    teacher = await _teacherRepo.Get(quizDTO.QuizCreatedBy);
                    int numOfQuizAttended = teacher.NumsOfQuizCreated ?? 0;
                    if (numOfQuizAttended == 0)
                    {
                        teacher.NumsOfQuizCreated = 1;
                    }
                    else
                    {
                        teacher.NumsOfQuizCreated += 1;
                    }
                    await _teacherRepo.Update(teacher);
                }

                return await MapQuizToQuizReturnDTO(result);
            }
            catch(NoSuchUserException ex)
            {
                throw new NoSuchUserException(ex.Message);
            }
            catch(Exception ex) {
                throw new Exception(ex.Message);
            }
            
        }

        //MAP QUIZ TO QUIZ_RETURN_DTO
        private async Task<QuizReturnDTO> MapQuizToQuizReturnDTO(Quiz quiz)
        {
            return new QuizReturnDTO
            {
                QuizId = quiz.Id,
                QuizName = quiz.QuizName,
                QuizDescription = quiz.QuizDescription,
                QuizType = quiz.QuizType,
                CreatedOn = quiz.CreatedOn,
                NumOfQuestions = quiz.NumOfQuestions,
                QuizCreatedBy = quiz.QuizCreatedBy,
                TotalPoints = quiz.TotalPoints,
                IsMultpleAllowed = quiz.IsMultipleAttemptAllowed,
                Timelimit = quiz.TimeLimit,
                QuizQuestions = quiz.QuizQuestions.Select(qq => qq.QuestionId).ToList()
            };
        }


        //MAP QUIZ_DTO TO QUIZ
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
                IsMultipleAttemptAllowed = quizDTO.IsMultpleAttemptAllowed,
                QuizQuestions = new List<QuizQuestion>(),
                TimeLimit = quizDTO.TimeLimit
            };

            decimal totalPoints = 0;
            foreach (var questionId in quizDTO.QuestionIds)
            {
                var question = await _questionRepository.Get(questionId);
                if (question != null && !question.IsDeleted)
                {
                    totalPoints += question.Points;
                    quiz.QuizQuestions.Add(new QuizQuestion { QuizId = quiz.Id, QuestionId = questionId });
                }
            }
            quiz.TotalPoints = totalPoints;
            return quiz;
        }

        //EDIT QUIZ BY QUIZ ID
        public async Task<QuizReturnDTO> EditQuizByIDAsync(QuizUpdateDTO quizDTO,int userId)
        {
            try
            {
                Quiz quiz = await _quizRepo.Get(quizDTO.QuizID);
                if(quiz.QuizCreatedBy == userId)
                {
                    if (quiz.IsDeleted)
                    {
                        throw new QuizDeletedException();
                    }

                    quiz.QuizName = quizDTO.QuizName ?? quiz.QuizName;
                    quiz.QuizDescription = quizDTO.QuizDescription ?? quiz.QuizDescription;
                    quiz.QuizType = quizDTO.QuizType ?? quiz.QuizType;
                    quiz.IsMultipleAttemptAllowed = quizDTO.IsMultipleAttemptAllowed ?? quiz.IsMultipleAttemptAllowed;
                    quiz.TimeLimit = quizDTO.TimeLimit ?? quiz.TimeLimit;

                    if (quizDTO.QuestionIds != null)
                    {
                        quiz.QuizQuestions.Clear();
                        decimal totalPoints = 0;

                        foreach (var questionId in quizDTO.QuestionIds)
                        {
                            var question = await _questionRepository.Get(questionId);
                            if (!question.IsDeleted)
                            {
                                totalPoints += question.Points;
                                quiz.QuizQuestions.Add(new QuizQuestion { QuizId = quiz.Id, QuestionId = questionId });
                            }
                            else
                            {
                                throw new NoSuchQuestionException(questionId);
                            }
                        }

                        quiz.NumOfQuestions = quizDTO.QuestionIds.Count;
                        quiz.TotalPoints = totalPoints;
                    }

                    await _quizRepo.Update(quiz);
                    return await MapQuizToQuizReturnDTO(quiz);
                }
                else
                {
                    throw new UnauthorizedToEditException();
                }
                
            }
            catch (UnauthorizedToEditException ex)
            {
                throw new UnauthorizedToEditException();
            }
            catch(QuizDeletedException ex)
            {
                throw new QuizDeletedException();
            }
            catch (NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch(NoSuchQuestionException ex)
            {
                throw new NoSuchQuestionException(ex.Message);
            }
            
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // DELETE QUIZ BY ID - HARD DELETE
        public async Task<QuizReturnDTO> DeleteQuizByIDAsync(int quizID,int userId)
        {
            try
            {
                Quiz quiz = await _quizRepo.Get(quizID);

                if(quiz.QuizCreatedBy == userId)
                {
                    var result = await _quizRepo.Delete(quiz.Id);

                    User user = await _userRepo.Get(quiz.QuizCreatedBy);
                    if (user is Teacher teacher)
                    {
                        teacher = await _teacherRepo.Get(quiz.QuizCreatedBy);
                        if (teacher.NumsOfQuizCreated != null && teacher.NumsOfQuizCreated > 0)
                        {
                            teacher.NumsOfQuizCreated -= 1;
                            await _teacherRepo.Update(teacher);
                        }
                    }
                    return await MapQuizToQuizReturnDTO(result);
                }
                else
                {
                    throw new UnauthorizedToDeleteException();
                }

            }
            catch (NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch(UnauthorizedToDeleteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // DELETE QUIZ BY ID - SOFTDELETE
        public async Task<QuizReturnDTO> SoftDeleteQuizByIDAsync(int quizID, int userId)
        {
            try
            {
                Quiz quiz = await _quizRepo.Get(quizID);

                if (quiz.QuizCreatedBy == userId)
                {
                    quiz.IsDeleted= true;
                    var result = await _quizRepo.Update(quiz);

                    User user = await _userRepo.Get(quiz.QuizCreatedBy);
                    if (user is Teacher teacher)
                    {
                        teacher = await _teacherRepo.Get(quiz.QuizCreatedBy);
                        if (teacher.NumsOfQuizCreated != null && teacher.NumsOfQuizCreated > 0)
                        {
                            teacher.NumsOfQuizCreated -= 1;
                            await _teacherRepo.Update(teacher);
                        }
                    }
                    return await MapQuizToQuizReturnDTO(result);
                }
                else
                {
                    throw new UnauthorizedToDeleteException();
                }

            }
            catch(UnauthorizedToDeleteException ex)
            {
                throw ex;
            }
            catch (NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // UNDO QUIZ SOFT DELETE BY QUIZ_ID 
        public async Task<QuizReturnDTO> UndoSoftDeleteQuizByIDAsync(int quizID, int userId)
        {
            try
            {
                Quiz quiz = await _quizRepo.Get(quizID);

                if (quiz.QuizCreatedBy == userId)
                {
                    quiz.IsDeleted = false;
                    var result = await _quizRepo.Update(quiz);

                    User user = await _userRepo.Get(quiz.QuizCreatedBy);
                    if (user is Teacher teacher)
                    {
                        teacher = await _teacherRepo.Get(quiz.QuizCreatedBy);
                        teacher.NumsOfQuizCreated += 1;
                        await _teacherRepo.Update(teacher);
                    }
                    return await MapQuizToQuizReturnDTO(result);
                }
                else
                {
                    throw new UnauthorizedToEditException();
                }

            }
            catch (NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch(UnauthorizedToEditException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GET ALL QUIZZES
        public async Task<List<QuizReturnDTO>> GetAllQuizzesAsync()
        {
            try
            {
                var AllQuizzes = await _quizRepo.Get();
                var quizzes = AllQuizzes.Where(q=>q.IsDeleted == false).ToList();
                List<QuizReturnDTO> quizDTOs = new List<QuizReturnDTO>();

                foreach (var quiz in quizzes)
                {
                    var quizDTO = await MapQuizToQuizReturnDTO(quiz);
                    quizDTOs.Add(quizDTO);
                }

                return quizDTOs;
            }
            catch(NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GET ALL SOFT DELETED QUIZ
        public async Task<List<QuizReturnDTO>> GetAllSoftDeletedQuizzesAsync()
        {
            try
            {
                var AllQuizzes = await _quizRepo.Get();
                var quizzes = AllQuizzes.Where(q => q.IsDeleted == true).ToList();
                List<QuizReturnDTO> quizDTOs = new List<QuizReturnDTO>();

                foreach (var quiz in quizzes)
                {
                    var quizDTO = await MapQuizToQuizReturnDTO(quiz);
                    quizDTOs.Add(quizDTO);
                }

                return quizDTOs;
            }
            catch (NoSuchQuizException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GET ALL QUIZ CREATED BY THE PARTICULAR TEACHER
        public async Task<List<QuizReturnDTO>> GetAllQuizzesCreatedByLoggedInTeacherAsync(int userId)
        {
            try
            {
                var AllQuizzes = await _quizRepo.Get();
                var quizzes = AllQuizzes.Where(q => q.QuizCreatedBy == userId).ToList();
                List<QuizReturnDTO> quizDTOs = new List<QuizReturnDTO>();

                foreach (var quiz in quizzes)
                {
                    var quizDTO = await MapQuizToQuizReturnDTO(quiz);
                    quizDTOs.Add(quizDTO);
                }

                return quizDTOs;
            }
            catch (NoSuchQuizException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // CREATE QUIZ BY USING THE EXISTING QUIZ
        public async Task<QuizReturnDTO> CreateQuizFromExistingQuiz(int quizID, int userId)
        {
            try
            {
                Quiz originalQuiz = await _quizRepo.Get(quizID);

                Quiz newQuiz = new Quiz
                {
                    QuizName = originalQuiz.QuizName,
                    QuizDescription = originalQuiz.QuizDescription,
                    QuizType = originalQuiz.QuizType,
                    CreatedOn = DateTime.Now,
                    NumOfQuestions = originalQuiz.NumOfQuestions,
                    QuizCreatedBy = userId,
                    IsMultipleAttemptAllowed = originalQuiz.IsMultipleAttemptAllowed,
                    TimeLimit = originalQuiz.TimeLimit,
                    QuizQuestions = new List<QuizQuestion>(),
                    TotalPoints = originalQuiz.TotalPoints
                };

                foreach (var quizQuestion in originalQuiz.QuizQuestions)
                {
                    newQuiz.QuizQuestions.Add(new QuizQuestion { QuizId = newQuiz.Id, QuestionId = quizQuestion.QuestionId });
                }

                var result = await _quizRepo.Add(newQuiz);

                Teacher teacher = await _teacherRepo.Get(userId);
                teacher.NumsOfQuizCreated = (teacher.NumsOfQuizCreated ?? 0) + 1;
                await _teacherRepo.Update(teacher);

                return await MapQuizToQuizReturnDTO(result);
            }
            catch (NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch (NoSuchUserException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ADD QUESTIONS TO EXISTING QUIZ
        public async Task<QuizReturnDTO> AddQuestionsToQuizAsync(AddQuestionsToQuizDTO inputDTO, int userId)
        {
            try
            {
                Quiz quiz = await _quizRepo.Get(inputDTO.QuizId);
                if (userId == quiz.QuizCreatedBy)
                {
                    decimal totalPoints = quiz.TotalPoints;
                    foreach (var questionId in inputDTO.QuestionIds)
                    {
                        if (quiz.QuizQuestions.Any(q => q.QuestionId == questionId))
                        {
                            continue;
                        }
                        var question = await _questionRepository.Get(questionId);
                        totalPoints += question.Points;
                        quiz.QuizQuestions.Add(new QuizQuestion { QuizId = quiz.Id, QuestionId = questionId });
                        quiz.NumOfQuestions += 1;
                    }
                    quiz.TotalPoints = totalPoints;
                    await _quizRepo.Update(quiz);
                    return await MapQuizToQuizReturnDTO(quiz);
                }
                else
                {
                    throw new UnauthorizedToEditException();
                }

            }
            catch (NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch (NoSuchQuestionException ex)
            {
                throw new NoSuchQuestionException(ex.Message);
            }
            catch (UnauthorizedToEditException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
