import React from 'react';

export default function HomepageOpenStandards() {
    return (
        <section id="standards" className="py-20 bg-white">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="lg:text-center">
                    <h2 className="text-base text-primary-600 font-semibold tracking-wide uppercase">Supported Standards</h2>
                    <p className="mt-2 text-3xl leading-8 font-extrabold tracking-tight text-gray-900 sm:text-4xl">
                        Industry-Standard Protocols
                    </p>
                    <p className="mt-4 max-w-2xl text-xl text-gray-500 lg:mx-auto">
                        SimpleIdServer supports all major identity and access management standards for seamless integration.
                    </p>
                </div>    
                <div className="mt-16">
                    <div className="grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-3">
                        <div className="bg-white p-8 rounded-lg shadow-sm hover:shadow-md transition-shadow duration-300">
                            <div className="standard-icon bg-blue-100 text-blue-600">
                                <i className="fas fa-id-card fa-2x"></i>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900">OpenID Connect</h3>
                            <p className="mt-2 text-gray-600">
                                SimpleIdServer is certified OpenID Provider supporting all core OpenID Connect flows including Authorization Code, Implicit, Hybrid and Client Credentials.
                            </p>
                        </div>
                        <div className="bg-white p-8 rounded-lg shadow-sm hover:shadow-md transition-shadow duration-300">
                            <div className="standard-icon bg-green-100 text-green-600">
                                <i className="fas fa-lock fa-2x"></i>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900">OAuth 2.0</h3>
                            <p className="mt-2 text-gray-600">
                                Full OAuth 2.0 implementation with support for all grant types including PKCE, Device Code, and Token Exchange.
                            </p>
                        </div>
                        <div className="bg-white p-8 rounded-lg shadow-sm hover:shadow-md transition-shadow duration-300">
                            <div className="standard-icon bg-purple-100 text-purple-600">
                                <i className="fas fa-fingerprint fa-2x"></i>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900">FIDO2/WebAuthn</h3>
                            <p className="mt-2 text-gray-600">
                                Passwordless authentication with FIDO2 standards. Supports security keys, biometrics, and platform authenticators.
                            </p>
                        </div>
                        <div className="bg-white p-8 rounded-lg shadow-sm hover:shadow-md transition-shadow duration-300">
                            <div className="standard-icon bg-yellow-100 text-yellow-600">
                                <i className="fas fa-exchange-alt fa-2x"></i>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900">SAML 2.0</h3>
                            <p className="mt-2 text-gray-600">
                                Enterprise-grade SAML 2.0 support for both Identity Provider (IdP) and Service Provider (SP) roles with metadata exchange.
                            </p>
                        </div>
                        <div className="bg-white p-8 rounded-lg shadow-sm hover:shadow-md transition-shadow duration-300">
                            <div className="standard-icon bg-red-100 text-red-600">
                                <i className="fas fa-users-cog fa-2x"></i>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900">SCIM 2.0</h3>
                            <p className="mt-2 text-gray-600">
                                System for Cross-domain Identity Management for automated user provisioning and synchronization across systems.
                            </p>
                        </div>
                        <div className="bg-white p-8 rounded-lg shadow-sm hover:shadow-md transition-shadow duration-300">
                            <div className="standard-icon bg-indigo-100 text-indigo-600">
                                <i className="fas fa-link fa-2x"></i>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900">EBSI</h3>
                            <p className="mt-2 text-gray-600">
                                European Blockchain Services Infrastructure support for decentralized identity and verifiable credentials.
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </section>);
}