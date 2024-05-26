using Microsoft.EntityFrameworkCore;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;

namespace QuizApp.Repositories
{
    public class QuizRepository : IRepository<int, Quiz>
    {
        private readonly QuizAppContext _context; 

        public QuizRepository(QuizAppContext context)
        {
            _context = context;
        }

        public async Task<Quiz> Add(Quiz item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Quiz> Delete(int quizId)
        {
            var quiz = await Get(quizId);
            if (quiz != null)
            {
                _context.Remove(quiz);
                _context.SaveChangesAsync(true);
                return quiz;
            }
            throw new NoSuchQuizException(quizId);
        }

        public async Task<Quiz> Get(int quizId)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz != null)
            {
                return quiz;
            }
            throw new NoSuchQuizException(quizId);
        }

        public async Task<IEnumerable<Quiz>> Get()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            if (quizzes.Count != 0)
            {
                return quizzes;
            }
            throw new NoSuchQuizException();
        }

        public async Task<Quiz> Update(Quiz quiz)
        {
            var existingQuiz = await Get(quiz.Id);

            if (existingQuiz != null)
            {
                _context.Update(quiz);
                await _context.SaveChangesAsync();
                return existingQuiz;
            }
            throw new NoSuchUserException(quiz.Id);
        }
    }
}
