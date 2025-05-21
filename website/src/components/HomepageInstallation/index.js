export default function HomepageInstallation() {
  return (  
    <section class="py-16 px-4 sm:px-6 lg:px-8 bg-gray-50">
        <div class="max-w-7xl mx-auto">
            <div class="text-center mb-12">
                <h2 class="text-3xl font-extrabold text-gray-900 sm:text-4xl">
                    Easy Installation
                </h2>
                <p class="mt-4 max-w-2xl text-xl text-gray-500 mx-auto">
                    Get started with SimpleIdServer in minutes using your preferred method
                </p>
            </div>
            
            <div class="carousel installation-carousel">
                <div class="carousel-inner installation-carousel-inner">
                    <div class="carousel-item installation-item active" data-index="0">
                        <div class="text-primary-500 mb-4">
                            <i class="fas fa-terminal text-4xl"></i>
                        </div>
                        <h3 class="text-2xl font-bold text-white mb-6">.NET Template Installation</h3>
                        <div class="command-box">
                            <i class="fas fa-angle-right"></i> dotnet new -i SimpleIdServer.Templates
                        </div>
                        <p class="text-gray-300 max-w-2xl">
                            Install our .NET templates to quickly create any type of SimpleIdServer project with all dependencies pre-configured.
                        </p>
                    </div>
                    <div class="carousel-item installation-item">
                        <div class="text-primary-500 mb-4">
                            <i class="fab fa-docker text-4xl"></i>
                        </div>
                        <h3 class="text-2xl font-bold text-white mb-6">Docker Installation</h3>
                        <div class="command-box">
                            <i class="fas fa-angle-right"></i> docker-compose up -d
                        </div>
                        <p class="text-gray-300 max-w-2xl">
                            Run SimpleIdServer in containers with our pre-configured Docker setup. Perfect for development and production environments.
                        </p>
                    </div>
                    <div class="carousel-item installation-item">
                        <div class="text-primary-500 mb-4">
                            <i class="fas fa-dharmachakra text-4xl"></i>
                        </div>
                        <h3 class="text-2xl font-bold text-white mb-6">Kubernetes Deployment</h3>
                        <div class="command-box">
                            <i class="fas fa-angle-right"></i> kubectl apply -f sid-kubernetes.yaml
                        </div>
                        <p class="text-gray-300 max-w-2xl">
                            Deploy SimpleIdServer on your Kubernetes cluster with our production-ready manifests for high availability.
                        </p>
                    </div>
                    <div class="carousel-item installation-item">
                        <div class="text-primary-500 mb-4">
                            <i class="fas fa-mobile-alt text-4xl"></i>
                        </div>
                        <h3 class="text-2xl font-bold text-white mb-6">Mobile Authenticator</h3>
                        <a href="http://play.google.com/store/apps/details?id=com.simpleidserver.mobile&hl=fr" target="_blank" class="mobile-link">
                            <i class="fab fa-google-play"></i>
                            Download on Google Play
                        </a>
                        <p class="text-gray-300 max-w-2xl mt-6">
                            Use our mobile app for secure multi-factor authentication and passwordless login experiences.
                        </p>
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
    </section>
  );
}