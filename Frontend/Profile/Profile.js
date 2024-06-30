const token = localStorage.getItem('token');
const user = localStorage.getItem('userID');
const role = localStorage.getItem('role');

const nameElementProfile = document.getElementById('userName');
const emailElementProfile = document.getElementById('userEmail');
const mobileElementProfile = document.getElementById('userMobile');
const ageElementProfile = document.getElementById('userAge');
const coinsElementStudent = document.getElementById('userCoins');
const quizAttendedStudent = document.getElementById('userQuizAttended');
const quizCreatedTeacher = document.getElementById('userQuizCreated');
const questionsCreated = document.getElementById('userQuestionsCreated');
const teacherElement = document.getElementById('teachersData');
const studentElement = document.getElementById('studentData');
const designationOrEducation = document.getElementById('userDesignationOrQualification');
const lottiePlayerElement = document.getElementById('profileLottiePlayer');
const lottiePlayerDiv = document.getElementById('avatarDiv');

document.addEventListener('DOMContentLoaded', async function () {
    console.log(role);
    if (role === "Student") {
        fetch('http://localhost:5273/api/UserView/ViewStudentProfile', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => response.json())
            .then(data => {
                console.log(data);
                studentElement.style.display = "block";
                teacherElement.style.display = "none";
                nameElementProfile.textContent = data.name;
                ageElementProfile.textContent = data.age;
                emailElementProfile.textContent = data.email;
                mobileElementProfile.textContent = data.mobileNumber;
                coinsElementStudent.textContent = data.coinsEarned;
                quizAttendedStudent.textContent = data.numsOfQuizAttended;
                designationOrEducation.textContent = data.educationQualification;

                var lottiePlayer = document.createElement('lottie-player');

                lottiePlayer.setAttribute('src', 'https://lottie.host/46fc593d-b615-4c85-8963-a19dc79324b7/6xaVkVJwXc.json');
                lottiePlayer.setAttribute('speed', '1');
                lottiePlayer.setAttribute('style', 'width: 300px; height: 300px');
                lottiePlayer.setAttribute('loop', 'true');
                lottiePlayer.setAttribute('autoplay', 'true');
                lottiePlayer.setAttribute('direction', '1');
                lottiePlayer.setAttribute('mode', 'normal');
                lottiePlayer.setAttribute('id', 'profileLottiePlayer');

                lottiePlayerDiv.appendChild(lottiePlayer);
            })
            .catch(error => console.error('Error fetching student profile data:', error));
    } else if (role == "Teacher") {
        fetch('http://localhost:5273/api/UserView/ViewTeacherProfile', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => response.json())
            .then(data => {
                console.log(data);
                studentElement.style.display = "none";
                teacherElement.style.display = "block";
                nameElementProfile.textContent = data.name;
                ageElementProfile.textContent = data.age;
                emailElementProfile.textContent = data.email;
                mobileElementProfile.textContent = data.mobileNumber;
                quizCreatedTeacher.textContent = data.numsOfQuizCreated;
                questionsCreated.textContent = data.numsOfQuestionsCreated;
                designationOrEducation.textContent = data.designation;

                
                var lottiePlayer = document.createElement('lottie-player');

                lottiePlayer.setAttribute('src', 'https://lottie.host/c84fb903-ff80-4473-a769-06d54b7d319e/qRljOGU0Cj.json');
                lottiePlayer.setAttribute('speed', '1');
                lottiePlayer.setAttribute('style', 'width: 300px; height: 300px');
                lottiePlayer.setAttribute('loop', 'true');
                lottiePlayer.setAttribute('autoplay', 'true');
                lottiePlayer.setAttribute('direction', '1');
                lottiePlayer.setAttribute('mode', 'normal');
                lottiePlayer.setAttribute('id', 'profileLottiePlayer');

                lottiePlayerDiv.appendChild(lottiePlayer);

            })
            .catch(error => console.error('Error fetching teacher profile data:', error));
    }
});
