document.addEventListener('DOMContentLoaded', function () {
    const params = new URLSearchParams(window.location.search);
    const quizId = params.get('quizID');
    const token = localStorage.getItem('token');

    const questionList = document.getElementById('question-list');
    const answerArea = document.getElementById('answer-area');
    const previousBtn = document.getElementById('previous-btn');
    const nextBtn = document.getElementById('next-btn');
    const submitBtn = document.getElementById('submit-btn');
    let questions = [];
    let currentIndex = 0;

    // Simulated fetch from local JSON data (test.json)
    fetch('/test.json')
        .then(response => response.json())
        .then(data => {
            questions = data; // Assuming 'test.json' contains an array of questions

            // Clear existing question list
            questionList.innerHTML = '';

            // Populate sidebar with questions
            questions.forEach((question, index) => {
                const listItem = document.createElement('li');
                listItem.textContent = question.questionText.substring(0, 10); // Adjust substring length as needed
                listItem.classList.add('list-group-item');
                listItem.dataset.index = index;
                listItem.addEventListener('click', () => showQuestion(index));
                questionList.appendChild(listItem);
            });

            // Show the first question initially
            showQuestion(0); // Assuming showQuestion function is defined elsewhere
        })
        .catch(error => {
            console.error('Error fetching or parsing data:', error);
            // Handle errors (e.g., show a message to the user)
        });

    // Function to display selected question in the answer area
    function showQuestion(index) {
        currentIndex = index;

        const listItems = questionList.querySelectorAll('.list-group-item');
        listItems.forEach(item => item.classList.remove('selected'));
        listItems[index].classList.add('selected');

        const selectedQuestion = questions[index];

        // Load answer if already saved
        const savedAnswer = Answers[selectedQuestion.questionId];
        if (savedAnswer !== undefined && savedAnswer !== null) {
            loadSavedAnswer(selectedQuestion, savedAnswer);
        } else {
            loadNewQuestion(selectedQuestion);
        }

        // Update Previous and Next button states
        updateButtonStates();
    }

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
                        <button class="btn btn-primary" id="save-answer-btn">Save Answer</button>
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
                            <button class="btn btn-primary mt-3" id="save-answer-btn">Save Answer</button>
                        </div>
                    </div>
                `;
            } else {
                answerArea.innerHTML = `<p>No options available for this question.</p>`;
            }
        }

        // Save Answer button event listener
        const saveAnswerBtn = document.getElementById('save-answer-btn');
        if (saveAnswerBtn) {
            saveAnswerBtn.addEventListener('click', function () {
                saveAnswer(currentIndex);
            });
        }
    }

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
                        <button class="btn btn-primary" id="save-answer-btn">Save Answer</button>
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
                            <button class="btn btn-primary mt-3" id="save-answer-btn">Save Answer</button>
                        </div>
                    </div>
                `;
            } else {
                answerArea.innerHTML = `<p>No options available for this question.</p>`;
            }
        }

        // Save Answer button event listener
        const saveAnswerBtn = document.getElementById('save-answer-btn');
        if (saveAnswerBtn) {
            saveAnswerBtn.addEventListener('click', function () {
                saveAnswer(currentIndex);
            });
        }
    }

    function saveAnswer(index) {
        const selectedQuestion = questions[index];
        let selectedAnswer = null;

        if (selectedQuestion.questionType === 'Fillups') {
            selectedAnswer = document.getElementById('fillInput').value;
            if (selectedAnswer === "") {
                alert("Please enter the answer to save.");
                return;
            }
        } else if (selectedQuestion.questionType === 'MultipleChoice') {
            const selectedOption = document.querySelector('input[name="mcqOptions"]:checked');
            if (selectedOption) {
                selectedAnswer = selectedOption.value;
            } else {
                alert("Please select an answer to save.");
                return;
            }
        }

        // Store answer in Answers object
        Answers[selectedQuestion.questionId] = selectedAnswer;
        console.log(Answers);
        alert(`Answer saved: ${selectedAnswer}`);
    }

    // Function to handle Previous button click
    previousBtn.addEventListener('click', function () {
        if (currentIndex > 0) {
            showQuestion(currentIndex - 1);
        }
    });

    // Function to handle Next button click
    nextBtn.addEventListener('click', function () {
        if (currentIndex < questions.length - 1) {
            showQuestion(currentIndex + 1);
        }
    });
    
    submitBtn.addEventListener('click', function () {
        const unansweredQuestions = questions.filter(question => !Answers.hasOwnProperty(question.questionId));
        if (unansweredQuestions.length > 0) {
            alert(`Please answer all questions before submitting.`);
            return;
        }
        console.log('Submitting quiz:', Answers);
        alert('Quiz submitted successfully.');
    });

    // Function to update Previous and Next button states
    function updateButtonStates() {
        previousBtn.disabled = currentIndex === 0;
        nextBtn.disabled = currentIndex === questions.length - 1;
    }

    let Answers = {}; // Object to store user's answers
});
