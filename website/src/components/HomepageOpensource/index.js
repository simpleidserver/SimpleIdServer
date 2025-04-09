import React from 'react';

export default function HomepageOpensource() {
    return (      
      <section class="py-12 px-4 sm:px-6 lg:px-8 bg-gray-50">
        <div class="max-w-7xl mx-auto">
            <div class="lg:flex lg:items-center lg:justify-between">
                <div class="lg:w-1/2">
                    <h2 class="text-3xl font-extrabold text-gray-900 sm:text-4xl">
                        Open Source Identity Solution
                    </h2>
                    <p class="mt-4 text-lg text-gray-500">
                        SimpleIdServer is a fully open-source identity and access management solution licensed under Apache 2.0, giving you complete control and flexibility over your identity infrastructure.
                    </p>
                    <div class="mt-6">
                        <div class="flex flex-wrap">
                            <span class="tech-badge">
                                <i class="fab fa-github"></i> GitHub
                            </span>
                            <span class="tech-badge">
                                <i class="fas fa-balance-scale"></i> Apache 2.0
                            </span>
                            <span class="tech-badge">
                                <i class="fab fa-microsoft"></i> .NET Core
                            </span>
                            <span class="tech-badge">
                                <i class="fas fa-code-branch"></i> MIT Licensed
                            </span>
                        </div>
                    </div>
                    <div class="mt-8">
                        <a href="https://github.com/simpleidserver/SimpleIdServer" class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-primary-500 hover:bg-primary-600">
                            <i class="fab fa-github mr-2"></i> View on GitHub
                        </a>
                        <a href="/docs/overview" class="ml-3 inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50">
                            <i class="fas fa-book mr-2"></i> Documentation
                        </a>
                    </div>
                </div>
                <div class="mt-8 lg:mt-0 lg:w-1/2 lg:pl-12">
                    <div class="bg-white p-6 rounded-lg shadow-md">
                        <h3 class="text-lg font-medium text-gray-900 mb-4">Why Open Source?</h3>
                        <ul class="space-y-4">
                            <li class="flex items-start">
                                <div class="flex-shrink-0 h-6 w-6 text-primary-500">
                                    <i class="fas fa-check-circle"></i>
                                </div>
                                <p class="ml-3 text-base text-gray-500">
                                    <span class="font-medium text-gray-900">Transparency:</span> Full visibility into the codebase and security implementations
                                </p>
                            </li>
                            <li class="flex items-start">
                                <div class="flex-shrink-0 h-6 w-6 text-primary-500">
                                    <i class="fas fa-check-circle"></i>
                                </div>
                                <p class="ml-3 text-base text-gray-500">
                                    <span class="font-medium text-gray-900">Flexibility:</span> Customize and extend to meet your specific requirements
                                </p>
                            </li>
                            <li class="flex items-start">
                                <div class="flex-shrink-0 h-6 w-6 text-primary-500">
                                    <i class="fas fa-check-circle"></i>
                                </div>
                                <p class="ml-3 text-base text-gray-500">
                                    <span class="font-medium text-gray-900">Community:</span> Benefit from contributions and peer reviews
                                </p>
                            </li>
                            <li class="flex items-start">
                                <div class="flex-shrink-0 h-6 w-6 text-primary-500">
                                    <i class="fas fa-check-circle"></i>
                                </div>
                                <p class="ml-3 text-base text-gray-500">
                                    <span class="font-medium text-gray-900">No Vendor Lock-in:</span> Own your identity infrastructure
                                </p>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
      </section>
    );
}