import React from 'react';

export default function HomepageFullyCustom() {
  return (    
    <section class="py-16 px-4 sm:px-6 lg:px-8 bg-gray-50">
        <div class="max-w-7xl mx-auto">
            <div class="text-center mb-12">
                <h2 class="text-3xl font-extrabold text-gray-900 sm:text-4xl">
                    Fully Customizable Authentication Experience
                </h2>
                <p class="mt-4 max-w-2xl text-xl text-gray-500 mx-auto">
                    Tailor every aspect of your authentication forms and workflows to match your brand and security requirements
                </p>
            </div>
            
            <div class="lg:grid lg:grid-cols-2 lg:gap-12">
                <div>
                    <div class="auth-form-mockup mb-8">
                        <div class="form-field">
                            <label class="form-label">Email Address</label>
                            <input type="email" class="form-input" placeholder="your@email.com" />
                        </div>                        
                        <div class="form-field">
                            <label class="form-label">Password</label>
                            <input type="password" class="form-input" placeholder="••••••••" />
                        </div>
                        <div class="flex items-center">
                            <input type="checkbox" id="remember-me" class="h-4 w-4 text-primary-500 focus:ring-primary-500 border-gray-300 rounded" />
                            <label for="remember-me" class="ml-2 block text-sm text-gray-700">Remember me</label>
                        </div>
                        
                        <button class="auth-btn">Sign In</button>

                        <div class="mt-4 text-center text-sm text-gray-500">
                            Or
                        </div>

                        <a href="#" class="text-sm text-primary-500 hover:text-primary-700">Forgot password?</a>
                        
                        <div class="mt-4 text-center text-sm text-gray-500">
                            Or continue with
                        </div>
                        
                        <div class="grid grid-cols-3 gap-2 mt-4">
                            <button class="flex items-center justify-center py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50">
                                <i class="fab fa-google text-red-500 mr-2"></i> Google
                            </button>
                            <button class="flex items-center justify-center py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50">
                                <i class="fab fa-microsoft text-blue-500 mr-2"></i> Microsoft
                            </button>
                            <button class="flex items-center justify-center py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50">
                                <i class="fab fa-apple text-gray-900 mr-2"></i> Apple
                            </button>
                        </div>
                    </div>
                    
                    <p class="text-gray-500 text-center">
                        This is just one example of the many customizable authentication screens available
                    </p>
                </div>
                
                <div>
                    <h3 class="text-xl font-bold text-gray-900 mb-6">Complete Control Over Authentication Processes</h3>
                    
                    <div class="process-step">
                        <div class="step-number">1</div>
                        <div class="step-content">
                            <h4 class="step-title">Branding & Styling</h4>
                            <p class="step-description">
                                Fully customize colors, fonts, logos, and layouts to match your brand identity. CSS overrides give you pixel-perfect control.
                            </p>
                        </div>
                    </div>
                    
                    <div class="process-step">
                        <div class="step-number">2</div>
                        <div class="step-content">
                            <h4 class="step-title">Multi-step Workflows</h4>
                            <p class="step-description">
                                Design complex authentication journeys with conditional steps, progressive profiling.
                            </p>
                        </div>
                    </div>
                    
                    <div class="process-step">
                        <div class="step-number">3</div>
                        <div class="step-content">
                            <h4 class="step-title">Social & Enterprise Providers</h4>
                            <p class="step-description">
                                Easily integrate with any OAuth/OpenID Connect provider or SAML identity provider with custom configuration options.
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
  );
}