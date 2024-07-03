const quizzesPerPage = 6;
let currentPage = 1;

const token = localStorage.getItem('token');

const renderQuizzes = (quizzesToRender) => {
    const quizGrid = document.getElementById('quiz-grid');
    quizGrid.innerHTML = '';

    quizzesToRender.forEach(quiz => {
        const col = document.createElement('div');
        col.className = 'col-lg-4 col-md-6 mb-4';

        const randomImageNumber = Math.floor(Math.random() * 10) + 1;
        const imageUrl = `/Assets/Images/Quiz/Quiz${randomImageNumber}.jpg`;

        col.innerHTML = `
            <div class="card border-0 shadow-sm">
                <img src="${imageUrl}" class="card-img-top rounded-top quiz-image" alt="${quiz.quizName} Image">
                <div class="card-body">
                    <h5 class="card-title text-center">${quiz.quizName}</h5>
                    <p class="card-text text-center">${quiz.quizDescription}</p>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item d-flex justify-content-between"><strong>Number of Questions:</strong> ${quiz.numOfQuestions}</li>
                        <li class="list-group-item d-flex justify-content-between"><strong>Points:</strong> ${quiz.totalPoints}</li>
                        <li class="list-group-item d-flex justify-content-between"><strong>Time Limit:</strong> ${quiz.timelimit} minutes</li>
                    </ul>
                </div>
                <div class="card-footer bg-white border-top-0 text-right text-center">
                    <button class="btn btn-sm edit-quiz-btn edit-quiz-button-design mb-2" data-quiz-id="${quiz.quizId}">Edit Quiz</button>
                    <button class="btn btn-sm del-quiz-btn edit-quiz-button-design" data-quiz-id="${quiz.quizId}">Delete</button>
                </div>
            </div>
        `;

        quizGrid.appendChild(col);
    });
    document.querySelectorAll('.edit-quiz-btn').forEach(button => {
        button.addEventListener('click', function () {
            const quizId = parseInt(this.getAttribute('data-quiz-id'));
            window.location.href = `/EditQuiz/EditQuiz.html?quizID=${quizId}`;
        });
    });

    document.querySelectorAll('.del-quiz-btn').forEach(button => {
        button.addEventListener('click', async function () {
            const quizId = parseInt(this.getAttribute('data-quiz-id'));
            const confirmation = confirm("Are you sure you want to delete this question?");

            if(confirmation){
                try {
                    const response = await fetch('http://localhost:5273/api/Quiz/SoftDeleteQuiz', {
                        method: 'DELETE',
                        headers: {
                            'content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`
                        },
                        body: JSON.stringify(
                            {
                                quizId: quizId
                            }
                        )
                    })
    
                    if (!response.ok) {
                        if (!response.ok) {
                            const errorData = await response.json();
                            throw new Error(errorData.message || 'Error Occured');
                        }
                    }
                    const data = await response.json();
                    console.log(data);
                    window.location.reload();
                }
                catch (error) {
                    console.error('Error While Deleting quizzes:', error);
                }
            }
            else{
                alert("cancelled");
            }

        })
    })

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

const renderPagination = (totalQuizzes, quizzesPerPage, currentPage) => {
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
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const page = parseInt(this.getAttribute('data-page'));
            currentPage = page;
            const start = (page - 1) * quizzesPerPage;
            const end = start + quizzesPerPage;
            renderQuizzes(quizzes.slice(start, end));
            renderPagination(totalQuizzes, quizzesPerPage, currentPage);
        });
    });
};

let quizzes = [];

const sortAZBtn = document.getElementById('sort-az');
const sortZABtn = document.getElementById('sort-za');

document.addEventListener('DOMContentLoaded', () => {
    let originalQuizzes = [];
    const apiUrl = 'http://localhost:5273/api/ViewQuiz/GetQuizzesBySpecificTeacher';
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
            originalQuizzes = data;
            quizzes = originalQuizzes.slice(0);
            console.log(quizzes);
            document.getElementById('pagination-container').style.display = 'block';
            renderQuizzes(quizzes.slice(0, quizzesPerPage));
            renderPagination(quizzes.length, quizzesPerPage, 1);

        })
        .catch(error => {
            console.error('Error fetching quizzes:', error);
        });

    document.getElementById('search-button').addEventListener('click', () => {
        const quizId = document.getElementById('quiz-id-input').value.trim();

        if (quizId === '') {
            quizzes = originalQuizzes.slice(0);
        } else {
            quizzes = originalQuizzes.filter(quiz => quiz.quizId.toString() === quizId);
        }

        currentPage = 1;
        renderQuizzes(quizzes.slice(0, quizzesPerPage));
        renderPagination(quizzes.length, quizzesPerPage, 1);
    });

    document.getElementById('filter-button').addEventListener('click', () => {
        const quizType = document.getElementById('quizType').value;
        const isMultipleAllowed = document.getElementById('isMultipleAllowed').value;

        quizzes = originalQuizzes.filter(quiz => {
            let matchesType = true;
            let matchesMultiple = true;

            if (quizType !== '') {
                matchesType = quiz.quizType === quizType;
            }

            if (isMultipleAllowed !== '') {
                matchesMultiple = quiz.isMultpleAllowed.toString() === isMultipleAllowed;
            }

            return matchesType && matchesMultiple;
        });

        currentPage = 1;
        renderQuizzes(quizzes.slice(0, quizzesPerPage));
        renderPagination(quizzes.length, quizzesPerPage, 1);
    });

    function sortQuizzes(order) {
        if (order === 'az') {
            quizzes.sort((a, b) => a.quizName.toLowerCase().localeCompare(b.quizName.toLowerCase()));
        } else if (order === 'za') {
            quizzes.sort((a, b) => b.quizName.toLowerCase().localeCompare(a.quizName.toLowerCase()));
        }
        currentPage = 1;
        renderQuizzes(quizzes.slice(0, quizzesPerPage));
        renderPagination(quizzes.length, quizzesPerPage, 1);
    }

    sortAZBtn.addEventListener('click', () => {
        sortQuizzes('az');
    });

    sortZABtn.addEventListener('click', () => {
        sortQuizzes('za');
    });
});
