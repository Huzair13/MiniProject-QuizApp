const startQuizElement = document.getElementById('startQuizElement');
const errorMessageElement = document.getElementById('errorMessageDivStartQuiz');

const token = localStorage.getItem('token');
if (!token) {
    window.location.href = '/Home/Home.html'
}


document.addEventListener('DOMContentLoaded', async function () {
    //QuizID from parameter
    const params = new URLSearchParams(window.location.search);
    const quizId = params.get('quizID');
    const isAnswersSubmitted = sessionStorage.getItem("answersSubmitted");

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
                window.location.href = '/LoggedInHome/LoggedInHome.html';
            }
        }, 1000);
    }

    // Show loading modal
    async function showLoadingModal() {
        const loadingAnimationContainer = document.getElementById('loadingAnimation');
        loadingAnimationContainer.innerHTML = ''; 
        

        const animation = bodymovin.loadAnimation({
            container: loadingAnimationContainer,
            renderer: 'svg',
            loop: true,
            autoplay: true,
            path: 'https://lottie.host/c8bd9837-fcdf-4106-8906-b454da03b8b7/9qRpxRP31N.json' 
        });

        $('#loadingModal').modal('show');
    }

    // Hide loading modal
    function hideLoadingModal() {
        $('#loadingModal').modal('hide');
    }


    //token from local storage
    const token = localStorage.getItem('token');

    if (!quizId) {
        window.location.href = '/LoggedInHome/LoggedInHome.html';
        return;
    }

    const questionList = document.getElementById('question-list');
    const answerArea = document.getElementById('answer-area');
    const previousBtn = document.getElementById('previous-btn');
    const nextBtn = document.getElementById('next-btn');
    const submitAnswersBtn = document.getElementById('submit-btn');

    //questions from local storage
    let questions = JSON.parse(sessionStorage.getItem(`questions${quizId}`)) || [];
    // console.log(questions);

    let currentIndex = 0;

    //answers
    // let Answers = {};
    let Answers = JSON.parse(sessionStorage.getItem(`answers${quizId}`)) || {};

    // Timer variables
    const quizDataKey = `QuizData${quizId}`;
    // console.log(quizDataKey);

    const quizData = JSON.parse(sessionStorage.getItem(quizDataKey));
    // console.log(quizData);

    const timeLimit = quizData ? parseTimeLimit(quizData.timelimit) : 20 * 60 * 1000;
    // console.log(timeLimit);

    const startTimeKey = `StartTime${quizId}`;
    // console.log(startTimeKey);

    let startTime = sessionStorage.getItem('startedTime');
    // console.log('StartTime :', startTime);

    const timerDisplay = document.getElementById('timer');

    function parseTimeLimit(timeLimitStr) {
        const parts = timeLimitStr.split(':');
        const hours = parseInt(parts[0], 10);
        const minutes = parseInt(parts[1], 10);
        const seconds = parseInt(parts[2], 10);
        return (hours * 3600 + minutes * 60 + seconds) * 1000;
    }

    function startTimer(startTimeStr, timeLimit) {

        const animationContainer = document.getElementById('lottie-animation');
        const animationData = {
            container: animationContainer,
            renderer: 'svg',
            loop: true,
            autoplay: true,
            path: 'https://lottie.host/25ab4c6f-f452-4136-aaeb-711d766bdc0b/FPY7X1X5mw.json' 
        };
        lottie.loadAnimation(animationData);

        if (!startTimeStr) {
            timerDisplay.textContent = "No start time available.";
            return;
        }

        const startTime = parseInt(startTimeStr, 10);
        if (isNaN(startTime)) {
            timerDisplay.textContent = "Invalid start time.";
            return;
        }

        const remainingTime = calculateRemainingTime(startTime, timeLimit);

        if (remainingTime <= 0) {
            timerDisplay.textContent = "The start time is in the past or the quiz has already ended.";
            return;
        }

        // console.log("Remaining time:", remainingTime);

        const endTime = startTime + timeLimit;
        // console.log("End time:", endTime);

        const timerInterval = setInterval(() => {
            const now = new Date().getTime();
            const distance = endTime - now;

            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);

            timerDisplay.textContent = `Time Remaining: ${minutes}m ${seconds}s`;

            if (distance < 0) {
                clearInterval(timerInterval);
                timerDisplay.textContent = "Time's up!";
                // submitAnswersBtn.click();
                automaticSubmit();
            }
        }, 1000);
    }

    function calculateRemainingTime(startTime, timeLimit) {
        const currentTime = new Date().getTime();
        // console.log("Current time:", currentTime);

        const elapsedTime = currentTime - startTime;
        // console.log("Elapsed time:", elapsedTime);

        const remainingTime = timeLimit - elapsedTime;
        // console.log(`Remaining time: ${remainingTime}`);
        return remainingTime > 0 ? remainingTime : 0;
    }

    startTimer(startTime, timeLimit);



    function setQuizQuestionsFromLocalStorage() {
        questionList.innerHTML = '';

        questions.forEach((question, index) => {
            const listItem = document.createElement('li');
            listItem.textContent = "Question " + question.questionId;
            listItem.classList.add('list-group-item');
            listItem.dataset.index = index;
            listItem.addEventListener('click', () => {
                
                saveCurrentAnswer();
                sessionStorage.setItem(`answers${quizId}`, JSON.stringify(Answers));
                const listItems = document.querySelectorAll('.list-group-item');
                listItems.forEach(item => item.classList.remove('selected-item'));

                listItem.classList.add('selected-item');

                listItems.forEach(item => {
                    item.style.background = "#ECD06F";
                });

                listItem.style.background = "#343a40";

                showQuestion(index);
            });

            listItem.style.background = "#ECD06F";
            listItem.style.border = "1px solid black";
            listItem.style.borderRadius = "0";
            listItem.type = "button";

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
                <div class="card" style="background:#ECECEC;">
                    <div class="card-body">
                        <h5 class="card-title ms-3">${question.questionId}</h5>
                        <p class="card-text ms-3">${question.questionText}</p>
                        <div class="mb-3 fill-ups-answer">
                            <input type="text" class="form-control" id="fillInput" value="${savedAnswer}" placeholder="Enter your answer" required>
                        </div>
                    </div>
                </div>
            `;
        } else if (question.questionType === 'MultipleChoice') {
            if (question.options && Array.isArray(question.options)) {
                let optionsHTML = question.options.map(option => `
                    <div class="form-check m-3 p-3" style="background:#FF9398;">
                        <input class="form-check-input ms-1" type="radio" name="mcqOptions" id="${option}" value="${option}" ${savedAnswer === option ? 'checked' : ''}>
                        <label class="form-check-label ms-3" for="${option}">
                            ${option}
                        </label>
                    </div>
                `).join('');

                answerArea.innerHTML = `
                    <div class="card" style="background:#ECECEC;">
                        <div class="card-body">
                            <h5 class="card-title ms-3">${question.questionId}</h5>
                            <p class="card-text ms-3">${question.questionText}</p>
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
                <div class="card" style="background:#ECECEC;">
                    <div class="card-body">
                        <h5 class="card-title ms-3">${question.questionId}</h5>
                        <p class="card-text ms-3">${question.questionText}</p>
                        <div class="m-3 fill-ups-answer">
                            <input type="text" class="form-control" id="fillInput" placeholder="Enter your answer" required>
                        </div>
                    </div>
                </div>
            `;
        } else if (question.questionType === 'MultipleChoice') {
            if (question.options && Array.isArray(question.options)) {
                let optionsHTML = question.options.map(option => `
                    <div class="form-check m-3 p-2" style="background:#FF9398;">
                        <input class="form-check-input ms-1" type="radio" name="mcqOptions" id="${option}" value="${option}">
                        <label class="form-check-label ms-3" for="${option}">
                            ${option}
                        </label>
                    </div>
                `).join('');

                answerArea.innerHTML = `
                    <div class="card" style="background:#ECECEC;">
                        <div class="card-body">
                            <h5 class="card-title ms-3">${question.questionId}</h5>
                            <p class="card-text ms-3">${question.questionText}</p>
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

        // console.log(selectedQuestion);

        if (selectedQuestion.questionType === 'Fillups') {
            const fillInput = document.getElementById('fillInput');
            Answers[selectedQuestion.questionId] = fillInput.value;
        } else if (selectedQuestion.questionType === 'MultipleChoice') {
            const selectedOption = document.querySelector('input[name="mcqOptions"]:checked');
            if (selectedOption) {
                Answers[selectedQuestion.questionId] = selectedOption.value;
            }
        }

        sessionStorage.setItem(`answers${quizId}`, JSON.stringify(Answers));
    }


    //SUBMIT ANSWER
    submitAnswersBtn.addEventListener('click', async function () {
        const confirmation = confirm("Are you sure you want to submit the Quiz?");
        if (confirmation) {
            await showLoadingModal()
            saveCurrentAnswer();
            const unansweredQuestions = questions.filter(question => !Answers.hasOwnProperty(question.questionId));
            if (unansweredQuestions.length > 0) {
                setTimeout(function () {
                    alert(`Please answer all questions before submitting.`);
                    hideLoadingModal();
                }, 500);
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

            // console.log('Submitted Answers:', JSON.stringify(submittedAnswers));

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
                    // console.log(data);
                    sessionStorage.setItem("answersSubmitted", "true");
                    alert('Answers submitted successfully.');
                    sessionStorage.removeItem(`answers${quizId}`);
                    hideLoadingModal();
                    window.location.href = `/LeaderBoard/QuizLeaderBoard.html?quizID=${quizId}`;
                } else {
                    const data = await response.text();
                    // console.log(data);
                    sessionStorage.setItem("answersSubmitted", "true");
                    alert(data);
                    sessionStorage.removeItem(`answers${quizId}`);
                    hideLoadingModal();
                    window.location.href = `/LeaderBoard/QuizLeaderBoard.html?quizID=${quizId}`;
                }
            } catch (error) {
                console.error('Error:', error.message);
                sessionStorage.removeItem(`answers${quizId}`);

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
        }
    });

    function automaticSubmit() {
        // saveCurrentAnswer();
        const answeredQuestions = questions.filter(question => Answers.hasOwnProperty(question.questionId));

        // Prepare answers to submit
        const questionAnswers = {};
        answeredQuestions.forEach(question => {
            questionAnswers[question.questionId] = Answers[question.questionId];
        });

        // console.log(questionAnswers)

        const userId = localStorage.getItem('userID') || 0;
        const submittedAnswers = {
            userId: parseInt(userId),
            quizId: parseInt(quizId),
            questionAnswers: questionAnswers
        };

        fetch('http://localhost:5273/api/QuizAttempt/submitAllAnswer', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(submittedAnswers)
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(errorData => {
                        throw new Error(errorData.message);
                    });
                }
                return response.json();
            })
            .then(data => {
                // console.log(data);
                sessionStorage.setItem("answersSubmitted", "true");
                alert('Answers automatically submitted.');
                sessionStorage.removeItem(`answers${quizId}`);
                window.location.href="/LoggedInHome/LoggedInHome.html"
            })
            .catch(error => {
                sessionStorage.removeItem(`answers${quizId}`);
                console.error('Error:', error.message);
                showError(error.message);
            });
    }

    function showError(message) {
        console.error('Error:', message);
        // Display error message in your UI (e.g., update an error div)
    }


    function updateButtonStates() {
        previousBtn.disabled = currentIndex === 0;
        nextBtn.disabled = currentIndex === questions.length - 1;
    }
});
