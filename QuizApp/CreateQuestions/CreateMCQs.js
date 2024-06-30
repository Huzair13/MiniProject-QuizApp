document.addEventListener('DOMContentLoaded',function(){

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


    QuestionCategory.addEventListener('change', function() {
        if (this.value === 'Other') {
            otherCategoryInput.style.display = 'block';
        } else {
            otherCategoryInput.style.display = 'none';
        }
    });

    function parseJwt(token) {
        try {
            return JSON.parse(atob(token.split('.')[1]));
        } catch (e) {
            return {};
        }
    }

    const printToken = () =>{
        
        if (token) {
            console.log("JWT Token:", token);
            alert("JWT Token:\n" + token);
        } else {
            alert("No JWT token found in local storage.");
        }
        const decodedToken = parseJwt(token);
        const userRole = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

        console.log(token);
        console.log(decodedToken);
        console.log(userRole);

    }

    createButton.addEventListener('click', function() {
        printToken();
        const QuestionTxt = QuestionInputField.value;
        const option1 = Option1InputField.value;
        const option2 = Option2InputField.value;
        const option3 = Option3InputField.value;
        const option4 = Option4InputField.value;
        const points = parseFloat(pointsInput.value); 
        const category = QuestionCategory.value === 'Other' ? otherCategoryInput.value : QuestionCategory.value;
        const difficultyLevel = parseInt(difficultyLevelInput.value); 
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

