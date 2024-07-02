document.addEventListener('DOMContentLoaded',function(){

    const token = localStorage.getItem('token');

    const QuestionInputField = document.getElementById('questionInputFillups');
    const answerElement =  document.getElementById('blankAnswer');
    const QuestionPointsInput = document.getElementById('pointsInputFillups');
    const QuestionCategory = document.getElementById('categoryInputFillUps');
    const otherCategoryInput = document.getElementById('otherCategoryInputFillups');
    
    const difficultyLevelInput = document.getElementById('difficultyLevelInputFillUps');

    const createButton = document.getElementById('createFillUpsQuestions');


    QuestionCategory.addEventListener('change', function() {
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

    createButton.addEventListener('click', function() {
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
    
        console.log("Data to be sent:", data);
    
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
            console.log("Response:", data);
        }).catch(error => {
            console.error('Error:', error);
        });
    });
    
})

