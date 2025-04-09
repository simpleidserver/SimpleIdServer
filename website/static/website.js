
document.addEventListener('DOMContentLoaded', function() {
    let currentIndex = 0;
    function showNextSlide() {
        const carouselItems = document.querySelectorAll('.carousel-item');
        if(carouselItems.length === 0) {
            return;
        }
        
        carouselItems[currentIndex].classList.remove('active');
        currentIndex = (currentIndex + 1) % carouselItems.length;
        carouselItems[currentIndex].classList.add('active');
    }

    function initTailwind()
    {
        try {
            tailwind.config = {
                theme: {
                    extend: {
                        colors: {
                            primary: {
                                50: '#fff7ed',
                                100: '#ffedd5',
                                200: '#fed7aa',
                                300: '#fdba74',
                                400: '#fb923c',
                                500: '#f97316',
                                600: '#ea580c',
                                700: '#c2410c',
                                800: '#9a3412',
                                900: '#7c2d12',
                            }
                        }
                    }
                }
            }
        }
        catch {
            setTimeout(initTailwind, 100);
        }
    }

    setInterval(showNextSlide, 5000);
    initTailwind();
});