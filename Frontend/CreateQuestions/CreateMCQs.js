const token = localStorage.getItem('token');
if (!token) {
    window.location.href = './Home/Home.html'
}

const userRole = localStorage.getItem('role')
if (userRole === 'student') {
    alert('Unauthorized');
    window.location.href = '/LoggedInHome/LoggedInHome.html'; // Redirect to home page
}

document.addEventListener('DOMContentLoaded', function () {

    const logoutButton = document.getElementById('logoutbtn');
    const logoutModal = new bootstrap.Modal(document.getElementById('logoutModal'));
    const confirmLogoutButton = document.getElementById('confirmLogoutButton');

    const token = localStorage.getItem('token');

    const QuestionInputField = document.getElementById('questionInputMCQ');
    const Option1InputField = document.getElementById('option1MCQ');
    const Option2InputField = document.getElementById('option2MCQ');
    const Option3InputField = document.getElementById('option3MCQ');
    const Option4InputField = document.getElementById('option4MCQ');
    const pointsInput = document.getElementById('pointsInput');
    const QuestionCategory = document.getElementById('categoryInput');
    const otherCategoryInput = document.getElementById('otherCategoryInput');

    const difficultyLevelInput = document.getElementById('difficultyLevelInput');
    const correctAnswerInput = document.getElementById('correctAnswerInput');

    const createButton = document.getElementById('CreateMCQQuestion');
    const confirmButton = document.getElementById('confirmBtn');
    const confirmModal = new bootstrap.Modal(document.getElementById('confirmModal'));

    const loadingModal = new bootstrap.Modal(document.getElementById('loadingModal'));

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


    // Event listener for Category dropdown change
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

    const createQuestion = () => {
        const QuestionTxt = QuestionInputField.value;
        const option1 = Option1InputField.value;
        const option2 = Option2InputField.value;
        const option3 = Option3InputField.value;
        const option4 = Option4InputField.value;
        const points = parseFloat(pointsInput.value);
        const category = QuestionCategory.value === 'Other' ? otherCategoryInput.value : QuestionCategory.value;
        const difficultyLevel = parseInt(difficultyLevelInput.value);

        const correctAnswerInputOpt1 = document.getElementById('correctAnswerInputOpt1');
        const correctAnswerInputOpt2 = document.getElementById('correctAnswerInputOpt2');
        const correctAnswerInputOpt3 = document.getElementById('correctAnswerInputOpt3');
        const correctAnswerInputOpt4 = document.getElementById("correctAnswerInputOpt4");

        correctAnswerInputOpt1.value = option1;
        correctAnswerInputOpt2.value = option2;
        correctAnswerInputOpt3.value = option3;
        correctAnswerInputOpt4.value = option4;

        const correctAnswer = correctAnswerInput.value;

        const data = {
            QuestionText: QuestionTxt,
            Points: points,
            Category: category,
            DifficultyLevel: difficultyLevel,
            Choice1: option1,
            Choice2: option2,
            Choice3: option3,
            Choice4: option4,
            CorrectAnswer: correctAnswer
        };

        // console.log("Data to be sent:", data);

        fetch('http://localhost:5273/api/Question/AddMCQQuestion', {
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
            // console.log("Response:", data);
            setTimeout(function () {
                hideLoadingModal();
                const questionID = data.id;
                document.getElementById('questionID').textContent = questionID;
                $('#questionIDModal').modal('show');
            }, 2000);
        }).catch(error => {
            console.error('Error:', error);
        });
    }

    //REDIRECT TO HOME
    $('#questionIDModal').on('hidden.bs.modal', function () {
        window.location.href = "/LoggedInHome/LoggedInHome.html"
    });


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

        // $('#loadingModal').modal('show');
        loadingModal.show();
    }

    // Hide loading modal
    function hideLoadingModal() {
        // $('#loadingModal').modal('hide');
        loadingModal.hide();
    }

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
})

