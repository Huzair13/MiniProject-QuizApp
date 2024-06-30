document.addEventListener('DOMContentLoaded', function () {
    const params = new URLSearchParams(window.location.search);
    const quizId = params.get('quizID');
    const token = localStorage.getItem('token');

    const questionList = document.getElementById('question-list');
    const answerArea = document.getElementById('answer-area');
    const previousBtn = document.getElementById('previous-btn');
    const nextBtn = document.getElementById('next-btn');
    const submitAnswersBtn = document.getElementById('submit-btn');
    let questions = [];
    let currentIndex = 0;
    let Answers = {}; 

    fetch('/test.json')
        .then(response => response.json())
        .then(data => {
            questions = data; 

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
        })
        .catch(error => {
            console.error('Error fetching or parsing data:', error);
        });

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

    previousBtn.addEventListener('click', function () {
        if (currentIndex > 0) {
            saveCurrentAnswer(); 
            showQuestion(currentIndex - 1);
        }
    });

    nextBtn.addEventListener('click', function () {
        if (currentIndex < questions.length - 1) {
            saveCurrentAnswer(); 
            showQuestion(currentIndex + 1);
        }
    });


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

    submitAnswersBtn.addEventListener('click', function () {
        saveCurrentAnswer();
        const unansweredQuestions = questions.filter(question => !Answers.hasOwnProperty(question.questionId));
        if (unansweredQuestions.length > 0) {
            alert(`Please answer all questions before submitting.`);
            return;
        }
    
        const userId = localStorage.getItem('userID') || 0; // Retrieve userId from local storage or set to 0 if not found
    
        const questionAnswers = {};
        questions.forEach(question => {
            questionAnswers[question.questionId] = Answers[question.questionId];
        });
    
        const submittedAnswers = {
            userId: parseInt(userId), 
            quizId: parseInt(quizId),
            questionAnswers: questionAnswers
        };
    
        console.log('Submitted Answers:', JSON.stringify(submittedAnswers, null, 2));
        alert('Answers submitted successfully.');
    });
    

    function updateButtonStates() {
        previousBtn.disabled = currentIndex === 0;
        nextBtn.disabled = currentIndex === questions.length - 1;
    }
});
