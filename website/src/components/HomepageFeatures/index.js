import React from 'react';

const FeatureList = [
  {
    title: 'Single-Sign On',
    Icon: 'fas fa-user-shield text-xl',
    description: (
      <>
        User authentication and authorization mechanism that allows individuals to access multiple applications or systems with just one set of login credentials.
      </>
    ),
  },
  {
    title: 'Identity Management',
    Icon: 'fas fa-users text-xl',
    description: (
      <>
         Manage the lifecycle of digital identities within an organization. To accomplish this, we have chosen SCIM Version 2.0 as the preferred protocol for identity management.
      </>
    ),
  },
  {
    title: 'Identity Provisioning',
    Icon: 'fas fa-gear text-xl',
    description: (
      <>
        Refers to the process of creating, modifying, and managing user identities and their associated access privileges within an organization's systems and applications. 
      </>
    ),
  },
  {
    title: 'Financial-grade API (FAPI)',
    Icon: 'fas fa-dollar-sign text-xl',
    description: (
      <>
        Set of standards and guidelines developed by the OpenID Foundation to enhance the security and interoperability of APIs in the financial industry. 
      </>
    ),
  },
  {
    title: 'Verifiable Credential Issuer',
    Icon: 'fas fa-barcode text-xl',
    description: (
      <>
        Entity or organization that issues digital credentials in a verifiable and tamper-proof format, for examples educational degrees, professional certifications or government-issued identification documents.
      </>
    ),
  },
  {
    title: 'Workspace',
    Icon: 'fas fa-toolbox text-xl',
    description: (
      <>
        SimpleIdServer has introduced the concept of realms, which serves the purpose of segregating various resources, including clients and users, into distinct realms.
      </>
    ),
  },
  {
    title: 'Mobile Application',    
    Icon: 'fas fa-mobile-screen-button text-xl',
    description: (
      <>
        The mobile application can function as an authentication device, utilizing public-key encryption as outlined in the <a href="https://fidoalliance.org/specifications/">FIDO U2F authentication standard</a>.
      </>
    )
  },
  {
    title: 'Electronic wallet',    
    Icon: 'fas fa-wallet text-xl>',
    description: (
      <>
      The mobile application can be used as an electronic wallet that complies with the <a href="https://hub.ebsi.eu/wallet-conformance">EBSI standard</a>.
      </>
    )
  }
];

function Feature({Icon, title, description, full}) {
  return (
    <div className="feature-card transition-all duration-300 ease-in-out p-6 bg-white rounded-lg border border-gray-200 hover:border-primary-200">
      <div className="flex items-center justify-center h-12 w-12 rounded-md bg-primary-500 text-white">
        <i className={Icon}></i>
      </div>
      <div className="mt-6">
        <h3 className="text-lg font-medium text-gray-900">{title}</h3>
        <p className="mt-2 text-base text-gray-500">{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures() {
  return (    
    <section className="py-20 bg-white">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="lg:text-center">
          <h2 className="text-base text-primary-600 font-semibold tracking-wide uppercase">Features</h2>
          <p className="mt-2 text-3xl leading-8 font-extrabold tracking-tight text-gray-900 sm:text-4xl">
              Everything you need for modern IAM
          </p>
          <p className="mt-4 max-w-2xl text-xl text-gray-500 lg:mx-auto">
              SimpleIdServer provides all the identity and access management features your applications need.
          </p>
        </div>
        <div className="mt-20">
          <div className="grid grid-cols-1 gap-12 md:grid-cols-2 lg:grid-cols-3">
            {FeatureList.map((props, idx) => (
              <Feature key={idx} {...props} />
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}