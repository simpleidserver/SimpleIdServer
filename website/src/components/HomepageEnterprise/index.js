import React from 'react';

export default function HomepageEnterprise() {
  return (    
    <section class="py-12 px-4 sm:px-6 lg:px-8 bg-white">
        <div class="max-w-7xl mx-auto">
            <div class="text-center">
                <h2 class="text-3xl font-extrabold text-gray-900 sm:text-4xl">
                    Enterprise Support License
                </h2>
                <p class="mt-4 max-w-2xl text-xl text-gray-500 mx-auto">
                    While SimpleIdServer is fully open-source, we offer an enterprise license for organizations needing premium support
                </p>
            </div>
            <div class="mt-10">
                <div class="bg-white shadow-xl rounded-lg overflow-hidden">
                    <div class="p-8">
                        <div class="md:flex">
                            <div class="md:w-1/2 md:pr-8">
                                <div class="flex items-center">
                                    <div class="bg-primary-100 text-primary-600 p-4 rounded-lg">
                                        <i class="fas fa-crown text-3xl"></i>
                                    </div>
                                    <div class="ml-6">
                                        <h3 class="text-2xl font-bold text-gray-900">Enterprise License</h3>
                                        <div class="mt-2">
                                            <span class="price-tag">$5,000</span>
                                            <span class="text-gray-500">/month</span>
                                        </div>
                                    </div>
                                </div>
                                <p class="mt-6 text-gray-600">
                                    For large organizations that require dedicated support and assistance with deployment.
                                </p>
                                <div class="mt-8">
                                    <a href="mailto:agentsimpleidserver@gmail.com" class="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-md shadow-sm text-white bg-primary-500 hover:bg-primary-600">
                                        Contact Sales
                                    </a>
                                </div>
                            </div>
                            <div class="md:w-1/2 md:pl-8 mt-8 md:mt-0">
                                <h4 class="text-lg font-medium text-gray-900">Enterprise Benefits:</h4>
                                <ul class="mt-4 space-y-4">
                                    <li class="flex items-start">
                                        <div class="flex-shrink-0 h-6 w-6 text-primary-500">
                                            <i class="fas fa-check-circle"></i>
                                        </div>
                                        <p class="ml-3 text-base text-gray-600">
                                            <span class="font-medium text-gray-900">Priority Support:</span> Your requests are handled with the highest priority
                                        </p>
                                    </li>
                                    <li class="flex items-start">
                                        <div class="flex-shrink-0 h-6 w-6 text-primary-500">
                                            <i class="fas fa-check-circle"></i>
                                        </div>
                                        <p class="ml-3 text-base text-gray-600">
                                            <span class="font-medium text-gray-900">Deployment Assistance:</span> Our experts help you deploy and configure SimpleIdServer
                                        </p>
                                    </li>
                                    <li class="flex items-start">
                                        <div class="flex-shrink-0 h-6 w-6 text-primary-500">
                                            <i class="fas fa-check-circle"></i>
                                        </div>
                                        <p class="ml-3 text-base text-gray-600">
                                            <span class="font-medium text-gray-900">Custom Development:</span> Get custom features developed specifically for your needs
                                        </p>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
  );
}