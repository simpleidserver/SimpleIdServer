import React, { useEffect } from 'react';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import HomepageFeatures from '@site/src/components/HomepageFeatures';
import HomepageEnterprise from '@site/src/components/HomepageEnterprise';
import HomepageOpensource from '@site/src/components/HomepageOpensource';
import HomepageInstallation from '@site/src/components/HomepageInstallation';
import HomepageFullyCustom from '@site/src/components/HomepageFullyCustom';
import HomepageOpenStandards from '../components/HomepageOpenStandards';
import HomePageMigration from '../components/HomepageMigration';

function HomepageHeader() {
  return ( 
    <div class="hero-gradient">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 main-container">
            <div class="grid grid-cols-1 md:grid-cols-12 gap-8 items-center">
                <div class="md:col-span-4 branding-column">
                    <div>
                        <h1 className="text-4xl tracking-tight font-extrabold text-gray-900 sm:text-5xl md:text-6xl">                            
                            <span>Simple</span>
                            <span className="text-primary-600">Idserver</span>
                        </h1>
                        <p class="text-2xl text-primary-600 font-medium leading-relaxed">Secure identity. Simple ecosystem.</p>
                    </div>
                    <div className="flex flex-col sm:flex-row sm:justify-center lg:justify-start space-y-3 sm:space-y-0 sm:space-x-3">
                        <a href="/docs/overview" className="w-full flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 md:py-4 md:text-lg md:px-10 transition duration-150 ease-in-out">
                            Get Started
                        </a>
                        <a href="https://website.simpleidserver.com/master/clients" className="w-full flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-md text-primary-700 bg-primary-100 hover:bg-primary-200 md:py-4 md:text-lg md:px-10 transition duration-150 ease-in-out">
                            Live Demo
                        </a>
                    </div>
                </div>
                <div class="md:col-span-8 carousel-column">
                    <div class="carousel h-full">
                        <div class="carousel-inner">
                            <div class="carousel-item active" style={{ height: "420px"}} data-index="0">
                                <div class="flex flex-col md:flex-row items-center">
                                    <div class="md:w-1/2 animate-fade-in" style={{ "animationDelay": "0.1s" }}>
                                        <div class="feature-icon">
                                            <i class="fas fa-shield-alt"></i>
                                        </div>
                                        <h3 class="text-3xl font-bold text-gray-900 mb-4">Multi-protocol Identity Server</h3>
                                        <ul class="space-y-3">
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">OpenID Connect</span>
                                            </li>
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">WS-Federation</span>
                                            </li>
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">SAML 2.0</span>
                                            </li>
                                        </ul>
                                    </div>
                                    <div class="md:w-1/2 animate-fade-in" style={{ "animationDelay": "0.3s" }}>
                                        <div class="bg-gray-50 p-4 rounded-lg">
                                            <div class="p-2 bg-white rounded shadow-sm border border-gray-100">
                                                <img src="/img/clients.png" style={{ height: "200px", display: "block", margin: "auto"}} alt="Identity Server Protocols" class="rounded" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="carousel-item" style={{ height: "420px"}} data-index="1">
                                <div class="flex flex-col md:flex-row items-center">
                                    <div class="md:w-1/2 animate-fade-in" style={{ "animationDelay": "0.1s" }}>
                                        <div class="feature-icon">
                                            <i class="fas fa-paint-brush"></i>
                                        </div>
                                        <h3 class="text-3xl font-bold text-gray-900 mb-4">Customizable Authentication Interface</h3>
                                        <ul class="space-y-3">
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">Customizable themes and colors</span>
                                            </li>
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">Adaptable page templates</span>
                                            </li>
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">Unified user experience</span>
                                            </li>
                                        </ul>
                                    </div>
                                    <div class="md:w-1/2 animate-fade-in" style={{ "animationDelay": "0.3s" }}>
                                        <div class="bg-gray-50 p-4 rounded-lg">
                                            <div class="p-2 bg-white rounded shadow-sm border border-gray-100">
                                                <img src="/img/auth.png" style={{ height: "200px", display: "block", margin: "auto"}}  alt="Authentication Interface" class="rounded" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="carousel-item" style={{ height: "420px"}} data-index="2">
                                <div class="flex flex-col md:flex-row items-center">
                                    <div class="md:w-1/2 animate-fade-in" style={{ "animationDelay": "0.1s" }}>
                                        <div class="feature-icon">
                                            <i class="fas fa-users-cog"></i>
                                        </div>
                                        <h3 class="text-3xl font-bold text-gray-900 mb-4">Flexible Identity Provisioning</h3>
                                        <ul class="space-y-3">
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">Automatic provisioning via SCIM and LDAP</span>
                                            </li>
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">Intuitive administration interface</span>
                                            </li>
                                        </ul>
                                    </div>
                                    <div class="md:w-1/2 animate-fade-in" style={{ "animationDelay": "0.3s" }}>
                                        <div class="bg-gray-50 p-4 rounded-lg">
                                            <div class="p-2 bg-white rounded shadow-sm border border-gray-100">
                                                <img src="/img/provisioning.png" style={{ height: "200px", display: "block", margin: "auto"}} alt="Identity Provisioning" class="rounded" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="carousel-item" style={{ height: "420px"}} data-index="3">
                                <div class="flex flex-col md:flex-row items-center ">
                                    <div class="md:w-1/2 animate-fade-in" style={{ "animationDelay": "0.1s" }}>
                                        <div class="feature-icon">
                                            <i class="fas fa-wallet"></i>
                                        </div>
                                        <h3 class="text-3xl font-bold text-gray-900 mb-4">EBSI-compatible Digital Wallet</h3>
                                        <ul class="space-y-3">
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">Verifiable Credentials management</span>
                                            </li>
                                            <li class="flex items-center">
                                                <span class="bg-primary-100 text-primary-600 rounded-full p-1.5 mr-3">
                                                    <i class="fas fa-check text-sm"></i>
                                                </span>
                                                <span class="text-lg">Sovereign identity and data portability</span>
                                            </li>
                                        </ul>
                                    </div>
                                    <div class="md:w-1/2 animate-fade-in" style={{ "animationDelay": "0.3s" }}>
                                        <div class="bg-gray-50 p-4 rounded-lg">
                                            <div class="p-2 bg-white rounded shadow-sm border border-gray-100">
                                                <img src="/img/mobile.webp" style={{ height: "200px", display: "block", margin: "auto"}}  alt="EBSI Wallet" class="rounded" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="carousel-control carousel-control-prev">
                            <i class="fas fa-chevron-left"></i>
                        </div>
                        <div class="carousel-control carousel-control-next">
                            <i class="fas fa-chevron-right"></i>
                        </div>
                        <div class="carousel-indicators">
                            <div class="carousel-indicator active" data-slide-to="0"></div>
                            <div class="carousel-indicator" data-slide-to="1"></div>
                            <div class="carousel-indicator" data-slide-to="2"></div>
                            <div class="carousel-indicator" data-slide-to="3"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
  );
}

