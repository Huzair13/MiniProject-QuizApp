﻿using Microsoft.EntityFrameworkCore;
using QuizApp.Models;

namespace QuizApp.Contexts
{
    public class QuizAppContext :DbContext
    {
        public QuizAppContext(DbContextOptions options) : base(options)
        {

        }

        DbSet<User> Users { get; set; }
        DbSet<UserDetails> UsersDetails { get; set; }
        DbSet<Student> Students { get; set; }
        DbSet<Teacher> Teachers { get; set; }
        DbSet<MultipleChoice> MultipleChoices { get; set; }
        DbSet<FillUps> FillUps { get; set; }
        DbSet<Response> Responses { get; set; }
        DbSet<Question> Questions { get; set; }
        DbSet<Quiz> Quizzes { get; set; }
        DbSet<QuizQuestion> QuizQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //user
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 101, Name = "Janu", Email = "janu@gmail.com", MobileNumber = "1234567890", DateOfBirth = new DateTime(1990, 1, 1),Designation="HOD",NumsOfQuestionsCreated =2,NumsOfQuizCreated=1}
            );

            //Question - MCQ
            modelBuilder.Entity<MultipleChoice>().HasData(
                new MultipleChoice
                {
                    Id = 201,
                    QuestionText = "What is the capital of France?",
                    QuestionType = "Multiple Choice",
                    Points = 10,
                    Category = "Geography",
                    DifficultyLevel = DifficultyLevel.Easy,
                    CreatedDate = DateTime.Now,
                    QuestionCreatedBy = 101,
                    Choice1 = "Paris",
                    Choice2 = "London",
                    Choice3 = "Berlin",
                    Choice4 = "Rome",
                    CorrectChoice = "Paris"
                }
            );

            //Question - FillUps
            modelBuilder.Entity<FillUps>().HasData(
                new FillUps
                {
                    Id = 202,
                    QuestionText = "What is the largest planet in our solar system?",
                    QuestionType = "Fill in the Blank",
                    Points = 15,
                    Category = "Science",
                    DifficultyLevel = DifficultyLevel.Medium,
                    CreatedDate = DateTime.Now,
                    QuestionCreatedBy = 101,
                    CorrectAnswer = "Jupiter"
                }
            );

            //Quiz 
            modelBuilder.Entity<Quiz>().HasData(
                new Quiz { Id = 1, QuizName = "Sample Quiz", QuizDescription = "A sample quiz", QuizType = "Practice", CreatedOn = DateTime.Now, NumOfQuestions = 2, TotalPoints = 25 ,QuizCreatedBy=101}
            );

            //QuizQuestion
            modelBuilder.Entity<QuizQuestion>().HasData(
                new QuizQuestion { QuizId = 1, QuestionId = 201 },
                new QuizQuestion { QuizId = 1, QuestionId = 202 }
            );


            modelBuilder.Entity<Question>()
                       .Property(q => q.Points)
                       .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Response>()
                       .Property(r => r.ScoredPoints)
                       .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Quiz>()
                       .Property(q => q.TotalPoints)
                       .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Question>()
                        .Property(q => q.Category)
                        .HasConversion<string>();

            modelBuilder.Entity<QuizQuestion>()
                .HasKey(q => new { q.QuizId, q.QuestionId });

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(q => q.Quiz)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(q => q.Question)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(q => q.QuestionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<UserDetails>()
                .HasOne(ud => ud.User)
                .WithOne(u => u.UserDetails)
                .HasForeignKey<UserDetails>(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.CreatedByUser)
                .WithMany(u => u.QuestionsCreated)
                .HasForeignKey(q => q.QuestionCreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.QuizCreatedByUser)
                .WithMany(u => u.QuizCreated)
                .HasForeignKey(q => q.QuizCreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}