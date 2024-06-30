// NAVBAR
window.addEventListener('scroll', function() {
    var navbar = document.querySelector('.navbar');
    var logInAndSignUp = document.getElementById('loginAndSignUp');
    if (window.scrollY > 0) {
        navbar.classList.add('navbar-scrolled');
        logInAndSignUp.classList.add('navbar-scrolled-login-signup')
        
    } else {
        navbar.classList.remove('navbar-scrolled');
        logInAndSignUp.classList.remove('navbar-scrolled-login-signup')
    }
});

//OPEN LOGIN PAGE
var openLoginPage = () =>{
  window.location.href = "/Login/Login.html"
}

//OPEN REGISTER PAGE
var openRegisterPage = () =>{
  window.location.href = "RegisterOption.html"
}

//INITIALIZE LANDING CAROUSEL
document.addEventListener("DOMContentLoaded", function () {
  var landingCarousel = document.querySelector("#LandingCarousel");
  if (landingCarousel) {
    var landingCarouselInstance = new bootstrap.Carousel(landingCarousel, {
      interval: 3000,
      ride: "carousel"
    });
  }

  //CARD SLIDER INITIALIZE FUNCTION
  function initializeCardSlider() {
    var cardSlider = document.querySelector("#CardSlider");
    if (!cardSlider) return;

    var cardCarouselInner = cardSlider.querySelector(".carousel-inner");
    var cardCarouselItems = cardSlider.querySelectorAll(".carousel-item");
    var cardWidth = cardCarouselItems[0].offsetWidth;
    var cardScrollPosition = 0;

    //SLIDING FUNCTION FO NEXT
    function slideToNext() {
      if (cardScrollPosition < cardCarouselInner.scrollWidth - cardWidth * 4) {
        cardScrollPosition += cardWidth;
      } else {
        cardScrollPosition = 0;
      }
      cardCarouselInner.scrollTo({
        left: cardScrollPosition,
        behavior: "smooth"
      });
    }

    // SLIDING FUNCTION FOR PREV 
    function slideToPrev() {
      if (cardScrollPosition > 0) {
        cardScrollPosition -= cardWidth;
      } else {
        cardScrollPosition = cardCarouselInner.scrollWidth - cardWidth * 4;
      }
      cardCarouselInner.scrollTo({
        left: cardScrollPosition,
        behavior: "smooth"
      });
    }

    //NEXT 
    cardSlider.querySelector(".carousel-control-next").addEventListener("click", function () {
      slideToNext();
    });

    // PREVIOUS
    cardSlider.querySelector(".carousel-control-prev").addEventListener("click", function () {
      slideToPrev();
    });

    var autoSlideInterval = setInterval(slideToNext, 3000);

    //ON HOVER STOP AUTOPLAY
    cardSlider.addEventListener("mouseenter", function () {
      clearInterval(autoSlideInterval);
    });
    cardSlider.addEventListener("mouseleave", function () {
      autoSlideInterval = setInterval(slideToNext, 3000);
    });

    //SMALL SCREEN CARD SLIDER
    window.addEventListener("resize", function () {
      cardWidth = cardCarouselItems[0].offsetWidth;
      cardScrollPosition = 0;
      cardCarouselInner.scrollTo({
        left: cardScrollPosition,
        behavior: "auto"
      });
    });

    //ON LOAD SLIDING INITIALIZE
    cardCarouselInner.scrollTo({
      left: cardScrollPosition,
      behavior: "auto"
    });

    var cardCarousel = new bootstrap.Carousel(cardSlider, {
      interval: 3000,
      ride: "carousel"
    });
  }

  //AUTO PLAY FOR SMALL SCREEN
  var cardSlider = document.querySelector("#CardSlider");
  if (cardSlider) {
    if (window.matchMedia("(min-width: 768px)").matches) {
      initializeCardSlider();
    } else {
      var cardCarousel = new bootstrap.Carousel(cardSlider, {
        interval: 3000,
        ride: "carousel"
      });
    }
  }
});