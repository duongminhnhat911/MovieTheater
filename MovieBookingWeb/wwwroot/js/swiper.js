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
    slidesPerView: 1,
    spaceBetween: 40,
    loop: false,
    navigation: {
        nextEl: '.swiper-btn-next',
        prevEl: '.swiper-btn-prev',
    },
    pagination: {
        el: '.movie-swiper-pagination',
        type: 'bullets',
        clickable: true,
    },
});

const comingSoonSwiper = new Swiper('.coming-soon-swiper', {
    slidesPerView: 1,
    spaceBetween: 40,
    loop: false,
    navigation: {
        nextEl: '.coming-soon-btn-next',
        prevEl: '.coming-soon-btn-prev',
    },
    pagination: {
        el: '.coming-soon-swiper-pagination',
        type: 'bullets',
        clickable: true,
    },
});
