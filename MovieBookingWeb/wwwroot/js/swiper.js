const swiper = new Swiper('.swiper-container', {
    loop: true,
    centeredSlides: true,
    slidesPerView: 1,
    spaceBetween: 0,
    autoplay: { 
        delay: 4000, 
        disableOnInteraction: false 
    },
    navigation: { 
        nextEl: '.arrow-btn.next', 
        prevEl: '.arrow-btn.prev' 
    },
    effect: 'fade',
    fadeEffect: { 
        crossFade: true 
    },
});

const movieSwiper = new Swiper('.movie-swiper', {
    slidesPerView: 4,
    spaceBetween: 20,
    navigation: {
        nextEl: '.swiper-btn-next',
        prevEl: '.swiper-btn-prev',
    },
    loop: true,
    breakpoints: {
        0: {
            slidesPerView: 1.5,
        },
        768: {
            slidesPerView: 2.5,
        },
        1024: {
            slidesPerView: 4,
        }
    }
});

const comingSoonSwiper = new Swiper('.coming-soon-swiper', {
    slidesPerView: 4,
    spaceBetween: 20,
    navigation: {
        nextEl: '.coming-soon-btn-next',
        prevEl: '.coming-soon-btn-prev',
    },
    loop: true,
    breakpoints: {
        0: {
            slidesPerView: 1.5,
        },
        768: {
            slidesPerView: 2.5,
        },
        1024: {
            slidesPerView: 4,
        }
    }
});