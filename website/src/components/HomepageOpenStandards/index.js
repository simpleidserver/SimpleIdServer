import React from 'react';
import styles from './styles.module.css';
const OpenId = require('@site/static/img/openid.svg').default;
const OAuth = require('@site/static/img/oauth.svg').default;
const Fido = require('@site/static/img/fido.svg').default;
const Saml = require('@site/static/img/saml.svg').default;
const Docker = require('@site/static/img/docker.svg').default;

export default function HomepageOpenStandards() {
    return (<section>
        <div className="container mb-5">
            <h1>Open Standards</h1>
            <div className="row gy-5">
                <div className="col col--6">
                    <div class="card">
                        <div className="text--center">
                            <OpenId className={styles.standardSvg} />
                        </div>
                        <div className="text--center padding-horiz--md">
                            <h3>OPENID</h3>
                            <p>OpenID is a widely used federated identity standard for web-based applications.</p>
                        </div>
                    </div>
                </div>
                <div className="col col--6">
                    <div class="card">
                        <div className="text--center">
                            <OAuth className={styles.standardSvg} />
                        </div>
                        <div className="text--center padding-horiz--md">
                            <h3>OAUTH</h3>
                            <p>OAuth is utilized to authenticate an individual using a mobile device or to authenticate software—identity is not exclusive to humans.</p>
                        </div>
                    </div>
                </div>
                <div className="col col--6">
                    <div class="card">
                        <div className="text--center">
                            <Fido className={styles.standardSvg} />
                        </div>
                        <div className="text--center padding-horiz--md">
                            <h3>FIDO</h3>
                            <p>A set of standards enables the hardware, operating system, browser, and identity provider to authenticate a person using phishing-resistant, client-side biometrics.</p>
                        </div>
                    </div>
                </div>
                <div className="col col--6">
                    <div class="card">
                        <div className="text--center">
                            <Saml className={styles.standardSvg} />
                        </div>
                        <div className="text--center padding-horiz--md">
                            <h3>SAML</h3>
                            <p>SAML, the XML federation predecessor of OpenID Connect, is primarily used by SaaS and older web-based enterprise applications for Single Sign-On (SSO).</p>
                        </div>
                    </div>
                </div>
                <div className="col col--6">
                    <div class="card pt-3">
                        <div className="text--center">
                            <img src="/img/scim.webp" className={styles.standardImg} />
                        </div>
                        <div className="text--center padding-horiz--md">
                            <h3>SCIM2.0</h3>
                            <p>The SCIM protocol is an application-level REST protocol for provisioning and managing identity data on the web.</p>
                        </div>
                    </div>
                </div>
                <div className="col col--6">
                    <div class="card pt-3">
                        <div className="text--center">
                            <img src="/img/logo-ebsi.png" className={styles.standardImg} />
                        </div>
                        <div className="text--center padding-horiz--md">
                            <h3>EBSI</h3>
                            <p>The first public sector blockchain infrastructure in Europe is used by public institutions from different countries to build and issue verifiable credentials, such as diplomas and driving licenses.</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>);
}