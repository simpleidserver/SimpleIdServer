import React, { useEffect } from 'react';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import HomepageFeatures from '@site/src/components/HomepageFeatures';
import HomepageEnterprise from '@site/src/components/HomepageEnterprise';
import HomepageOpensource from '@site/src/components/HomepageOpensource';
import HomepageInstallation from '@site/src/components/HomepageInstallation';
import HomepageFullyCustom from '@site/src/components/HomepageFullyCustom';
import HomepageOpenStandards from '../components/HomepageOpenStandards';

function HomepageHeader() {
  return (    
    <header className="hero-gradient py-20 px-4 sm:px-6 lg:px-8">
        <div className="max-w-7xl mx-auto">
            <div className="lg:grid lg:grid-cols-12 lg:gap-8">
                <div className="px-4 sm:px-0 sm:text-center md:max-w-2xl md:mx-auto lg:col-span-6 lg:text-left lg:flex lg:items-center">
                    <div>
                        <h1 className="text-4xl tracking-tight font-extrabold text-gray-900 sm:text-5xl md:text-6xl">
                            <span className="block">Secure Your Digital</span>
                            <span className="block text-primary-600">Identity Ecosystem</span>
                        </h1>
                        <p className="mt-3 text-base text-gray-500 sm:mt-5 sm:text-lg sm:max-w-xl sm:mx-auto md:mt-5 md:text-xl lg:mx-0">
                            SimpleIdServer provides modern identity and access management solutions that are secure, scalable, and developer-friendly.
                        </p>
                        <div className="mt-8 sm:max-w-lg sm:mx-auto sm:text-center lg:text-left lg:mx-0">
                            <div className="flex flex-col sm:flex-row sm:justify-center lg:justify-start space-y-3 sm:space-y-0 sm:space-x-3">
                                <a href="/docs/overview" className="w-full flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 md:py-4 md:text-lg md:px-10 transition duration-150 ease-in-out">
                                    Get Started
                                </a>
                                <a href="https://website.simpleidserver.com/master/clients" className="w-full flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-md text-primary-700 bg-primary-100 hover:bg-primary-200 md:py-4 md:text-lg md:px-10 transition duration-150 ease-in-out">
                                    Live Demo
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="mt-12 lg:mt-0 lg:col-span-6">
                    <div className="bg-white shadow-xl rounded-lg overflow-hidden glow-effect">
                        <div className="p-5 bg-gray-50 border-b border-gray-200">
                            <div className="flex space-x-2">
                                <div className="w-3 h-3 rounded-full bg-red-400"></div>
                                <div className="w-3 h-3 rounded-full bg-yellow-400"></div>
                                <div className="w-3 h-3 rounded-full bg-green-400"></div>
                            </div>
                        </div>
                        
                        <div className="p-6">                   
                            <div className="carousel">
                                <div className="carousel-inner">
                                    <div className="carousel-item active">
                                        <img src="/img/auth.png" />
                                    </div>
                                    <div className="carousel-item">
                                        <img src="/img/clients.png" />
                                    </div>
                                </div>
                            </div>       
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </header>
  );
}

function init() {
    let currentIndex = 0;
    let installationIndex = 0;
    let installationInterval;
    function showNextSlide() {
        const carouselItems = document.querySelectorAll('.carousel-item');
        if(carouselItems.length === 0) {
            return;
        }
        
        carouselItems[currentIndex].classList.remove('active');
        currentIndex = (currentIndex + 1) % carouselItems.length;
        carouselItems[currentIndex].classList.add('active');
    }

    function initIndicators() {
        const indicators = document.querySelectorAll('.carousel-indicator');
        if(indicators.length === 0) {
            setInterval(initIndicators, 100);
            return;
        }

        indicators.forEach((indicator, index) => {
            indicator.addEventListener('click', () => {
                const installationItems = document.querySelectorAll('.installation-item');
                const indicators = document.querySelectorAll('.carousel-indicator');
                showInstallationSlide(index, installationItems, indicators);
                initInstallationSlide();
            });
        });
    }

    function showInstallationSlide(index, installationItems, indicators) {
        installationItems.forEach(item => item.classList.remove('active'));
        indicators.forEach(indicator => indicator.classList.remove('active'));
        installationItems[index].classList.add('active');
        indicators[index].classList.add('active');
        installationIndex = index;
    }

    function initInstallationSlide() {
        if(installationInterval) {
            clearInterval(installationInterval);
        }

        installationInterval = setInterval(() => {
            const installationItems = document.querySelectorAll('.installation-item');
            const indicators = document.querySelectorAll('.carousel-indicator');
            if(installationItems.length === 0 || indicators.length === 0) {
                return;
            }
    
            const nextIndex = (installationIndex + 1) % installationItems.length;
            showInstallationSlide(nextIndex, installationItems, indicators);
        }, 5000);
    }

    var interval = setInterval(showNextSlide, 3000);
    initInstallationSlide();
    initIndicators();
    return () => {
        clearInterval(interval);
        if(installationInterval) {
            clearInterval(installationInterval);
        }
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
        <HomepageFullyCustom />
        <HomepageFeatures />
        <HomepageOpenStandards />
      </main>
    </Layout>
  );
}
