document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem('token');
    const questionId = new URLSearchParams(window.location.search).get('questionID');

    const mcqForm = document.getElementById('mcqForm');
    const fillUpsForm = document.getElementById('fillUpsForm');

    const questionInputMCQ = document.getElementById('questionInputMCQ');
    const option1MCQ = document.getElementById('option1MCQ');
    const option2MCQ = document.getElementById('option2MCQ');
    const option3MCQ = document.getElementById('option3MCQ');
    const option4MCQ = document.getElementById('option4MCQ');
    const pointsInputMCQ = document.getElementById('pointsInput');
    const categoryInputMCQ = document.getElementById('categoryInput');
    const otherCategoryInputMCQ = document.getElementById('otherCategoryInput');
    const difficultyLevelInputMCQ = document.getElementById('difficultyLevelInput');
    const correctAnswerInputMCQ = document.getElementById('correctAnswerInput');

    const questionInputFillUps = document.getElementById('questionInputFillups');
    const blankAnswerFillUps = document.getElementById('blankAnswer');
    const pointsInputFillUps = document.getElementById('pointsInputFillups');
    const categoryInputFillUps = document.getElementById('categoryInputFillUps');
    const otherCategoryInputFillUps = document.getElementById('otherCategoryInputFillups');
    const difficultyLevelInputFillUps = document.getElementById('difficultyLevelInputFillUps');

    const option1 = document.getElementById('option1');
    const option2 = document.getElementById('option2');
    const option3 = document.getElementById('option3');
    const option4 = document.getElementById('option4');

    categoryInputFillUps.addEventListener('change', function () {
        if (this.value === 'Other') {
            otherCategoryInputFillUps.style.display = 'block';
        } else {
            otherCategoryInputFillUps.style.display = 'none';
        }
    });

    categoryInputMCQ.addEventListener('change', function () {
        if (this.value === 'Other') {
            otherCategoryInputMCQ.style.display = 'block';
        } else {
            otherCategoryInputMCQ.style.display = 'none';
        }
    });

    const saveButton = document.getElementById('saveQuestionBtn');
    
    async function fetchQuestionData() {
        try {
            const response = await fetch(`http://localhost:5273/api/ViewQuestion/GetQuestionByQuestionID?questionID=${questionId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const questionData = await response.json();
            console.log(questionData);
            autofillForm(questionData);
        } catch (error) {
            console.error('Error fetching question data:', error);
        }
    }

    function autofillForm(data) {
        if (data.questionType === 'MCQ' || data.questionType === 'MultipleChoice') {
            mcqForm.style.display = 'block';
            fillUpsForm.style.display = 'none';

            questionInputMCQ.value = data.questionText;
            option1MCQ.value = data.choice1 || '';
            option2MCQ.value = data.choice2 || '';
            option3MCQ.value = data.choice3 || '';
            option4MCQ.value = data.choice4 || '';

            option1.value = data.choice1;
            option2.value = data.choice2;
            option3.value = data.choice3;
            option4.value = data.choice4;

            pointsInputMCQ.value = data.points;
            setDropdownValue(categoryInputMCQ, data.category);
            setDifficultyDropdown(difficultyLevelInputMCQ, data.difficultyLevel);
            setOptionsDropDown(correctAnswerInputMCQ, data.correctAnswer);
            correctAnswerInputMCQ.value = data.correctAnswer;

            toggleOtherCategoryInput(categoryInputMCQ.value, otherCategoryInputMCQ, data.category);
        } else if (data.questionType === 'Fillups') {
            mcqForm.style.display = 'none';
            fillUpsForm.style.display = 'block';

            questionInputFillUps.value = data.questionText;
            blankAnswerFillUps.value = data.correctAnswer || '';
            pointsInputFillUps.value = data.points;
            setDropdownValue(categoryInputFillUps, data.category);
            setDifficultyDropdown(difficultyLevelInputFillUps, data.difficultyLevel);

            toggleOtherCategoryInput(categoryInputFillUps.value, otherCategoryInputFillUps, data.category);
        }
    }

    function setOptionsDropDown(dropdown, value) {
        dropdown.value = value
    }
    function setDifficultyDropdown(dropdown, value) {
        dropdown.value = value;
    }

    function setDropdownValue(dropdown, value) {
        const option = [...dropdown.options].find(opt => opt.value === value);
        if (option) {
            dropdown.value = value;
        } else {
            dropdown.value = 'Other';
        }
    }

    function toggleOtherCategoryInput(categoryValue, otherInput, category) {
        if (categoryValue === 'Other') {
            otherInput.style.display = 'block';
        } else {
            otherInput.style.display = 'none';
        }
        otherInput.value = category;
    }


    saveButton.addEventListener('click', async function () {
        const payload = {
            id: parseInt(questionId),
            questionText: '',
            points: 0,
            category: '',
            difficultyLevel: '',
            correctAnswer: '',
            choice1: '',
            choice2: '',
            choice3: '',
            choice4: ''
        };

        if (mcqForm.style.display === 'block') {
            payload.questionText = questionInputMCQ.value;
            payload.points = parseFloat(pointsInputMCQ.value);
            payload.category = categoryInputMCQ.value;
            payload.difficultyLevel = parseInt(difficultyLevelInputMCQ.value);
            payload.correctAnswer = correctAnswerInputMCQ.value;
            payload.choice1 = option1MCQ.value;
            payload.choice2 = option2MCQ.value;
            payload.choice3 = option3MCQ.value;
            payload.choice4 = option4MCQ.value;
        } else if (fillUpsForm.style.display === 'block') {
            delete payload.choice1;
            delete payload.choice2;
            delete payload.choice3;
            delete payload.choice4;
            payload.questionText = questionInputFillUps.value;
            payload.points = parseFloat(pointsInputFillUps.value);
            payload.category = categoryInputFillUps.value;
            payload.difficultyLevel = parseInt(difficultyLevelInputFillUps.value);
            payload.correctAnswer = blankAnswerFillUps.value;
        }

        try {
            console.log(payload);
            const response = await fetch(`http://localhost:5273/api/Question/EditQuestionByID`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            alert('Question updated successfully');
        } catch (error) {
            console.error('Error saving question data:', error);
        }
    });

    fetchQuestionData();
});
