document.addEventListener('DOMContentLoaded', function() {
    const QuizNameInput = document.getElementById('quizName');
    const QuizDescriptionInput = document.getElementById('quizDescription');
    const searchQueryInput = document.getElementById('search-query');
    const searchButton = document.getElementById('search-button');
    const sortAZBtn = document.getElementById('sort-az');
    const sortZABtn = document.getElementById('sort-za');

    const questionCategoryInput = document.getElementById('questionCategory');
    const questionTypeInput = document.getElementById('questionType');
    const difficultyLevelInput = document.getElementById('difficultyLevel');
    const filterButton = document.getElementById('filter-button');

    let currentPage = 1;
    const perPage = 8; 
    let totalQuestions = 0; 
    const token = localStorage.getItem('token');
    let questions = [];
    let selectedQuestions = []; 
    let filteredQuestions = [];

    //INITIAL FETCH 
    fetchQuestions();

    //VALIDATION -- ON SUBMIT
    const forms = document.querySelectorAll('.needs-validation');
    forms.forEach(form => {
        form.addEventListener('submit', function(event) {
            event.preventDefault(); 

            if (!form.checkValidity()) {
                event.preventDefault(); 
                event.stopPropagation();
            }
            else if (QuizDescriptionInput.value === '') {
                validateInput(QuizDescriptionInput);
            }  
            else if (selectedQuestions.length === 0) {
                alert('Please select at least one question.');
                return;
            } 
            else {
                event.preventDefault(); 
                createQuiz();
            }

            form.classList.add('was-validated');
        });
    });

    //VALIDATION ON ENTERING THE INPUT
    // const inputs = document.querySelectorAll('input, select, textarea');
    const inputsToValidate = document.querySelectorAll('input:not(.filter-input), select:not(.filter-input), textarea:not(.filter-input)');

    inputsToValidate.forEach(input => {
        input.addEventListener('input', function() {
            validateInput(input);
        });
    });


    //VALIDATE THE INPUT
    function validateInput(input) {
        if (input.checkValidity()) {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
        } else {
            input.classList.remove('is-valid');
            input.classList.add('is-invalid');
        }
    }

    //SEARCH QUESTION FUNCTIONALITY
    function SortQuestions(query) {
        console.log(query);
        if (query) {
            console.log("if reached");
            filteredQuestions = questions.filter(question => question.questionText.toLowerCase().includes(query.toLowerCase()));
            console.log(filteredQuestions);
        } else {
            filteredQuestions = questions;
        }
        totalPages = Math.ceil(filteredQuestions.length / perPage);
        currentPage = 1;
        renderPagination(filteredQuestions, filteredQuestions.length, perPage, currentPage);
        renderQuestions(filteredQuestions.slice(0, perPage));
    }

    //SEARCH BUTTON
    searchButton.addEventListener('click', () => {
        console.log(questions);
        const query = searchQueryInput.value.trim();
        SortQuestions(query);
    });

    //SORT FUNCTIONALITY
    function sortQuestions(order) {
        if (filteredQuestions.length === 0) {
            filteredQuestions = questions;
        }
        if (order === 'az') {
            filteredQuestions.sort((a, b) => {
                const QuestionA = a.questionText.toLowerCase();
                const QuestionB = b.questionText.toLowerCase();
                return QuestionA.localeCompare(QuestionB);
            });
        } else if (order === 'za') {
            filteredQuestions.sort((a, b) => {
                const QuestionA = a.questionText.toLowerCase();
                const QuestionB = b.questionText.toLowerCase();
                return QuestionB.localeCompare(QuestionA);
            });
        }
        currentPage = 1;
        renderPagination(filteredQuestions, filteredQuestions.length, perPage, currentPage);
        renderQuestions(filteredQuestions.slice(0, perPage));
    }

    //SORT BUTTONS
    sortAZBtn.addEventListener('click', () => {
        sortQuestions('az');
    });

    sortZABtn.addEventListener('click', () => {
        sortQuestions('za');
    });

// FILTER FUNCTIONALITY
function filterQuestions(category, type, difficulty) {
    console.log("Filter Criteria:", category, type, difficulty);
    filteredQuestions = questions.filter(question => {
        console.log("Question Data:", question.category, question.questionType, question.difficultyLevel);
        const difficultyLevelInt = {
            "Easy": 0,
            "Medium": 1,
            "Hard": 2
        }[difficulty];

        return (category === "" || question.category === category) &&
               (type === "" || question.questionType === type) &&
               (difficulty === "" || question.difficultyLevel === difficultyLevelInt);
    });

    console.log("Filtered Questions:", filteredQuestions); 
    totalPages = Math.ceil(filteredQuestions.length / perPage);
    currentPage = 1;
    renderPagination(filteredQuestions, filteredQuestions.length, perPage, currentPage);
    renderQuestions(filteredQuestions.slice(0, perPage));
}
    
    //FILTER BUTTONS
    filterButton.addEventListener('click', (event) => {
        event.preventDefault();

        const category = questionCategoryInput.value;
        const type = questionTypeInput.value;
        const difficulty = difficultyLevelInput.value;
        filterQuestions(category, type, difficulty);
    });


    //FETCH QUESTIONS
    function fetchQuestions() {
        const apiUrl = `http://localhost:5273/api/ViewQuestion/GetAllQuestions`;
    
        fetch(apiUrl, {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            questions = data;
            totalQuestions = data.total;
            filteredQuestions = questions; 
            renderQuestions(questions.slice(0, perPage));
            renderPagination(questions, questions.length, perPage, currentPage);
        })
        .catch(error => {
            console.error('Error fetching questions:', error);
        });
    }
    
    //RENDER QUESTIONS
    function renderQuestions(questions) {
        const questionGrid = document.getElementById('questionGrid');
        questionGrid.innerHTML = '';
    
        if (!Array.isArray(questions)) {
            console.error('Invalid questions data:', questions);
            return;
        }
        questions.forEach(question => {
            const questionCard = document.createElement('div');
            questionCard.className = 'question-card';
            questionCard.classList.add('shadow')
            const isChecked = selectedQuestions.includes(question.id); 
            questionCard.innerHTML = `
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" id="question${question.id}" value="${question.id}" ${isChecked ? 'checked' : ''} onchange="updateSelectedQuestions(${question.id}, this.checked)">
                    <label class="form-check-label" for="question${question.id}">${question.questionText}</label>
                </div>
            `;
            questionGrid.appendChild(questionCard);
        });
    }

    //RENDER PAGINATION
    const renderPagination = (PaginationQuestions, totalQuizzes, quizzesPerPage, currentPage) => {
        const paginationContainer = document.querySelector('.pagination');
        paginationContainer.innerHTML = '';
        const pageCount = Math.ceil(totalQuizzes / quizzesPerPage);
    
        if (currentPage > 1) {
            const prevItem = document.createElement('li');
            prevItem.className = 'page-item';
            prevItem.innerHTML = `<a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a>`;
            paginationContainer.appendChild(prevItem);
        }
    
        const startPage = Math.max(1, currentPage - 1);
        const endPage = Math.min(pageCount, currentPage + 1);
    
        for (let i = startPage; i <= endPage; i++) {
            const pageItem = document.createElement('li');
            pageItem.className = `page-item ${i === currentPage ? 'active' : ''}`;
            pageItem.innerHTML = `<a class="page-link" href="#" data-page="${i}">${i}</a>`;
            paginationContainer.appendChild(pageItem);
        }
    
        if (currentPage < pageCount) {
            const nextItem = document.createElement('li');
            nextItem.className = 'page-item';
            nextItem.innerHTML = `<a class="page-link" href="#" data-page="${currentPage + 1}">Next</a>`;
            paginationContainer.appendChild(nextItem);
        }
    
        document.querySelectorAll('.page-link').forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                const page = parseInt(this.getAttribute('data-page'));
                currentPage = page;
                const start = (page - 1) * quizzesPerPage;
                const end = start + quizzesPerPage;
                renderQuestions(PaginationQuestions.slice(start, end));
                renderPagination(PaginationQuestions, totalQuizzes, quizzesPerPage, currentPage);
            });
        });
    };
    
    //CREATE QUIZ
    function createQuiz() {
        const quizName = QuizNameInput.value;
        const quizDescription = QuizDescriptionInput.value;
        const quizType = document.getElementById('quizType').value;
        const isMultipleAttemptAllowed = document.getElementById('isMultipleAttemptAllowed').checked;
        const timeLimit = parseInt(document.getElementById('timeLimit').value);

        let timelimitString =""

        if(timeLimit<10){
            timelimitString =`00:0${timeLimit}:00`;
        }
        else{
            timelimitString = `00:${timeLimit}:00`;
        }
        
        console.log(selectedQuestions); 

        //QUIZ JSON DATA
        const quizData = {
            quizName: quizName,
            quizDescription: quizDescription,
            quizType: quizType,
            isMultipleAttemptAllowed: isMultipleAttemptAllowed,
            timeLimit: timelimitString,
            questionIds: selectedQuestions
        };

        const apiUrl = 'http://localhost:5273/api/Quiz/AddQuiz';
        fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(quizData)
        })
        .then(response => {
            console.log(quizData);
            console.log(response);
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            console.log('Quiz created successfully:', data);
            document.getElementById('quizForm').reset(); 
            selectedQuestions = []; 
            renderQuestions(questions.slice(0, perPage)); 
            renderPagination(questions, totalQuestions, perPage, currentPage);
        })
        .catch(error => {
            console.error('Error creating quiz:', error);
        });
    }

    //UPDATE CHECKED QUESTIONS
    window.updateSelectedQuestions = function(questionId, isChecked) {
        if (isChecked) {
            selectedQuestions.push(questionId);
        } else {
            const index = selectedQuestions.indexOf(questionId);
            if (index !== -1) {
                selectedQuestions.splice(index, 1);
            }
        }
    }

});
