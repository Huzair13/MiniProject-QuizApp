const QuizNameInput = document.getElementById('quizName');
const QuizDescriptionInput = document.getElementById('quizDescription');
const QuizTypeInput = document.getElementById('quizType');
const NumOfQuestionsInput = document.getElementById('numOfQuestions');
const TotalPointsInput = document.getElementById('totalPoints');
const isMultipleAllowedElement = document.getElementById('isMultipleAllowed');
const timeLimitElement = document.getElementById('timeLimit');
const errorMessageElement = document.getElementById('errorMessageDiv');
const centeredCardElement = document.getElementById('centeredCard');
const errorMessage = document.getElementById('errorMessage');

const startQuizBtn = document.getElementById('startQuizButtonStartPage');


document.addEventListener('DOMContentLoaded', async function () {
    const params = new URLSearchParams(window.location.search);
    const QuizId = params.get('quizID');
    console.log(QuizId);

    if (!QuizId) {
        window.location.href = '/LoggedInHome/StudentHome.html';
        return;
    }

    const token = localStorage.getItem('token');
    await fetchQuizDetails(QuizId, token);

    startQuizBtn.addEventListener('click', function () {
        window.location.href = `/StartQuiz/StartQuizSideBar.html?quizID=${QuizId}`;
    })

});

async function fetchQuizDetails(QuizId, token) {
    const url = `http://localhost:5273/api/ViewQuiz/GetQuizByIDWithoutQuestions`;

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(QuizId)
        });

        console.log('Response:', response);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();
        console.log('Data:', data);

        if (!data || !data.quizName) {
            throw new Error('Invalid quiz data');
        }

        let quizDataRetrived = data;
        quizDataRetrived.isAnweredQuestions = false;
        quizDataRetrived.startedTime = new Date().getTime();
        localStorage.setItem(`QuizData${data.QuizId}`,JSON.stringify(quizDataRetrived));
        console.log(localStorage.getItem(`QuizData${data.QuizId}`));
        console.log("hello")
        updateQuizDetails(data);

    } catch (error) {
        console.error('Error fetching quiz details:', error);
        showErrorAndRedirect();
    }
}

function updateQuizDetails(quizData) {
    try {
        console.log('Updating quiz details:', quizData);
        QuizNameInput.textContent = quizData.quizName;
        QuizDescriptionInput.textContent = quizData.quizDescription;
        QuizTypeInput.textContent = quizData.quizType;
        NumOfQuestionsInput.textContent = quizData.numOfQuestions;
        TotalPointsInput.textContent = quizData.totalPoints;

        isMultipleAllowedElement.innerHTML = quizData.isMultpleAllowed ?
            '<i class="fas fa-check-circle icon" style="color: green;"></i> Yes' :
            '<i class="fas fa-times-circle icon" style="color: red;"></i> No';

        timeLimitElement.innerHTML = `<i class="far fa-clock icon" style="color: #007bff;"></i> ${quizData.timelimit} minutes`;

    } catch (error) {
        console.error('Error updating quiz details:', error);
    }
}



function showErrorAndRedirect() {
    centeredCardElement.classList.add('hidden');
    errorMessageElement.classList.remove('hidden');

    let countdown = 5;
    errorMessage.textContent = `Invalid Quiz ID. Redirecting in ${countdown} seconds...`;

    const interval = setInterval(() => {
        countdown--;
        errorMessage.textContent = `Invalid Quiz ID. Redirecting in ${countdown} seconds...`;

        if (countdown <= 0) {
            clearInterval(interval);
            window.location.href = '/LoggedInHome/StudentHome.html';
        }
    }, 1000);
}
