document.addEventListener('DOMContentLoaded', function () {

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


    // Event listener for Category dropdown change
    QuestionCategory.addEventListener('change', function () {
        if (this.value === 'Other') {
            otherCategoryInput.style.display = 'block';
        } else {
            otherCategoryInput.style.display = 'none';
        }
    });
    

    // function parseJwt(token) {
    //     try {
    //         return JSON.parse(atob(token.split('.')[1]));
    //     } catch (e) {
    //         return {};
    //     }
    // }

    // const printToken = () =>{

    //     if (token) {
    //         console.log("JWT Token:", token);
    //         alert("JWT Token:\n" + token);
    //     } else {
    //         alert("No JWT token found in local storage.");
    //     }
    //     const decodedToken = parseJwt(token);
    //     const userRole = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    //     console.log(token);
    //     console.log(decodedToken);
    //     console.log(userRole);

    // }

    createButton.addEventListener('click', function () {
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

        console.log("Data to be sent:", data);

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
            console.log("Response:", data);
            // Handle response as needed
        }).catch(error => {
            console.error('Error:', error);
        });
    });

})

