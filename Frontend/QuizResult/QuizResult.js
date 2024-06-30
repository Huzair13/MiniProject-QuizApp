const params = new URLSearchParams(window.location.search);
const QuizId = params.get('quizID');
const token = localStorage.getItem('token');
console.log(QuizId);
console.log(token);
document.addEventListener('DOMContentLoaded', function () {
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
            console.log(data);
            const resultsContainer = document.getElementById('results-container');
            data.forEach(result => {
                const card = document.createElement('div');
                card.className = 'card result-card response-card';
                card.innerHTML = `
                            <div class="card-header">
                                Quiz Attempt Result
                            </div>
                            <div class="card-body">
                                <h5 class="card-title">Quiz ID: ${result.quizId}</h5>
                                <p class="card-text">
                                    <strong>User ID:</strong> ${result.userId}<br>
                                    <strong>Score:</strong> ${result.score}<br>
                                    <strong>Time Taken:</strong> ${result.timeTaken}
                                </p>
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