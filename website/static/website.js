
document.addEventListener('DOMContentLoaded', function() {
    let currentIndex = 0;
    
    function showNextSlide() {
        const carouselItems = document.querySelectorAll('.carousel-item');
        if(carouselItems.length === 0) {
            return;
        }
        
        // Hide current slide
        carouselItems[currentIndex].classList.remove('active');
        
        // Calculate next index
        currentIndex = (currentIndex + 1) % carouselItems.length;
        
        // Show next slide
        carouselItems[currentIndex].classList.add('active');
    }

    // Start the carousel
    setInterval(showNextSlide, 5000); // Change slide every 5 seconds
});

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