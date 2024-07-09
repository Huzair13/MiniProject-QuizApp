const questionsPerPage = 6;
let currentPage = 1;

const token = localStorage.getItem('token');
if (!token) {
    window.location.href = '/Home/Home.html'
}


const renderQuestions = (questionsToRender) => {
    const questionGrid = document.getElementById('quiz-grid');
    questionGrid.innerHTML = '';

    questionsToRender.forEach(question => {
        const col = document.createElement('div');
        col.className = 'col-lg-4 col-md-6 mb-4';

        const options = ['Math', 'Science', 'History', 'Literature', 'Geography', 'Art', 'Music', 'Technology', 'Sports'];

        let imageUrl;
        if (question.category && options.includes(question.category)) {
            imageUrl = `/Assets/Images/Question/${question.category}.jpg`;
        } else {
            imageUrl = `/Assets/Images/Question/General.jpg`;
        }

        col.innerHTML = `
            <div class="card border-0 shadow-sm">
                <img src="${imageUrl}" class="card-img-top rounded-top quiz-image" alt="${question.questionText} Image">
                <div class="card-body">
                    <h5 class="card-title text-center">${question.questionText}</h5>
                    <p class="card-text text-center">${question.questionType}</p>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item d-flex flex-wrap flex-column flex-sm-row justify-content-between word-wrap-custom"><strong>ID:</strong> ${question.id}</li>
                        <li class="list-group-item d-flex flex-wrap flex-column flex-sm-row justify-content-between word-wrap-custom"><strong>Category:</strong> ${question.category}</li>
                        <li class="list-group-item d-flex flex-wrap flex-column flex-sm-row justify-content-between word-wrap-custom"><strong>Points:</strong> ${question.points}</li>
                        <li class="list-group-item d-flex flex-wrap flex-column flex-sm-row justify-content-between word-wrap-custom"><strong>Difficulty Level:</strong> ${question.difficultyLevel}</li>
                    </ul>
                </div>
                <div class="card-footer bg-white border-top-0 text-right text-center">
                    <button class="btn mb-1 restore-question-button-design btn-sm undo-delete-btn" data-quiz-id="${question.id}">Restore</button>
                    <button class="btn restore-question-button-design btn-sm permanent-delete-btn" data-quiz-id="${question.id}">Delete Permanently</button>
                </div>
            </div>
        `;
        questionGrid.appendChild(col);
    });

    document.querySelectorAll('.permanent-delete-btn').forEach(button => {
        button.addEventListener('click', async function () {
            const questionID = parseInt(this.getAttribute('data-quiz-id'));
            const confirmation = confirm("Are you sure you want to Permanently this question?");
            if (confirmation) {
                try {
                    const response = await fetch(`http://localhost:5273/api/Question/DeleteQuestionByID?questionId=${questionID}`, {
                        method: 'DELETE',
                        headers: {
                            'content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`
                        }
                    })

                    if (!response.ok) {
                        if (!response.ok) {
                            const errorData = await response.json();
                            throw new Error(errorData.message || 'Error Occured');
                        }
                    }
                    const data = await response.json();
                    window.location.reload();
                }
                catch (error) {
                    console.error('Error While Deleting quizzes:', error);
                }
            }
            else {
                alert("cancelled");
            }
        });
    });

    document.querySelectorAll('.undo-delete-btn').forEach(button => {
        button.addEventListener('click', async function () {
            const questionID = parseInt(this.getAttribute('data-quiz-id'));
            const confirmation = confirm("Are you sure you want to Restore this question?");
            if (confirmation) {
                try {
                    const response = await fetch(`http://localhost:5273/api/Question/UndoSoftDelete?QuestionID=${questionID}`, {
                        method: 'POST',
                        headers: {
                            'content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`
                        }
                    })

                    if (!response.ok) {
                        if (!response.ok) {
                            const errorData = await response.json();
                            throw new Error(errorData.message || 'Error Occured');
                        }
                    }
                    const data = await response.json();
                    window.location.reload();
                }
                catch (error) {
                    console.error('Error While Deleting quizzes:', error);
                }
            }
            else {
                alert("cancelled");
            }
        });
    });

};

