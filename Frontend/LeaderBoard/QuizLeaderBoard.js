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
                            <td>${user.correctPercentage.toFixed(2)}%</td>
                            <td>${user.timeTaken}</td>
                            <td>${new Date(user.startTime).toLocaleString()}</td>
                            <td>${new Date(user.endTime).toLocaleString()}</td>
                        `;
                leaderboardBody.appendChild(row);
            });
        })
        .catch(error => console.error('Error fetching leaderboard data:', error));
});