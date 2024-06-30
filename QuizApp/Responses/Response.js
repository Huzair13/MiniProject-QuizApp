document.addEventListener('DOMContentLoaded', function () {
    const params = new URLSearchParams(window.location.search);
    const quizId = parseInt(params.get('quizID'));
    const token = localStorage.getItem('token');
    const userId = parseInt(localStorage.getItem('userID'));

    if (!quizId) {
        alert('No quiz ID provided.');
        return;
    }

    fetch('http://localhost:5273/api/QuizAttempt/GetAllUserResponses', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        console.log('Fetched data:', data); // Log fetched data
        console.log('Expected quizId:', quizId);
        console.log('Expected userId:', userId);

        // Check if quizId and userId are numbers
        if (isNaN(quizId) || isNaN(userId)) {
            console.error('Quiz ID or User ID is not a number.');
            alert('An error occurred while processing the response details.');
            return;
        }

        const userResponses = data.filter(response => {
            console.log(`Checking response with quizId: ${response.quizId} and userId: ${response.userId}`);
            console.log(`Type of quizId in response: ${typeof response.quizId}`);
            console.log(`Type of userId in response: ${typeof response.userId}`);
            return response.quizId === quizId && response.userId === userId;
        });

        console.log('Filtered Responses:', userResponses);

        if (userResponses.length === 0) {
            alert('No previous responses found for this quiz.');
            return;
        }

        const latestResponse = userResponses[userResponses.length - 1]; // Assuming you want the latest response
        displayResponseDetails(latestResponse);
    })
    .catch(error => {
        console.error('Error fetching or parsing data:', error);
        alert('An error occurred while fetching the response details.');
    });

    function displayResponseDetails(response) {
        const startTime = new Date(response.startTime);
        const endTime = response.endTime ? new Date(response.endTime) : new Date();
        const timeTaken = new Date(endTime - startTime);
        const totalQuestions = response.responseAnswers.length;
        const answeredQuestions = response.responseAnswers.filter(answer => answer.submittedAnswer !== null).length;
        const timeRemaining = new Date(timeTaken);

        document.getElementById('start-time').textContent = `Start Time: ${startTime.toLocaleString()}`;
        document.getElementById('answered-questions').textContent = `Answered Questions: ${answeredQuestions} / ${totalQuestions}`;
        document.getElementById('time-remaining').textContent = `Time Remaining: ${timeRemaining.getUTCHours()}h ${timeRemaining.getUTCMinutes()}m ${timeRemaining.getUTCSeconds()}s`;

        document.getElementById('continue-btn').addEventListener('click', function () {
            window.location.href = `/path/to/continue/test/page?responseId=${response.responseId}`;
        });
    }
});
