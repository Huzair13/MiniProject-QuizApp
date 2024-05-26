using Microsoft.EntityFrameworkCore;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;

namespace QuizApp.Repositories
{
    public class MultipleChoiceRepository : IRepository<int, MultipleChoice>
    {
        private readonly QuizAppContext _context;
        public MultipleChoiceRepository(QuizAppContext context)
        {
            _context = context;
        }

        public async Task<MultipleChoice> Add(MultipleChoice item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<MultipleChoice> Delete(int QuestionId)
        {
            var question = await Get(QuestionId);
            if (question != null)
            {
                _context.Remove(question);
                _context.SaveChangesAsync(true);
                return question;
            }
            throw new NoSuchQuestionException(QuestionId);
        }

        public async Task<MultipleChoice> Get(int QuestionId)
        {
            var question = await _context.MultipleChoices.FirstOrDefaultAsync(e => e.Id == QuestionId);
            if(question != null)
            {
                return question;
            }

            throw new NoSuchQuestionException(QuestionId);
        }

        public async Task<IEnumerable<MultipleChoice>> Get()
        {
            var questions = await _context.MultipleChoices.ToListAsync();
            if (questions.Count != 0)
            {
                return questions;
            }
            throw new NoSuchQuestionException();
        }

        public async Task<MultipleChoice> Update(MultipleChoice item)
        {
            var question = await Get(item.Id);
            if (question != null)
            {
                _context.Update(item);
                _context.SaveChangesAsync(true);
                return question;
            }
            throw new NoSuchQuestionException(item.Id);
        }
    }
}
