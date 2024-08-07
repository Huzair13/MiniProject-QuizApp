document.addEventListener('DOMContentLoaded', function () {

    const params = new URLSearchParams(window.location.search);
    const type = params.get('type');

    if (!type || (type !== 'student' && type !== 'teacher')) {
        window.location.href = 'RegisterOption.html';
        return;
    }

    //INPUTS
    const userName = document.getElementById('RegisterUserName');
    const userEmail = document.getElementById('RegisterEmail');
    const userMobile = document.getElementById('RegisterMobileNum');
    const userDOB = document.getElementById('RegisterDOB');
    const userEducation = document.getElementById('educationalQualification');
    const userDesignation = document.getElementById('designation');
    const userPass1 = document.getElementById('registerPass1');
    const userPass2 = document.getElementById('registerPass2');

    const loadingModal = new bootstrap.Modal(document.getElementById('loadingModal'));

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
    }

    // Hide loading modal
    function hideLoadingModal() {
        loadingModal.hide();
    }


    //STUDENT EDUCATION QUALIFICATION
    var studentFields = document.getElementById('studentFields');

    //TEACHER DESIGNATION
    var teacherFields = document.getElementById('teacherFields');

    //DISPLAYING BASED ON ROLE
    if (type === 'student') {
        if (studentFields) {
            studentFields.style.display = 'block';
        }
        if (teacherFields) {
            teacherFields.style.display = 'none';
            teacherFields.querySelectorAll('select').forEach(select => {
                select.removeAttribute('required');
            });
        }
    } else if (type === 'teacher') {
        if (teacherFields) {
            teacherFields.style.display = 'block';
        }
        if (studentFields) {
            studentFields.style.display = 'none';
            studentFields.querySelectorAll('select').forEach(select => {
                select.removeAttribute('required');
            });
        }
    }

    //FORM VALIDATION ---- ON SUBMIT
    const forms = document.querySelectorAll('.needs-validation');
    forms.forEach(form => {
        form.addEventListener('submit', function (event) {
            const txtPass = userPass1.value.trim();
            const txtPass2 = userPass2.value.trim();

            // showLoadingModal();
            event.preventDefault();

            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            } else {
                event.preventDefault();

                if (txtPass !== txtPass2) {
                    hideLoadingModal();
                    alert("PassWord Not Matched");
                    return;
                }
                Register();
            }

            form.classList.add('was-validated');
        });
    });

    var Register = () => {
        const txtUserName = userName.value.trim();
        const txtEmail = userEmail.value.trim();
        const txtMobile = userMobile.value.trim();
        const txtDOB = userDOB.value;
        const txtPass = userPass1.value.trim();
        const txtPass2 = userPass2.value.trim();

        var txtDesignation = "";
        var txtEducationQualification = "";
        var txtType = "";

        if (type === "student") {
            txtEducationQualification = userEducation.value.trim();
            txtType = "Student";
        } else if (type === "teacher") {
            txtDesignation = userDesignation.value.trim();
            txtType = "Teacher";
        }

        const jsonInput = {
            name: txtUserName,
            email: txtEmail,
            mobileNumber: txtMobile,
            dateOfBirth: txtDOB,
            password: txtPass,
            educationQualification: txtEducationQualification,
            designation: txtDesignation,
            userType: txtType
        };

        fetch('http://localhost:5273/api/User/Register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(jsonInput)
        })
            .then(res => {
                if (!res.ok) {
                    throw new Error('Network response was not ok');
                }
                return res.json();
            })
            .then(data => {
                hideLoadingModal();
                setTimeout(() => {
                    const userID = data.id;
                    document.getElementById('userID').textContent = userID;
                    $('#userIDModal').modal('show');
                }, 1500);

            })
            .catch(error => {
                hideLoadingModal();
                console.error('Error:', error);
            });
    };


    //REDIRECT TO LOGIN AFTER REGISTER
    $('#userIDModal').on('hidden.bs.modal', function () {
        window.location.href = '/Login/Login.html';
    });

    //VALIDATION ON ENTERING THE INPUT
    const inputs = document.querySelectorAll('input, select');
    inputs.forEach(input => {
        input.addEventListener('input', function () {
            validateInput(input);
        });
    });

    //VALIDATE THE INPUT
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
