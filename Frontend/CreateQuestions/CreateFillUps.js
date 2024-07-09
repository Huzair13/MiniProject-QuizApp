const token = localStorage.getItem('token');
if (!token) {
    window.location.href = './Home/Home.html'
}

const userRole = localStorage.getItem('role')
if (userRole === 'Student') {
    alert('Unauthorized');
    window.location.href = '/LoggedInHome/LoggedInHome.html';
}


document.addEventListener('DOMContentLoaded', function () {

    const logoutButton = document.getElementById('logoutbtn');
    const logoutModal = new bootstrap.Modal(document.getElementById('logoutModal'));
    const confirmLogoutButton = document.getElementById('confirmLogoutButton');
    
    const loadingModal = new bootstrap.Modal(document.getElementById('loadingModal'));

    const token = localStorage.getItem('token');
    const QuestionInputField = document.getElementById('questionInputFillups');
    const answerElement = document.getElementById('blankAnswer');
    const QuestionPointsInput = document.getElementById('pointsInputFillups');
    const QuestionCategory = document.getElementById('categoryInputFillUps');
    const otherCategoryInput = document.getElementById('otherCategoryInputFillups');
    const difficultyLevelInput = document.getElementById('difficultyLevelInputFillUps');


    const createButton = document.getElementById('createFillUpsQuestions');
    const confirmButton = document.getElementById('confirmBtn');
    const confirmModal = new bootstrap.Modal(document.getElementById('confirmModal'));


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

    // Toggle other category input visibility
    QuestionCategory.addEventListener('change', function () {
        if (this.value === 'Other') {
            otherCategoryInput.required = true;
            otherCategoryInput.style.display = 'block';
        } else {
            otherCategoryInput.required = false;
            otherCategoryInput.style.display = 'none';
        }
        validateInput(otherCategoryInput);
    });

    const form = document.querySelector('.needs-validation');
    // Inside the event listener for form submission
    createButton.addEventListener('click', function (event) {
        event.preventDefault();

        if (!form.checkValidity()) {
            event.stopPropagation();
            form.classList.add('was-validated');
        } else {
            confirmModal.show();
        }
    });

    confirmButton.addEventListener('click', function () {
        confirmModal.hide();
        showLoadingModal();
        createQuestion();
    });


    // Function to create question
    const createQuestion = () => {
        // const confirmation = confirm("Are you sure you want to create the question?");

        const QuestionTxt = QuestionInputField.value;
        const points = parseFloat(QuestionPointsInput.value);
        const category = QuestionCategory.value === 'Other' ? otherCategoryInput.value : QuestionCategory.value;
        const difficultyLevel = parseInt(difficultyLevelInput.value);
        const correctAnswer = answerElement.value;

        const data = {
            QuestionText: QuestionTxt,
            Points: points,
            Category: category,
            DifficultyLevel: difficultyLevel,
            CorrectAnswer: correctAnswer
        };

        fetch('http://localhost:5273/api/Question/AddFillUpsQuestion', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(data)
        }).then(res => {
            if (!res.ok) {
                throw new Error(`HTTP error! Status: ${res.status}`);
            }
            return res.json();
        }).then(data => {
            setTimeout(function () {
                hideLoadingModal();
                const questionID = data.id;
                document.getElementById('questionID').textContent = questionID;
                $('#questionIDModal').modal('show');
            }, 2000);
        }).catch(error => {
            alert(error.message)
            console.error('Error:', error);
            hideLoadingModal();
        });
    }

    // Show loading modal
    function showLoadingModal() {
        const loadingAnimationContainer = document.getElementById('loadingAnimation');
        loadingAnimationContainer.innerHTML = '';

        const animation = bodymovin.loadAnimation({
            container: loadingAnimationContainer,
            renderer: 'svg',
            loop: true,
            autoplay: true,
            path: 'https://lottie.host/c8bd9837-fcdf-4106-8906-b454da03b8b7/9qRpxRP31N.json'
        });

        loadingModal.show();
        // $('#loadingModal').modal('show');
    }

    // Hide loading modal
    function hideLoadingModal() {
        loadingModal.hide();
        // $('#loadingModal').modal('hide');
    }


    // Redirect to home on modal close
    $('#questionIDModal').on('hidden.bs.modal', function () {
        window.location.href = "/LoggedInHome/LoggedInHome.html";
    });

    // Validate input fields on input
    const inputs = document.querySelectorAll('input, select, textarea');
    inputs.forEach(input => {
        input.addEventListener('input', function () {
            validateInput(input);
        });
    });

    // Function to validate input fields
    function validateInput(input) {
        if (input.checkValidity()) {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
        } else {
            input.classList.remove('is-valid');
            input.classList.add('is-invalid');
        }
    }
});
