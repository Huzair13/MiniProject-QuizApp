document.addEventListener('DOMContentLoaded', function () {
    const params = new URLSearchParams(window.location.search);
    const token = localStorage.getItem('token');
    const responsesContainer = document.getElementById('responses-container');
    const quizIdInput = document.getElementById('quizIdInput');
    const searchButton = document.getElementById('searchButton');
    let allResponses = []; // To store all fetched responses

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
                console.log('Fetched responses:', data); // Log fetched data for debugging
                allResponses = data; // Store all responses locally
                displayResponses(allResponses); // Display all responses initially
            })
            .catch(error => console.error('Error fetching data:', error));
    }

    // Function to display responses in the container
    function displayResponses(responses) {
        responsesContainer.innerHTML = ''; // Clear previous results
        responses.forEach(responses => {
            const card = document.createElement('div');
            card.className = 'card responses-card response-card';
            card.innerHTML = `
                <div class="card-header">
                    Your Responses
                </div>
                <div class="card-body">
                    <h5 class="card-title">Quiz ID: ${responses.quizId}</h5>
                    <p class="card-text">
                        <strong>User ID:</strong> ${responses.userId}<br>
                        <strong>Score:</strong> ${responses.score}<br>
                        <strong>Time Taken:</strong> ${responses.timeTaken}<br>
                        <strong>Start Time:</strong> ${responses.startTime}<br>
                        <strong>End Time:</strong> ${responses.endTime}<br>
                    </p>
                    <h5>Answered Questions</h5>
                    <ul class="list-group">
                        ${responses.answeredQuestions.map(question => `
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

    // Load all responses initially
    fetchAndPopulateResponses();

    // Event listener for the search button
    // Event listener for the search button
    searchButton.addEventListener('click', function () {
        const quizIdInputValue = quizIdInput.value.trim();

        if (!quizIdInputValue) {
            displayResponses(allResponses);
            return;
        }

        const quizId = parseInt(quizIdInputValue);

        if (isNaN(quizId)) {
            alert('Please enter a valid QuizID.');
            return;
        }

        const filteredResponses = allResponses.filter(response => response.quizId === quizId);

        displayResponses(filteredResponses);
    });

});
