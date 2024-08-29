import React from 'react';
import clsx from 'clsx';
import styles from './styles.module.css';

const FeatureList = [
  {
    title: 'Single-Sign On',
    Svg: require('@site/static/img/sso.svg').default,
    description: (
      <>
        User authentication and authorization mechanism that allows individuals to access multiple applications or systems with just one set of login credentials.
      </>
    ),
  },
  {
    title: 'Identity Management',
    Svg: require('@site/static/img/identity-card.svg').default,
    description: (
      <>
         Manage the lifecycle of digital identities within an organization. To accomplish this, we have chosen SCIM Version 2.0 as the preferred protocol for identity management.
      </>
    ),
  },
  {
    title: 'Identity Provisioning',
    Svg: require('@site/static/img/id-provisioning.svg').default,
    description: (
      <>
        Refers to the process of creating, modifying, and managing user identities and their associated access privileges within an organization's systems and applications. 
      </>
    ),
  },
  {
    title: 'Financial-grade API (FAPI)',
    Svg: require('@site/static/img/fapi.svg').default,
    description: (
      <>
        Set of standards and guidelines developed by the OpenID Foundation to enhance the security and interoperability of APIs in the financial industry. 
      </>
    ),
  },
  {
    title: 'Verifiable Credential Issuer',
    Svg: require('@site/static/img/credentialissuer.svg').default,
    description: (
      <>
        Entity or organization that issues digital credentials in a verifiable and tamper-proof format, for examples educational degrees, professional certifications or government-issued identification documents.
      </>
    ),
  },
  {
    title: 'Workspace',
    Svg: require('@site/static/img/workspace.svg').default,
    description: (
      <>
        SimpleIdServer has introduced the concept of realms, which serves the purpose of segregating various resources, including clients and users, into distinct realms.
      </>
    ),
  },
  {
    title: 'Mobile Application',    
    Svg: require('@site/static/img/credentialissuer.svg').default,
    description: (
      <>
        The mobile application can function as an authentication device, utilizing public-key encryption as outlined in the <a href="https://fidoalliance.org/specifications/">FIDO U2F authentication standard</a>.
      </>
    )
  },
  {
    title: 'Electronic wallet',    
    Svg: require('@site/static/img/credentialissuer.svg').default,
    description: (
      <>
        TODO.
      </>
    )
  }
];

function Feature({Svg, title, description, full}) {
  return (
    <div className={full == true ? clsx('col col--12') : clsx('col col--4')}>
      <div className="text--center">
        <Svg className={styles.featureSvg} role="img" />
      </div>
      <div className="text--center padding-horiz--md">
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures() {
  return (
    <section className={styles.features}>
      <div className="container">
        <h1>Featured Capabilities</h1>
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}