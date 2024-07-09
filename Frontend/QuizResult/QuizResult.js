const params = new URLSearchParams(window.location.search);
const QuizId = params.get('quizID');

const token = localStorage.getItem('token');
if (!token) {
    window.location.href = '/Home/Home.html'
}

// console.log(QuizId);
// console.log(token);
document.addEventListener('DOMContentLoaded', function () {
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


    fetch('http://localhost:5273/api/QuizAttempt/checkResult', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(QuizId)
    })
        .then(response => response.json())
        .then(data => {
            // console.log(data);
            const resultsContainer = document.getElementById('results-container');
            data.forEach(result => {
                const card = document.createElement('div');
                card.className = 'card result-card response-card';
                card.innerHTML = `
                            <div class="card-header text-center mb-4 d-flex justify-content-center align-items-center">
                                <div id="lottie-animation" style="width: 100px; height: 100px;">
                                    <lottie-player
                                        src="https://lottie.host/67a1e489-faf7-40f1-b9fc-fd124a10512f/AlRiUBPX5B.json"
                                        speed="1" style="width: 100%; height: auto;" loop autoplay direction="1"
                                        mode="normal"></lottie-player>    
                                </div>
                                <span class="h4 ml-2">Quiz Result</span>
                            </div>
                            <div class="card-body">
                                <div class="d-flex justify-content-center text-right mb-3">
                                    <div>
                                        <h5 class="card-title text-center">Quiz ID: ${result.quizId}</h5>
                                        <p class="card-text">
                                            <strong>User ID:</strong> ${result.userId}<br>
                                            <strong>Score:</strong> ${result.score}<br>
                                            <strong>Time Taken:</strong> ${result.timeTaken}
                                        </p>
                                    </div>
                                </div>
                                <h5>Answered Questions</h5>
                                <ul class="list-group">
                                    ${result.answeredQuestions.map(question => `
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
                resultsContainer.appendChild(card);
            });
        })
        .catch(error => console.error('Error fetching data:', error));
});