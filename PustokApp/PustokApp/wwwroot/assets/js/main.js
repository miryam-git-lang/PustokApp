$(document).ready(function () {
    $(".bookModalIcon").click(function () {
        let url = $(this).attr("href");
        fetch(url)            .then(res => res.text())
            .then((response) => response.text())
            .then((data) => {
            
                let firstSlider = {
                    "infinite":true,
                    "autoplay": true,
                    "autoplaySpeed": 8000,
                    "slidesToShow": 4,
                    "arrows": true,
                    "prevArrow":{"buttonClass": "slick-prev","iconClass":"fa fa-chevron-left"},
                    "nextArrow":{"buttonClass": "slick-next","iconClass":"fa fa-chevron-right"},
                    "asNavFor": ".product-details-slider",
                    "focusOnSelect": true
                };
                let secondSlider = {
                    "infinite":true,
                    "autoplay": true,
                    "autoplaySpeed": 8000,
                    "slidesToShow": 4,
                    "arrows": true,
                    "prevArrow":{"buttonClass": "slick-prev","iconClass":"fa fa-chevron-left"},
                    "nextArrow":{"buttonClass": "slick-next","iconClass":"fa fa-chevron-right"},
                    "asNavFor": ".product-details-slider",
                    "focusOnSelect": true
                };
                
                $(".product-details-slider").slick(firstSlider);
                $(".product-details-nav").slick(secondSlider);
                $("#quickModal").modal("show"); 
                
            })
    })
})