const startQuizElement = document.getElementById('startQuizElement');
const errorMessageElement = document.getElementById('errorMessageDivStartQuiz');

document.addEventListener('DOMContentLoaded', async function () {
    //QuizID from parameter
    const params = new URLSearchParams(window.location.search);
    const quizId = params.get('quizID');
    const isAnswersSubmitted = localStorage.getItem("answersSubmitted");

    if (isAnswersSubmitted == "true") {
        showErrorAndRedirect();
    }

    function showErrorAndRedirect() {
        startQuizElement.style.display = "none";
        errorMessageElement.classList.remove('hidden');

        let countdown = 5;
        errorMessage.textContent = `Quiz Submitted Already. Redirecting in ${countdown} seconds...`;

        const interval = setInterval(() => {
            countdown--;
            errorMessage.textContent = `Quiz Submitted Already. Redirecting in ${countdown} seconds...`;

            if (countdown <= 0) {
                clearInterval(interval);
                window.location.href = '/LoggedInHome/StudentHome.html';
            }
        }, 1000);
    }

    //token from local storage
    const token = localStorage.getItem('token');

    if (!quizId) {
        window.location.href = '/LoggedInHome/StudentHome.html';
        return;
    }

    const questionList = document.getElementById('question-list');
    const answerArea = document.getElementById('answer-area');
    const previousBtn = document.getElementById('previous-btn');
    const nextBtn = document.getElementById('next-btn');
    const submitAnswersBtn = document.getElementById('submit-btn');

    //questions from local storage
    let questions = JSON.parse(localStorage.getItem(`questions${quizId}`)) || [];
    console.log(questions);

    let currentIndex = 0;

    //answers
    let Answers = {};

    // Timer variables
    const quizDataKey = `QuizData${quizId}`;
    console.log(quizDataKey);

    const quizData = JSON.parse(localStorage.getItem(quizDataKey));
    console.log(quizData);

    const timeLimit = quizData ? parseTimeLimit(quizData.timelimit) : 20 * 60 * 1000;
    console.log(timeLimit);

    const startTimeKey = `StartTime${quizId}`;
    console.log(startTimeKey);

    let startTime = localStorage.getItem('startedTime');
    console.log('StartTime :', startTime);

    const timerDisplay = document.getElementById('timer');

    function parseTimeLimit(timeLimitStr) {
        const parts = timeLimitStr.split(':');
        const hours = parseInt(parts[0], 10);
        const minutes = parseInt(parts[1], 10);
        const seconds = parseInt(parts[2], 10);
        return (hours * 3600 + minutes * 60 + seconds) * 1000;
    }

    function startTimer(startTimeStr, timeLimit) {
        if (!startTimeStr) {
            timerDisplay.textContent = "No start time available.";
            return;
        }

        const startTime = parseInt(startTimeStr, 10); // Ensure the start time is parsed as an integer
        if (isNaN(startTime)) {
            timerDisplay.textContent = "Invalid start time.";
            return;
        }

        const remainingTime = calculateRemainingTime(startTime, timeLimit);

        if (remainingTime <= 0) {
            timerDisplay.textContent = "The start time is in the past or the quiz has already ended.";
            return;
        }

        console.log("Remaining time:", remainingTime);

        const endTime = startTime + timeLimit;
        console.log("End time:", endTime);

        const timerInterval = setInterval(() => {
            const now = new Date().getTime();
            const distance = endTime - now;

            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);

            timerDisplay.textContent = `Time Remaining: ${minutes}m ${seconds}s`;

            if (distance < 0) {
                clearInterval(timerInterval);
                timerDisplay.textContent = "Time's up!";
                submitAnswersBtn.click();
            }
        }, 1000);
    }

    function calculateRemainingTime(startTime, timeLimit) {
        const currentTime = new Date().getTime();
        console.log("Current time:", currentTime);

        const elapsedTime = currentTime - startTime;
        console.log("Elapsed time:", elapsedTime);

        const remainingTime = timeLimit - elapsedTime;
        console.log(`Remaining time: ${remainingTime}`);
        return remainingTime > 0 ? remainingTime : 0;
    }

    startTimer(startTime, timeLimit);



    //QUESTION LIST SIDE BAR
    function setQuizQuestionsFromLocalStorage() {
        questionList.innerHTML = '';

        questions.forEach((question, index) => {
            const listItem = document.createElement('li');
            listItem.textContent = "Question " + question.questionId;
            listItem.classList.add('list-group-item');
            listItem.dataset.index = index;
            listItem.addEventListener('click', () => {
                saveCurrentAnswer();
                showQuestion(index);
            });
            questionList.appendChild(listItem);
        });

        showQuestion(0);
    }

    // QUESTIONS RENDERING
    if (questions.length > 0) {
        setQuizQuestionsFromLocalStorage();
    } else {
        answerArea.innerHTML = `
                <div class="alert alert-danger" role="alert">
                    No quiz questions available. Please contact your instructor.
                </div>
            `;
    }

    //QUESTION AREA
    function showQuestion(index) {
        currentIndex = index;
        const listItems = questionList.querySelectorAll('.list-group-item');
        listItems.forEach(item => item.classList.remove('selected'));
        listItems[index].classList.add('selected');

        const selectedQuestion = questions[index];

        const savedAnswer = Answers[selectedQuestion.questionId];
        if (savedAnswer !== undefined && savedAnswer !== null) {
            loadSavedAnswer(selectedQuestion, savedAnswer);
        } else {
            loadNewQuestion(selectedQuestion);
        }
        updateButtonStates();
    }

    //LOAD SAVED ANSWERS
    function loadSavedAnswer(question, savedAnswer) {
        if (question.questionType === 'Fillups') {
            answerArea.innerHTML = `
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">${question.questionId}</h5>
                        <p class="card-text">${question.questionText}</p>
                        <div class="mb-3">
                            <input type="text" class="form-control" id="fillInput" value="${savedAnswer}" required>
                        </div>
                    </div>
                </div>
            `;
        } else if (question.questionType === 'MultipleChoice') {
            if (question.options && Array.isArray(question.options)) {
                let optionsHTML = question.options.map(option => `
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="mcqOptions" id="${option}" value="${option}" ${savedAnswer === option ? 'checked' : ''}>
                        <label class="form-check-label" for="${option}">
                            ${option}
                        </label>
                    </div>
                `).join('');

                answerArea.innerHTML = `
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">${question.questionId}</h5>
                            <p class="card-text">${question.questionText}</p>
                            <form id="mcqForm">${optionsHTML}</form>
                        </div>
                    </div>
                `;
            } else {
                answerArea.innerHTML = `<p>No options available for this question.</p>`;
            }
        }
    }

    //LOAD NEW QUESTIONS
    function loadNewQuestion(question) {
        if (question.questionType === 'Fillups') {
            answerArea.innerHTML = `
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">${question.questionId}</h5>
                        <p class="card-text">${question.questionText}</p>
                        <div class="mb-3">
                            <input type="text" class="form-control" id="fillInput" placeholder="Enter your answer" required>
                        </div>
                    </div>
                </div>
            `;
        } else if (question.questionType === 'MultipleChoice') {
            if (question.options && Array.isArray(question.options)) {
                let optionsHTML = question.options.map(option => `
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="mcqOptions" id="${option}" value="${option}">
                        <label class="form-check-label" for="${option}">
                            ${option}
                        </label>
                    </div>
                `).join('');

                answerArea.innerHTML = `
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">${question.questionId}</h5>
                            <p class="card-text">${question.questionText}</p>
                            <form id="mcqForm">${optionsHTML}</form>
                        </div>
                    </div>
                `;
            } else {
                answerArea.innerHTML = `<p>No options available for this question.</p>`;
            }
        }
    }

    //PREVIOUS QUESTION BUTTON
    previousBtn.addEventListener('click', function () {
        if (currentIndex > 0) {
            saveCurrentAnswer();
            showQuestion(currentIndex - 1);
        }
    });

    //NEXT QUESTION BUTTON
    nextBtn.addEventListener('click', function () {
        if (currentIndex < questions.length - 1) {
            saveCurrentAnswer();
            showQuestion(currentIndex + 1);
        }
    });


    //SAVE CURRENT ANSWER
    function saveCurrentAnswer() {
        const selectedQuestion = questions[currentIndex];

        if (selectedQuestion.questionType === 'Fillups') {
            const fillInput = document.getElementById('fillInput');
            Answers[selectedQuestion.questionId] = fillInput.value;
        } else if (selectedQuestion.questionType === 'MultipleChoice') {
            const selectedOption = document.querySelector('input[name="mcqOptions"]:checked');
            if (selectedOption) {
                Answers[selectedQuestion.questionId] = selectedOption.value;
            }
        }
    }


    //SUBMIT ANSWER
    submitAnswersBtn.addEventListener('click', async function () {
        saveCurrentAnswer();
        const unansweredQuestions = questions.filter(question => !Answers.hasOwnProperty(question.questionId));
        if (unansweredQuestions.length > 0) {
            alert(`Please answer all questions before submitting.`);
            return;
        }

        const userId = localStorage.getItem('userID') || 0;

        const questionAnswers = {};
        questions.forEach(question => {
            questionAnswers[question.questionId] = Answers[question.questionId];
        });

        const submittedAnswers = {
            userId: parseInt(userId),
            quizId: parseInt(quizId),
            questionAnswers: questionAnswers
        };

        console.log('Submitted Answers:', JSON.stringify(submittedAnswers));

        try {
            const response = await fetch('http://localhost:5273/api/QuizAttempt/submitAllAnswer', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(submittedAnswers)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message);
            }

            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                const data = await response.json();
                console.log(data);
                localStorage.setItem("answersSubmitted", "true");
                alert('Answers submitted successfully.');
                window.location.href = '/LoggedInHome/StudentHome.html';
            } else {
                const data = await response.text();
                console.log(data);
                localStorage.setItem("answersSubmitted", "true");
                alert(data);
                window.location.href = '/LoggedInHome/StudentHome.html';
            }
        } catch (error) {
            console.error('Error:', error.message);

            if (error.message.includes('User has already answered')) {
                answerArea.innerHTML = `
                    <div class="alert alert-warning" role="alert">
                        You have already submitted your answers for this quiz.
                    </div>
                `;

                questionList.style.display = 'none';
            } else {
                console.log('Error message does not contain the substring.');
            }
        }
    });

    function showError(message) {
        console.error('Error:', message);
        // Display error message in your UI (e.g., update an error div)
    }


    function updateButtonStates() {
        previousBtn.disabled = currentIndex === 0;
        nextBtn.disabled = currentIndex === questions.length - 1;
    }
});
