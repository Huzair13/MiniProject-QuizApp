const params = new URLSearchParams(window.location.search);
const QuizId = params.get('quizID');
const token = localStorage.getItem('token');

let allQuestions = [];

let quizData = {};
async function fetchQuizDetails() {
    try {
        const response = await fetch(`http://localhost:5273/api/ViewQuiz/GetQuizByQuizID?quizID=${QuizId}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        if (response.ok) {
            console.log(response)
            quizData = await response.json();
            console.log("Hello", quizData, "Welcome")
        } else {
            console.error('Failed to fetch quiz details');
        }
    } catch (error) {
        console.error('Error fetching quiz details:', error);
    }
}
async function fetchQuestions() {
    try {
        const response = await fetch(`http://localhost:5273/api/ViewQuestion/GetAllQuestions`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        if (response.ok) {
            allQuestions = await response.json();
            console.log(allQuestions)
        } else {
            console.error('Failed to fetch questions');
        }
    } catch (error) {
        console.error('Error fetching questions:', error);
    }
}

async function fetchData() {
    await fetchQuestions();
    await fetchQuizDetails();
    console.log("After fetch, All Questions:", allQuestions);
    console.log("After fetch, Quiz Data:", quizData);

}

document.addEventListener('DOMContentLoaded', async function () {

    await fetchData();
    console.log(allQuestions)
    console.log(quizData)

    const params = new URLSearchParams(window.location.search);
    const QuizId = params.get('quizID');
    const token = localStorage.getItem('token');

    console.log("Heloo ",QuizId)

    if (!QuizId) {
        window.location.href = '/ViewAllQuizzes/ViewAllQuiz.html';
        return;
    }

    const questionGrid = document.getElementById('questionGrid');
    const addQuestionButton = document.getElementById('addQuestionButton');
    const questionSection = document.getElementById('questionSection');
    const addQuestionsBtn = document.getElementById('btnAddQuestions');

    const prevPageBtn = document.getElementById('prev-page');
    const nextPageBtn = document.getElementById('next-page');
    const pagination = document.getElementById('pagination');
    const searchQueryInput = document.getElementById('search-query');
    const searchButton = document.getElementById('search-button');

    const questionCategoryInput = document.getElementById('questionCategory');
    const questionTypeInput = document.getElementById('questionType');
    const difficultyLevelInput = document.getElementById('difficultyLevel');
    const filterButton = document.getElementById('filter-button');


    let totalPages = 1;
    let filteredQuestions = [];

    const questionsPerPage = 5;
    let currentPage = 1;

    let selectedQuestions = [];


    document.getElementById('quizName').textContent = quizData.quizName;
    document.getElementById('quizDescription').textContent = quizData.quizDescription;
    document.getElementById('quizType').textContent = quizData.quizType;
    document.getElementById('createdOn').textContent = new Date(quizData.createdOn).toLocaleString();
    document.getElementById('numOfQuestions').textContent = quizData.numOfQuestions;
    document.getElementById('quizCreatedBy').textContent = quizData.quizCreatedBy;
    document.getElementById('isMultpleAllowed').textContent = quizData.isMultpleAllowed ? 'Yes' : 'No';
    document.getElementById('totalPoints').textContent = quizData.totalPoints;
    document.getElementById('timelimit').textContent = quizData.timelimit;

    function renderQuestions(page) {
        questionGrid.innerHTML = '';
        const start = (page - 1) * questionsPerPage;
        const end = start + questionsPerPage;
        const paginatedQuestions = filteredQuestions.slice(start, end);

        paginatedQuestions.forEach(question => {
            const questionCard = document.createElement('div');
            questionCard.className = 'question-card';
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

    searchButton.addEventListener('click', () => {
        const query = searchQueryInput.value.trim();
        filterQuestions(query);
    });


    //FILTER BUTTONS
    filterButton.addEventListener('click', () => {
        const category = questionCategoryInput.value;
        const type = questionTypeInput.value;
        const difficulty = difficultyLevelInput.value;
        filterQuestions2(category, type, difficulty);
    });

    // FILTER FUNCTIONALITY
    function filterQuestions2(category, type, difficulty) {
        console.log("Filter Criteria:", category, type, difficulty);
        filteredQuestions = allQuestions.filter(question => {
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
        totalPages = Math.ceil(filteredQuestions.length / questionsPerPage);
        currentPage = 1;
        renderPagination();
        renderQuestions(currentPage);
    }

    function filterQuestions(query) {
        if (query) {
            filteredQuestions = allQuestions.filter(question => question.questionText.toLowerCase().includes(query.toLowerCase()));
        } else {
            filteredQuestions = allQuestions;
        }
        totalPages = Math.ceil(filteredQuestions.length / questionsPerPage);
        currentPage = 1;
        renderPagination();
        renderQuestions(currentPage);
    }

    function renderPagination() {
        while (pagination.children.length > 2) {
            pagination.removeChild(pagination.children[1]);
        }

        const startPage = Math.max(1, currentPage - 1);
        const endPage = Math.min(totalPages, currentPage + 1);

        for (let i = startPage; i <= endPage; i++) {
            const pageItem = document.createElement('li');
            pageItem.classList.add('page-item');
            if (i === currentPage) {
                pageItem.classList.add('active');
            }
            pageItem.id = `page-${i}`;
            const pageLink = document.createElement('a');
            pageLink.classList.add('page-link');
            pageLink.href = '#';
            pageLink.textContent = i;
            pageItem.appendChild(pageLink);
            pagination.insertBefore(pageItem, nextPageBtn);
        }

        const pageItems = document.querySelectorAll('.page-item');
        pageItems.forEach(item => {
            item.addEventListener('click', (e) => {
                e.preventDefault();
                const page = parseInt(e.target.textContent);
                if (!isNaN(page)) {
                    currentPage = page;
                    renderQuestions(currentPage);
                    renderPagination();
                    updatePagination();
                }
            });
        });

        updatePagination();
    }

    function updatePagination() {
        const pageItems = document.querySelectorAll('.page-item');
        pageItems.forEach(item => item.classList.remove('active'));
        const currentPageItem = document.getElementById(`page-${currentPage}`);
        if (currentPageItem) {
            currentPageItem.classList.add('active');
        }

        prevPageBtn.classList.toggle('disabled', currentPage === 1);
        nextPageBtn.classList.toggle('disabled', currentPage === totalPages);
    }

    window.updateSelectedQuestions = function (questionId, isChecked) {
        if (isChecked) {
            selectedQuestions.push(questionId);
        } else {
            const index = selectedQuestions.indexOf(questionId);
            if (index !== -1) {
                selectedQuestions.splice(index, 1);
            }
        }
    }

    function filterOutQuizQuestions() {
        filteredQuestions = allQuestions.filter(question => !quizData.quizQuestions.includes(question.id));
    }

    addQuestionButton.addEventListener('click', function () {
        filterOutQuizQuestions();
        questionSection.style.display = 'block';
        totalPages = Math.ceil(filteredQuestions.length / questionsPerPage);
        renderPagination();
        renderQuestions(currentPage);
    });

    prevPageBtn.addEventListener('click', (e) => {
        e.preventDefault();
        if (currentPage > 1) {
            currentPage--;
            renderQuestions(currentPage);
            renderPagination();
            updatePagination();
        }
    });

    nextPageBtn.addEventListener('click', (e) => {
        e.preventDefault();
        if (currentPage < totalPages) {
            currentPage++;
            renderQuestions(currentPage);
            renderPagination();
            updatePagination();
        }
    });

    addQuestionsBtn.addEventListener('click', async function () {
        console.log(selectedQuestions);
        const data = {
            quizId: QuizId,
            questionIds: selectedQuestions
        }
        await addSelectQuestions(JSON.stringify(data));
    });

    async function addSelectQuestions(addedQuestions) {
        console.log(addedQuestions)
        try {
            const response = await fetch(`http://localhost:5273/api/Quiz/AddQuestions`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: addedQuestions
            });
            if (response.ok) {
                alert("Questions Added SuccessFully");
                window.location.reload();
            } else {
                console.error('Failed to fetch questions');
            }
        } catch (error) {
            console.error('Error fetching questions:', error);
        }
    }

});