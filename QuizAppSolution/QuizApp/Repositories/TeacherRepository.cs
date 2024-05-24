using Microsoft.EntityFrameworkCore;
using QuizApp.Contexts;
using QuizApp.Interfaces;
using QuizApp.Models;

namespace QuizApp.Repositories
{
    public class TeacherRepository : UserRepository<Teacher>,IRepository<int, Teacher>
    {
        public TeacherRepository(QuizAppContext context) : base(context)
        {

        }
    }
}