function init() {
    // Store initialized carousels
    let initializedCarousels = new Set();

    // Function to initialize all carousels on the page
    function initAllCarousels() {
        // Find all carousel containers
        const carousels = document.querySelectorAll('.carousel');

        // Initialize each carousel if not already initialized
        carousels.forEach((carousel, carouselIndex) => {
            const carouselId = carousel.id || `carousel-${carouselIndex}`;            
            // Skip if already initialized
            if (initializedCarousels.has(carouselId)) {
                return;
            }

            // Set a unique ID if not already set
            if (!carousel.id) {
                carousel.id = carouselId;
            }

            initCarousel(carousel);
            initializedCarousels.add(carouselId);
        });
    }

    // Function to initialize a single carousel
    function initCarousel(carouselElement) {
        // Create carousel state object
        const carouselState = {
            currentIndex: 0,
            slideInterval: null,

            // Find carousel elements within this specific carousel
            inner: carouselElement.querySelector('.carousel-inner'),
            items: carouselElement.querySelectorAll('.carousel-item'),
            indicators: carouselElement.querySelectorAll('.carousel-indicator'),
            prevControl: carouselElement.querySelector('.carousel-control-prev'),
            nextControl: carouselElement.querySelector('.carousel-control-next')
        };

        const totalItems = carouselState.items.length;

        // Skip if no items found
        if (totalItems === 0) {
            return;
        }

        // Function to update carousel display
        function updateCarousel() {
            // Update active carousel item
            carouselState.items.forEach((item, index) => {
                        if (index === carouselState.currentIndex) {
                            item.classList.add('active');
                        } else {
                            item.classList.remove('active');
                        }
            });

            // Update carousel inner position
            carouselState.inner.style.transform = `translateX(-${carouselState.currentIndex * 100}%)`;

            // Update indicators
            carouselState.indicators.forEach((indicator, index) => {
                if (index === carouselState.currentIndex) {
                    indicator.classList.add('active');
                } else {
                    indicator.classList.remove('active');
                }
            });

            // Reset animations
            const currentItem = carouselState.items[carouselState.currentIndex];
            if (currentItem) {
                const animatedElements = currentItem.querySelectorAll('.animate-fade-in');
                animatedElements.forEach(el => {
                    el.style.opacity = 0;
                    void el.offsetWidth; // Trigger reflow
                    el.style.opacity = '';
                });
            }
        }

        // Slide functions
        function prevSlide() {
            carouselState.currentIndex = (carouselState.currentIndex - 1 + totalItems) % totalItems;
            updateCarousel();
        }

        function nextSlide() {
            carouselState.currentIndex = (carouselState.currentIndex + 1) % totalItems;
            updateCarousel();
        }

        // Reset interval function
        function resetInterval() {
            clearInterval(carouselState.slideInterval);
            // carouselState.slideInterval = setInterval(nextSlide, 5000);
        }

        // Event listeners for controls
        if (carouselState.prevControl) {
            carouselState.prevControl.addEventListener('click', function() {
                prevSlide();
                resetInterval();
            });
        }

        if (carouselState.nextControl) {
            carouselState.nextControl.addEventListener('click', function() {
                nextSlide();
                resetInterval();
            });
        }

        // Event listeners for indicators
        carouselState.indicators.forEach((indicator, index) => {
            indicator.addEventListener('click', function() {
                carouselState.currentIndex = index;
                updateCarousel();
                resetInterval();
            });
        });

        // Initialize carousel
        updateCarousel();

        // Setup automatic sliding
        carouselState.slideInterval = setInterval(nextSlide, 5000);
    }

    // Initialize all carousels when DOM is loaded
    initAllCarousels();

    // Function to check for new carousels (useful for dynamically added content)
    function checkForNewCarousels() {
        const allCarousels = document.querySelectorAll('.carousel');
        const currentCount = initializedCarousels.size;

        if (allCarousels.length > currentCount) {
            initAllCarousels();
        }
    }
    var interval = setInterval(checkForNewCarousels, 2000)
    return () => {
        clearInterval(interval);
    };
}

export default function Home() {
    useEffect(() => {
      return init();
    }, []);

  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={`Hello from ${siteConfig.title}`}
      description="Description will go into a meta tag in <head />">
      <HomepageHeader />
      <main>
        <HomepageOpensource />
        <HomepageEnterprise />
        <HomepageInstallation />
        <HomePageMigration />
        <HomepageFullyCustom />
        <HomepageFeatures />
        <HomepageOpenStandards />
      </main>
    </Layout>
  );
}
