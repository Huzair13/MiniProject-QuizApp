document.addEventListener('DOMContentLoaded',function(){
    const startQuizBtn = document.getElementById('startQuizHome');
    const QuizIDInput = document.getElementById('startQuizNumber');
    startQuizBtn.addEventListener('click',function(){
        const QuizID = QuizIDInput.value;
        window.location.href = `/StartQuiz/StartPage.html?quizID=${QuizID}`;
    })
});