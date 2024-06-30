document.addEventListener('DOMContentLoaded', function () {
    const params = new URLSearchParams(window.location.search);
    const QuizId = params.get('quizID');
    const token = localStorage.getItem('token');

    // if (!QuizId) {
    //     window.location.href = '/LoggedInHome/StudentHome.html';
    //     return;
    // }

    const questionList = document.getElementById('question-list');
    const answerArea = document.getElementById('answer-area');
    const previousBtn = document.getElementById('previous-btn');
    const nextBtn = document.getElementById('next-btn');
    let questions = [];
    let currentIndex = 0;

    // FETCH QUESTIONS
    // 'http://localhost:5273/api/QuizAttempt/StartQuiz', {
    //         method: 'POST',
    //         headers: {
    //             'Content-Type': 'application/json',
    //             'Authorization': `Bearer ${token}`
    //         },
    //         body: JSON.stringify(QuizId)
    //     }
    // fetch('test.json')
    //     .then(response => response.json())
    //     .then(data => {
    //         // console.log(data.questions);
    //         // questions = data.questions;
    //         let qq=[
    //             {
    //                 "questionId": 1,
    //                 "questionText": "Complete the sentence: 'HTML stands for HyperText Markup ____.'",
    //                 "questionType": "fill-in-the-blank"
    //               },
    //               {
    //                 "questionId": 2,
    //                 "questionText": "Which of the following is not a programming language?",
    //                 "questionType": "mcq",
    //                 "qptions": ["HTML", "Java", "Python", "Potato"]
    //               },
    //               {
    //                 "questionId": 2,
    //                 "questionText": "Which of the following is not a programming language?",
    //                 "questionType": "mcq",
    //                 "options": ["HTML", "Java", "Python", "Potato"]
    //               }
    //         ]
    //         questions=qq;
    //         //SIDEBAR QUESTIONS
    //         questions.forEach((question, index) => {
    //             const listItem = document.createElement('li');
    //             listItem.textContent = question.questionText.substring(0, 10);
    //             listItem.classList.add('list-group-item');
    //             listItem.dataset.index = index;
    //             listItem.addEventListener('click', () => showQuestion(index));
    //             questionList.appendChild(listItem);
    //         });

    //         //SHOW FIRST QUESTION INITIALLY
    //         showQuestion(currentIndex);
    //     });

    fetch('test.json')
    .then(response => response.json())
    .then(data => {
        let questions = data; // Assuming 'test.json' contains an array of questions

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


    //DISPLAY SELECTED QUESTION FROM SIDEBARS
    function showQuestion(index) {
        currentIndex = index;

        const listItems = questionList.querySelectorAll('.list-group-item');
        listItems.forEach(item => item.classList.remove('selected'));
        listItems[index].classList.add('selected');

        const selectedQuestion = questions[index];

        console.log('Selected Question:', selectedQuestion);

        if (selectedQuestion.questionType === 'Fillups') {
            answerArea.innerHTML = `
          <div class="card">
          <div class="card-body">
              <h5 class="card-title">${selectedQuestion.questionId}</h5>
              <p class="card-text">${selectedQuestion.questionText}</p>
              <div class="mb-3">
              <input type="text" class="form-control" id="fillInput" placeholder="Enter your answer" required>
              </div>
              <button class="btn btn-primary" id="save-answer-btn">Save Answer</button>
          </div>
          </div>
      `;
        } else if (selectedQuestion.questionType === 'MultipleChoice') {
            if (selectedQuestion.options && Array.isArray(selectedQuestion.options)) {
                let optionsHTML = selectedQuestion.options.map(option => `
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
              <h5 class="card-title">${selectedQuestion.questionId}</h5>
              <p class="card-text">${selectedQuestion.questionText}</p>
              <form id="mcqForm">${optionsHTML}</form>
              <button class="btn btn-primary mt-3" id="save-answer-btn">Save Answer</button>
              </div>
          </div>
          `;
            } else {
                answerArea.innerHTML = `<p>No options available for this question.</p>`;
            }
        }

        // SAVE ANSWER
        const saveAnswerBtn = document.getElementById('save-answer-btn');
        if (saveAnswerBtn) {
            saveAnswerBtn.addEventListener('click', function () {
                saveAnswer(index);
            });
        }

        //PREVIOUS AND NEXT BUTTON
        if (currentIndex === 0) {
            previousBtn.disabled = true;
        } else {
            previousBtn.disabled = false;
        }

        if (currentIndex === questions.length - 1) {
            nextBtn.disabled = true;
        } else {
            nextBtn.disabled = false;
        }
    }

    //PREVIOUS BUTTON CLICK
    previousBtn.addEventListener('click', function () {
        if (currentIndex > 0) {
            showQuestion(currentIndex - 1);
        }
    });

    //NEXT BUTTON CLICK
    nextBtn.addEventListener('click', function () {
        if (currentIndex < questions.length - 1) {
            showQuestion(currentIndex + 1);
        }
    });

    let Answers = {};
    //SAVE ANSWER
    function saveAnswer(index) {
        const selectedQuestion = questions[index];
        let selectedAnswer = null;

        if (selectedQuestion.questionType === 'Fillups') {
            selectedAnswer = document.getElementById('fillInput').value;
            if(selectedAnswer===""){
                alert("Please Enter the Answer to save");
                return;
            }
        } else if (selectedQuestion.questionType === 'MultipleChoice') {
            const selectedOption = document.querySelector('input[name="mcqOptions"]:checked');
            if (selectedOption) {
                console.log(selectedOption);
                selectedAnswer = selectedOption.value;
            }
            else{
                alert("Select the Answe to Save");
            }
        }
        Answers[selectedQuestion.questionId] = selectedAnswer;
        console.log(Answers);
        alert(selectedAnswer);
    }
});
