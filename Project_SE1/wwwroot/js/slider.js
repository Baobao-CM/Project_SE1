
$(document).ready(function () {
    //banner slider index
    $('.banner-slider').slick({
        autoplay: true,
        dots: false,
        arrows: true,
        prevArrow: '<button type="button" class="slick-prev btn btn-dark"><i class="bi bi-chevron-left"></i></button>',
        nextArrow: '<button type="button" class="slick-next btn btn-dark"><i class="bi bi-chevron-right"></i></button>',
        infinite: true,
        speed: 800,
        fade: true,
        cssEase: 'linear'
    });
});
