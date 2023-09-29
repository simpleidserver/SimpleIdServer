import React from 'react';
import styles from './styles.module.css';
const OnPremise = require('@site/static/img/onpremise.svg').default;
const Docker = require('@site/static/img/docker.svg').default;

export default function HomepageInstallations() {
    return (<section>
        <div className="container">
            <h1>Installations</h1>
            <div className="row">
                <div className="col col--6">
                    <div className="text--center">
                        <OnPremise className={styles.installationSvg} />
                    </div>
                    <div className="text--center padding-horiz--md">
                        <h3>On-Premise</h3>
                        <p>The Dotnet template tool is utilized for swiftly installing a local version.</p>
                    </div>
                </div>
                <div className="col col--6">
                    <div className="text--center">
                        <Docker className={styles.installationSvg} />
                    </div>
                    <div className="text--center padding-horiz--md">
                        <h3>Docker</h3>
                        <p>The Docker-compose file enables the rapid deployment of a local version</p>
                    </div>
                </div>
            </div>
        </div>
    </section>);
}