
const token = localStorage.getItem('token');
if (!token) {
    window.location.href = '/Home/Home.html'
}


document.addEventListener('DOMContentLoaded', function () {
    const logoutButton = document.getElementById('logoutbtn');
    const logoutModal = new bootstrap.Modal(document.getElementById('logoutModal'));
    const confirmLogoutButton = document.getElementById('confirmLogoutButton');

    const params = new URLSearchParams(window.location.search);
    const token = localStorage.getItem('token');
    const responsesContainer = document.getElementById('responses-container');
    const quizIdInput = document.getElementById('quizIdInput');
    const searchButton = document.getElementById('searchButton');
    const paginationContainer = document.getElementById('pagination-container'); 
    const quizzesPerPage = 5; 
    let allResponses = []; 
    let currentPage = 1; 

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


    // Function to fetch all responses and populate initially
    function fetchAndPopulateResponses() {
        fetch('http://localhost:5273/api/QuizAttempt/GetAllUserResponses', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        })
        .then(response => response.json())
        .then(data => {
            // console.log('Fetched responses:', data); 
            allResponses = data; 
            displayResponses(allResponses.slice(0, quizzesPerPage)); 
            renderPagination(allResponses.length, quizzesPerPage, currentPage);
        })
        .catch(error => console.error('Error fetching data:', error));
    }

    // Function to display responses in the container
    function displayResponses(responses) {
        responsesContainer.innerHTML = ''; 
        responses.forEach(response => {
            const card = document.createElement('div');
            card.className = 'card responses-card response-card';
            card.innerHTML = `
                <div class="card-header text-center mb-4 d-flex justify-content-center align-items-center">
                    <div id="lottie-animation" style="width: 100px; height: 100px;">
                        <lottie-player
                            src="https://lottie.host/67a1e489-faf7-40f1-b9fc-fd124a10512f/AlRiUBPX5B.json"
                            speed="1" style="width: 100%; height: auto;" loop autoplay direction="1"
                            mode="normal"></lottie-player>    
                    </div>
                    <span class="h4 ml-2">Your Responses</span>
                </div>
                <div class="card-body">
                    <div class="d-flex justify-content-center text-right mb-3">
                        <div class="container">
                            <h5 class="card-title text-center">Quiz ID: ${response.quizId}</h5>
                            <p class="card-text d-flex flex-wrap flex-column flex-sm-row justify-content-between word-wrap-custom"><strong>User ID:</strong> ${response.userId}</p>
                            <p class="card-text d-flex flex-wrap flex-column flex-sm-row justify-content-between word-wrap-custom"><strong>Score:</strong> ${response.score}</p>
                            <p class="card-text d-flex flex-wrap flex-column flex-sm-row justify-content-between word-wrap-custom"><strong>Time Taken:</strong> ${response.timeTaken}</p>
                            <p class="card-text d-flex flex-wrap flex-column flex-sm-row justify-content-between word-wrap-custom"><strong>Start Time:</strong> ${response.startTime}</p>
                            <p class="card-text d-flex flex-wrap flex-column flex-sm-row justify-content-between word-wrap-custom"><strong>End Time:</strong> ${response.endTime}</p>
                        </div>
                    </div>
                    <h5>Answered Questions</h5>
                    <ul class="list-group">
                        ${response.answeredQuestions.map(question => `
                            <li class="list-group-item question-item">
                                <div>
                                    <strong>Question ID:</strong> ${question.questionId}<br>
                                    <strong>Submitted Answer:</strong> ${question.submittedAnswer}<br>
                                    <strong>Correct Answer:</strong> ${question.correctAnswer}
                                </div>
                                <span class="${question.isCorrect ? 'badge badge-correct' : 'badge badge-incorrect'}">
                                    ${question.isCorrect ? '✔ Correct' : '✘ Incorrect'}
                                </span>
                            </li>
                        `).join('')}
                    </ul>
                </div>
            `;
            responsesContainer.appendChild(card);
        });
    }

    // Function to render pagination
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
    
        paginationContainer.addEventListener('click', function (e) {
            e.preventDefault();
            if (e.target.classList.contains('page-link')) {
                const page = parseInt(e.target.getAttribute('data-page'));
                currentPage = page;
                const start = (page - 1) * quizzesPerPage;
                const end = start + quizzesPerPage;
                displayResponses(allResponses.slice(start, end));
                renderPagination(totalQuizzes, quizzesPerPage, currentPage);
            }
        });
    };
    

    // Load all responses initially
    fetchAndPopulateResponses();

    // Event listener for the search button
    searchButton.addEventListener('click', function () {
        const quizIdInputValue = quizIdInput.value.trim();

        if (!quizIdInputValue) {
            displayResponses(allResponses.slice(0, quizzesPerPage));
            renderPagination(allResponses.length, quizzesPerPage, 1);
            return;
        }

        const quizId = parseInt(quizIdInputValue);

        if (isNaN(quizId)) {
            alert('Please enter a valid QuizID.');
            return;
        }

        const filteredResponses = allResponses.filter(response => response.quizId === quizId);

        displayResponses(filteredResponses.slice(0, quizzesPerPage));
        renderPagination(filteredResponses.length, quizzesPerPage, 1);
    });
});
