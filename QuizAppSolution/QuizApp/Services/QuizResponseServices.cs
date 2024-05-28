using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models.DTOs.ResponseDTO;
using QuizApp.Models.DTOs.ResponseDTOs;

namespace QuizApp.Services
{
    public class QuizResponseServices : IQuizResponseServices
    {
        //REPOSITORIES
        private readonly IRepository<int, Quiz> _quizRepo;
        private readonly IRepository<int, Question> _questionRepository;
        private readonly IRepository<int, Response> _responseRepo;
        private readonly IRepository<int,Student> _studentRepo;
        private readonly IRepository<int, User> _userRepo;


        //INJECTING THE REPOSITORY
        public QuizResponseServices(
            IRepository<int, Quiz> quizRepo,IRepository<int, Question> questionRepository,
            IRepository<int, Response> responseRepo, IRepository<int, Student> studentRepo
            ,IRepository<int,User> userRepo)
        {
            _quizRepo = quizRepo;
            _questionRepository = questionRepository;
            _responseRepo = responseRepo;
            _studentRepo = studentRepo;
            _userRepo = userRepo;

        }

        //GET QUIZ RESULT
        public async Task<IList<QuizResultDTO>> GetQuizResultAsync(int userId, int quizId)
        {
            try
            {
                List<QuizResultDTO> QuizResults = new List<QuizResultDTO>();

                var AllResponses = await _responseRepo.Get();
                var responses = AllResponses
                            .Where(r => r.UserId == userId && r.QuizId == quizId)
                            .OrderByDescending(r => r.ScoredPoints);
                foreach(var response in responses)
                {
                    var answeredQuestions = response.ResponseAnswers.Select(ra => new AnsweredQuestionDTO
                    {
                        QuestionId = ra.QuestionId,
                        SubmittedAnswer = ra.SubmittedAnswer,
                        CorrectAnswer = (ra.Question is MultipleChoice mcq) ? mcq.CorrectChoice : (ra.Question is FillUps fillUps) ? fillUps.CorrectAnswer : null,
                        IsCorrect = ra.IsCorrect
                    }).ToList();

                    QuizResults.Add(new QuizResultDTO
                    {
                        ResponseId = response.Id,
                        UserId = userId,
                        QuizId = quizId,
                        Score = response.ScoredPoints,
                        AnsweredQuestions = answeredQuestions
                    });
                }
                return QuizResults;
            }
            catch (NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        //START QUIZ
        public async Task<StartQuizResponseDTO> StartQuizAsync(int userId, int quizId)
        {
            try
            {
                var quiz = await _quizRepo.Get(quizId);
                bool isMultipleAttemptAllowed = quiz.IsMultipleAttemptAllowed;

                var responses = await _responseRepo.Get();
                var existingResponse = responses.FirstOrDefault(r => r.UserId == userId && r.QuizId == quizId);
                if (existingResponse!=null)
                {
                    if (!isMultipleAttemptAllowed)
                    {
                        throw new QuizAlreadyStartedException(quizId);
                    }
                    else
                    {
                        return await StartMultipleAttemptQuiz(userId, quizId, quiz);
                    }
                    
                }
                else
                {
                    return await StartSingleAttemptQuiz(userId, quizId, quiz);
                }

                
            }
            catch (NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //START SINGLE ATTEMPT ALLOWED QUIZ
        private async Task<StartQuizResponseDTO> StartSingleAttemptQuiz(int userId, int quizId, Quiz quiz)
        {
            var response = new Response
            {
                UserId = userId,
                QuizId = quizId,
                ScoredPoints = 0,
                StartTime = DateTime.Now,
                EndTime = null
            };

            await _responseRepo.Add(response);

            var questions = quiz.QuizQuestions.Select(q => new QuizResponseQuestionDTO
            {
                QuestionId = q.QuestionId,
                QuestionText = q.Question.QuestionText,
                QuestionType = q.Question.QuestionType,
                Options = (q.Question is MultipleChoice mc)
                            ? new List<string> { mc.Choice1, mc.Choice2, mc.Choice3, mc.Choice4 }
                            : new List<string>()
            }).ToList();

            await UpdateNumsOfQuizAttended(userId);

            return new StartQuizResponseDTO
            {
                QuizId = quizId,
                QuizName = quiz.QuizName,
                Questions = questions
            };
        }

        //UPDATE NUMBER OF QUIZ ATTENDED
        private async Task UpdateNumsOfQuizAttended(int userId)
        {
            User user = await _userRepo.Get(userId);
            if (user is Student student)
            {
                student = await _studentRepo.Get(userId);
                int numOfQuizAttended = student.NumsOfQuizAttended ?? 0;
                if (numOfQuizAttended == 0)
                {
                    student.NumsOfQuizAttended = 1;
                }
                else
                {
                    student.NumsOfQuizAttended += 1;
                }
                await _studentRepo.Update(student);
            }
        }

        //START MULTIPLE ATTEMPT ALLOWED QUIZ
        private async Task<StartQuizResponseDTO> StartMultipleAttemptQuiz(int userId, int quizId, Quiz quiz)
        {
            var response = new Response
            {
                UserId = userId,
                QuizId = quizId,
                ScoredPoints = 0,
                StartTime = DateTime.Now,
                EndTime = null
            };

            await _responseRepo.Add(response);

            var questions = quiz.QuizQuestions.Select(q => new QuizResponseQuestionDTO
            {
                QuestionId = q.QuestionId,
                QuestionText = q.Question.QuestionText,
                QuestionType = q.Question.QuestionType,
                Options = (q.Question is MultipleChoice mc)
                            ? new List<string> { mc.Choice1, mc.Choice2, mc.Choice3, mc.Choice4 }
                            : new List<string>()
            }).ToList();


            return new StartQuizResponseDTO
            {
                QuizId = quizId,
                QuizName = quiz.QuizName,
                Questions = questions
            };
        }

        //SUBMIT ALL ANSWERS
        public async Task<string> SubmitAllAnswersAsync(SubmitAllAnswersDTO submitAllAnswersDTO)
        {
            try
            {
                
                var quiz = await _quizRepo.Get(submitAllAnswersDTO.QuizId);

                var responses = await _responseRepo.Get();
                var response = responses
                    .Where(r => r.UserId == submitAllAnswersDTO.UserId && r.QuizId == submitAllAnswersDTO.QuizId)
                    .OrderByDescending(r => r.StartTime)
                    .FirstOrDefault();

                if (response == null)
                {
                    throw new QuizNotStartedException(submitAllAnswersDTO.QuizId);
                }

                foreach (var questionId in submitAllAnswersDTO.QuestionAnswers.Keys)
                {
                    var answer = submitAllAnswersDTO.QuestionAnswers[questionId];

                    var quizQuestion = quiz.QuizQuestions.FirstOrDefault(q => q.QuestionId == questionId);
                    if (quizQuestion == null)
                    {
                        throw new NoSuchQuestionException(questionId);
                    }

                    var question = await _questionRepository.Get(questionId);

                    var existingAnswer = response.ResponseAnswers.FirstOrDefault(ra => ra.QuestionId == questionId);
                    if (existingAnswer != null)
                    {
                        throw new UserAlreadyAnsweredTheQuestionException($"User has already answered question with ID {questionId}.");
                    }

                    bool isCorrect = false;

                    if (question is MultipleChoice multipleChoiceQuestion)
                    {
                        isCorrect = multipleChoiceQuestion.CorrectChoice == answer;
                    }
                    else if (question is FillUps fillUpsQuestion)
                    {
                        isCorrect = fillUpsQuestion.CorrectAnswer == answer;
                    }

                    response.ResponseAnswers.Add(new ResponseAnswer
                    {
                        QuestionId = questionId,
                        SubmittedAnswer = answer,
                        IsCorrect = isCorrect
                    });

                    if (isCorrect)
                    {
                        response.ScoredPoints += question.Points;
                    }
                }

                await UpdateStudentCoins(quiz, response, submitAllAnswersDTO.UserId ?? 0);
                
                var results=await _responseRepo.Update(response);
                return "Answers Submitted Successfully";
            }
            catch (NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch (NoSuchQuestionException ex)
            {
                throw new NoSuchQuestionException(ex.Message);
            }
            catch (UserAlreadyAnsweredTheQuestionException ex)
            {
                throw new UserAlreadyAnsweredTheQuestionException(ex.Message);
            }
            catch(NoSuchUserException ex)
            {
                throw new NoSuchUserException(ex.Message);
            }
            catch(NoSuchResponseException ex)
            {
                throw new NoSuchResponseException(ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE STUDENT COINS
        private async Task UpdateStudentCoins(Quiz quiz, Response response, int userId)
        {
            var totalPoints = quiz.QuizQuestions.Sum(q => q.Question.Points);
            response.EndTime = DateTime.Now;
            if (totalPoints == response.ScoredPoints)
            {
                Student student = await _studentRepo.Get(userId);
                int coinsEarned = student.CoinsEarned ?? 0;
                if (coinsEarned == 0)
                {
                    student.CoinsEarned = 10;
                }
                else
                {
                    student.CoinsEarned += 5;
                }
                await _studentRepo.Update(student);
            }
        }

        //SUBMIT QUIZ ANSWER ONE BY ONE
        public async Task<string> SubmitAnswerAsync(SubmitAnswerDTO submitAnswerDTO)
        {
            try
            {
                
                var quiz = await _quizRepo.Get(submitAnswerDTO.QuizId);
                var quizQuestion = quiz.QuizQuestions.FirstOrDefault(q => q.QuestionId == submitAnswerDTO.QuestionId);

                if (quizQuestion == null)
                {
                    throw new NoSuchQuestionException($"Question with ID {submitAnswerDTO.QuestionId} is not part of the quiz with ID {submitAnswerDTO.QuizId}.");
                }

                var question = await _questionRepository.Get(submitAnswerDTO.QuestionId);

                var responses = await _responseRepo.Get();
                var response = responses
                    .Where(r => r.UserId == submitAnswerDTO.UserId && r.QuizId == submitAnswerDTO.QuizId)
                    .OrderByDescending(r => r.StartTime)
                    .FirstOrDefault();

                var existingAnswer = response.ResponseAnswers.FirstOrDefault(ra => ra.QuestionId == submitAnswerDTO.QuestionId);

                if (existingAnswer != null)
                {
                    throw new UserAlreadyAnsweredTheQuestionException();
                }

                bool isCorrect = false;

                if (question is MultipleChoice multipleChoiceQuestion)
                {
                    isCorrect = multipleChoiceQuestion.CorrectChoice == submitAnswerDTO.Answer;
                }
                else if (question is FillUps fillUpsQuestion)
                {
                    isCorrect = fillUpsQuestion.CorrectAnswer == submitAnswerDTO.Answer;
                }

                response.ResponseAnswers.Add(new ResponseAnswer
                {
                    QuestionId = submitAnswerDTO.QuestionId,
                    SubmittedAnswer = submitAnswerDTO.Answer,
                    IsCorrect = isCorrect
                });

                if (isCorrect)
                {
                    response.ScoredPoints += question.Points;
                }

                if (response.ResponseAnswers.Count == quiz.QuizQuestions.Count)
                {
                    await UpdateStudentCoins(quiz, response, submitAnswerDTO.UserId ?? 0);
                }

                var result = await _responseRepo.Update(response);
                if (result != null)
                {
                    return "Answer Submitted";
                }
                return string.Empty;
                
            }
            catch(NoSuchQuizException ex)
            {
                throw new NoSuchQuizException(ex.Message);
            }
            catch(NoSuchQuestionException ex)
            {
                throw new NoSuchQuestionException(ex.Message);
            }
            catch(NoSuchResponseException ex)
            {
                throw new NoSuchResponseException(ex.Message);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }
    }
}
