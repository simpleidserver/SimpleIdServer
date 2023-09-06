import React from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import HomepageFeatures from '@site/src/components/HomepageFeatures';
import HomepageInstallations from '@site/src/components/HomepageInstallations';

import styles from './index.module.css';
import HomepagePhilosophy from '../components/HomepagePhilosophy';

function HomepageHeader() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={clsx('hero hero--primary', styles.sidBanner)}>
      <div className="container">
        <h1 className="hero__title">{siteConfig.title}</h1>
        <p className="hero__subtitle">{siteConfig.tagline}</p>
        <div className={styles.buttons}>
          <Link
            className="button button--secondary button--lg"
            to="https://website.simpleidserver.com">
            Demo Administration Website
          </Link>
          <Link 
            className="button button--secondary button--lg"
            to="https://appetize.io/app/s4nfjkzht6ypivfquiroblw77u?device=pixel4&osVersion=11.0&scale=75">
              Demo Mobile Application
          </Link>
        </div>
      </div>
    </header>
  );
}

export default function Home() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={`Hello from ${siteConfig.title}`}
      description="Description will go into a meta tag in <head />">
      <HomepageHeader />
      <main>
        <HomepagePhilosophy />
        <HomepageInstallations />
        <HomepageFeatures />
      </main>
    </Layout>
  );
}
