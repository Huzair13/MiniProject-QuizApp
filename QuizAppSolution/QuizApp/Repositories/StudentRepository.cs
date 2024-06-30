using Microsoft.EntityFrameworkCore;
using QuizApp.Contexts;
using QuizApp.Interfaces;
using QuizApp.Models;

namespace QuizApp.Repositories
{
    public class StudentRepository : GenericUserRepository<Student>, IRepository<int, Student>
    {
        public StudentRepository(QuizAppContext context) : base(context)
        {
        }

    }
}
