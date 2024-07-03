const params = new URLSearchParams(window.location.search);
const QuizId = params.get('quizID');
const token = localStorage.getItem('token');
console.log(QuizId);
console.log(token);
document.addEventListener('DOMContentLoaded', function () {
    fetch(`http://localhost:5273/api/QuizAttempt/LeaderBoard?quizId=${QuizId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            const leaderboardBody = document.getElementById('leaderboard-body');
            data.forEach(user => {
                const row = document.createElement('tr');
                const rankClass = user.rank === 1 ? 'badge-gold' : user.rank === 2 ? 'badge-silver' : user.rank === 3 ? 'badge-bronze' : '';
                row.innerHTML = `
                            <th scope="row">
                                <span class="rank-badge ${rankClass}">${user.rank}</span>
                            </th>
                            <td>${user.userId}</td>
                            <td>${user.userName}</td>
                            <td>${user.scoredPoints}</td>
                            <td>${user.correctPercentage}%</td>
                            <td>${user.timeTaken}</td>
                            <td>${new Date(user.startTime).toLocaleString()}</td>
                            <td>${new Date(user.endTime).toLocaleString()}</td>
                        `;
                leaderboardBody.appendChild(row);
            });
        })
        .catch(error => console.error('Error fetching leaderboard data:', error));
});