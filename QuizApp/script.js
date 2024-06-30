document.addEventListener('DOMContentLoaded', function() {
    const options = document.querySelectorAll('.option input');
    
    // Add click event listener to each option input
    options.forEach(option => {
        option.addEventListener('click', function() {
            // Remove 'active' class from all options
            options.forEach(opt => opt.parentElement.classList.remove('active'));
            
            // Add 'active' class to the clicked option's parent element (col-md-6)
            this.parentElement.classList.add('active');
        });
    });

    const submitBtn = document.getElementById('submitQuestionBtn');
    submitBtn.addEventListener('click', function() {
        // Get values from inputs
        const question = document.getElementById('questionInput').value;
        const option1 = document.getElementById('option1').value;
        const option2 = document.getElementById('option2').value;
        const option3 = document.getElementById('option3').value;
        const option4 = document.getElementById('option4').value;
        
        // Create question object
        const newQuestion = {
            question: question,
            options: [option1, option2, option3, option4]
        };
        
        // Send newQuestion object to JSON file (you may use AJAX or fetch API)
        // For demonstration, alerting the question object
        alert(JSON.stringify(newQuestion, null, 2));
        
        // Clear inputs
        document.getElementById('questionInput').value = '';
        document.getElementById('option1').value = '';
        document.getElementById('option2').value = '';
        document.getElementById('option3').value = '';
        document.getElementById('option4').value = '';
        
        // Remove 'active' class from all options
        options.forEach(opt => opt.parentElement.classList.remove('active'));
    });
});
