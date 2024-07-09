const token = localStorage.getItem('token');
if (!token) {
    window.location.href = '/Home/Home.html'
}

function openLeaderboard() {
    const quizId = document.getElementById('quizIdInput').value;
    // console.log('Quiz ID:', quizId);
    window.location.href = '/LeaderBoard/QuizLeaderboard.html?quizID=' + quizId;
    const modal = bootstrap.Modal.getInstance(document.getElementById('leaderboardModal'));
    modal.hide();
}

function CheckResult() {
    const quizId = document.getElementById('ResultQuizIdInput').value;
    // console.log('Quiz ID:', quizId);
    window.location.href = '/QuizResult/QuizResult.html?quizID=' + quizId;
    const modal = bootstrap.Modal.getInstance(document.getElementById('leaderboardModal'));
    modal.hide();
}

function viewQuizBin() {
    window.location.href = '/QuizRecycleBin/DeletedQuiz.html'
}
function viewQuestionBin() {
    window.location.href = '/QuestionRecycleBin/DeletedQuestions.html'
}

function viewQuestions() {
    window.location.href = '/MyQuestions/MyQuestions.html'
}

function viewQuizzes() {
    window.location.href = '/MyQuizzes/MyQuizzes.html'
}

function viewResponses() {
    window.location.href = '/ViewAllResponses/Responses.html'
}

const role = localStorage.getItem('role');
const response = document.getElementById('responseLibrary');
const question = document.getElementById('questionLibrary')
const quiz = document.getElementById('quizLibrary')
const leaderboard = document.getElementById('leaderboardLibrary')
const result = document.getElementById('resultLibrary')
const quesBin = document.getElementById('quesBinLibrary');
const quizBin = document.getElementById('quizBinLibrary');

if (role == "Teacher") {
    response.classList.remove('hidden')
    question.classList.remove('hidden')
    quiz.classList.remove('hidden')
    leaderboard.classList.remove('hidden')
    result.classList.remove('hidden')
    quesBin.classList.remove('hidden')
    quizBin.classList.remove('hidden')
} else if (role == "Student") {
    response.classList.remove('hidden')
    leaderboard.classList.remove('hidden')
    result.classList.remove('hidden')
}

document.addEventListener('DOMContentLoaded', function () {
    const logoutButton = document.getElementById('logoutbtn');
    const logoutModal = new bootstrap.Modal(document.getElementById('logoutModal'));
    const confirmLogoutButton = document.getElementById('confirmLogoutButton');

    logoutButton.addEventListener('click', function (event) {
        event.preventDefault();
        logoutModal.show();
    });

    confirmLogoutButton.addEventListener('click', function (event) {
        event.preventDefault();
        localStorage.removeItem('token');
        localStorage.removeItem('userID');
        localStorage.removeItem('role');

        window.location.href = '/Login/Login.html';
    });

})
