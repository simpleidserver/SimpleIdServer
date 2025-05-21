export default function HomePageMigration() {
  return (  
    <section id="migration" class="py-16 px-4 sm:px-6 lg:px-8 bg-white">
        <div class="max-w-7xl mx-auto">
            <div class="text-center">
                <h2 class="text-3xl font-extrabold text-gray-900 sm:text-4xl">
                    Migration Made Easy
                </h2>
                <p class="mt-4 max-w-2xl text-xl text-gray-500 mx-auto">
                    Transition smoothly from other identity solutions with our comprehensive migration tools
                </p>
            </div>            
            <div class="migration-container">
                <div class="migration-sources">
                    <a href="https://duendesoftware.com/products/identityserver" target="_blank" class="migration-step">
                        <div class="migration-icon">
                            <img src="/img/duende.png" />
                        </div>
                        <div class="migration-label">Duende</div>
                    </a>
                    <a href="https://documentation.openiddict.com/" target="_blank" class="migration-step">
                        <div class="migration-icon">
                            <img src="/img/openiddct.png" />
                        </div>
                        <div class="migration-label">OpenIddict</div>
                    </a>
                </div>
                <div class="migration-arrow-container">
                    <div class="arrow-line"></div>
                    <div class="arrow-head"></div>
                    <div class="data-particle user">
                        <i class="fas fa-user"></i>
                    </div>
                    <div class="data-particle app">
                        <i class="fas fa-mobile-alt"></i>
                    </div>
                    <div class="data-particle scope">
                        <div class="scope-bg"></div>
                        <div class="scope-icon">
                            <i class="fas fa-shield-alt"></i>
                        </div>
                    </div>
                    <div class="data-particle user">
                        <i class="fas fa-user"></i>
                    </div>
                    <div class="data-particle app">
                        <i class="fas fa-desktop"></i>
                    </div>
                </div>
                <div class="migration-step">
                    <div class="migration-icon migration-destination">
                        <img src="/img/logo-no-shield.png" />
                    </div>
                    <div class="migration-label">SimpleIdServer</div>
                </div>
            </div>
        </div>
    </section>
  );
}