// QUIZ IMAGE
const style = document.createElement('style');
style.textContent = `
    .quiz-image {
        width: 100%;
        height: 200px; 
        object-fit: cover;
    }
`;
document.head.appendChild(style);

const renderPagination = (totalQuizzes, questionsPerPage, currentPage) => {
    const paginationContainer = document.querySelector('.pagination');
    paginationContainer.innerHTML = '';
    const pageCount = Math.ceil(totalQuizzes / questionsPerPage);

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
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const page = parseInt(this.getAttribute('data-page'));
            currentPage = page;
            const start = (page - 1) * questionsPerPage;
            const end = start + questionsPerPage;
            renderQuestions(questions.slice(start, end));
            renderPagination(totalQuizzes, questionsPerPage, currentPage);
        });
    });
};

let questions = [];

const sortAZBtn = document.getElementById('sort-az');
const sortZABtn = document.getElementById('sort-za');

document.addEventListener('DOMContentLoaded', () => {
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


    let originalQuestions = [];
    const apiUrl = 'http://localhost:5273/api/ViewQuestion/GetAllSoftDeletedQuestion';
    fetch(apiUrl, {
        method: 'GET',
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
            originalQuestions = data;
            questions = originalQuestions.slice(0);
            // console.log(questions);
            document.getElementById('pagination-container').style.display = 'block';
            renderQuestions(questions.slice(0, questionsPerPage));
            renderPagination(questions.length, questionsPerPage, 1);

        })
        .catch(error => {
            console.error('Error fetching quizzes:', error);
        });

    document.getElementById('search-button').addEventListener('click', () => {
        const query = document.getElementById('question-text-input').value.trim().toLowerCase();

        if (query === '') {
            questions = originalQuestions.slice(0);
        } else {
            questions = originalQuestions.filter(quiz => quiz.questionText.toLowerCase().includes(query));
        }

        currentPage = 1;
        renderQuestions(questions.slice(0, questionsPerPage));
        renderPagination(questions.length, questionsPerPage, 1);
    });

    document.getElementById('filter-button').addEventListener('click', () => {
        const questionCategory = document.getElementById('questionCategory').value;
        const questionType = document.getElementById('questionType').value;
        const difficultyLevel = document.getElementById('difficultyLevel').value;

        // console.log(questionType)
        // console.log(questionCategory)

        // console.log(difficultyLevel);

        const predefinedCategories = ["Math", "Science", "History", "Literature", "Geography", "Art", "Music", "Technology", "Sports"];


        questions = originalQuestions.filter(question => {
            let matchesCategory = true;
            let matchesType = true;
            let matchesDifficultyLevel = true;

            if (questionType !== '') {
                matchesType = question.questionType === questionType;
            }

            if (difficultyLevel !== '') {
                matchesDifficultyLevel = question.difficultyLevel === parseInt(difficultyLevel);
            }

            if (questionCategory !== '') {
                if (questionCategory === "Other") {
                    matchesCategory = !predefinedCategories.includes(question.category);
                } else {
                    matchesCategory = question.category === questionCategory;
                }
            }
            return matchesCategory && matchesType && matchesDifficultyLevel;
        });

        currentPage = 1;
        renderQuestions(questions.slice(0, questionsPerPage));
        renderPagination(questions.length, questionsPerPage, 1);
    });

    function sortQuestions(order) {
        if (order === 'az') {
            questions.sort((a, b) => a.questionText.toLowerCase().localeCompare(b.questionText.toLowerCase()));
        } else if (order === 'za') {
            questions.sort((a, b) => b.questionText.toLowerCase().localeCompare(a.questionText.toLowerCase()));
        }
        currentPage = 1;
        renderQuestions(questions.slice(0, questionsPerPage));
        renderPagination(questions.length, questionsPerPage, 1);
    }

    sortAZBtn.addEventListener('click', () => {
        sortQuestions('az');
    });

    sortZABtn.addEventListener('click', () => {
        sortQuestions('za');
    });
});
