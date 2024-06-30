(() => {
    'use strict';
    function parseJwt(token) {
        try {
            return JSON.parse(atob(token.split('.')[1]));
        } catch (e) {
            return {};
        }
    }


    const form = document.querySelector('.needs-validation');
    const userIdInput = document.getElementById('loginUserID');
    const userPassInput = document.getElementById('loginUserPass');

    userIdInput.addEventListener('input', function() {
        validateInput(userIdInput);
    });

    userPassInput.addEventListener('input', function() {
        validateInput(userPassInput);
    });

    form.addEventListener('submit', event => {
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        } else {
            event.preventDefault(); 
            const txtUid = userIdInput.value * 1;
            const txtPass = userPassInput.value;

            fetch('http://localhost:5273/api/User/Login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    userId: txtUid,
                    password: txtPass
                })
            })
            .then(res => res.json())
            .then(data => {
                console.log(data);
                localStorage.setItem('token', data.token);
                localStorage.setItem('userID',data.userID);
                localStorage.setItem('role',data.role);

                const token = localStorage.getItem('token');
                const decodedToken = parseJwt(token);
                const userRole = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

                console.log(token);
                console.log(decodedToken);
                console.log(userRole);
                // alert(data.userRole);
                window.location.href = '/LoggedInHome/StudentHome.html'
            })
            .catch(error => {
                console.error('Error:', error);
            });
        }

        form.classList.add('was-validated');
    });

    function validateInput(input) {
        if (!input.checkValidity()) {
            input.classList.add('is-invalid');
            input.classList.remove('is-valid');
        } else {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
        }
    }
})();
