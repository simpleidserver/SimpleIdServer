import React from 'react';
import styles from './styles.module.css';
const OpenId = require('@site/static/img/openid.svg').default;
const OAuth = require('@site/static/img/oauth.svg').default;
const Fido = require('@site/static/img/fido.svg').default;
const Saml = require('@site/static/img/saml.svg').default;
const Docker = require('@site/static/img/docker.svg').default;

export default function HomepageOpenStandards() {
    return (<section>
        <div className="container">
            <h1>Open Standards</h1>
            <div className="row">
                <div className="col col--6">
                    <div className="text--center">
                        <OpenId className={styles.standardSvg} />
                    </div>
                    <div className="text--center padding-horiz--md">
                        <h3>OPENID</h3>
                        <p>OpenID is a popular federated identity standard for web-based applications.</p>
                    </div>
                </div>
                <div className="col col--6">
                    <div className="text--center">
                        <OAuth className={styles.standardSvg} />
                    </div>
                    <div className="text--center padding-horiz--md">
                        <h3>OAUTH</h3>
                        <p>OAuth is used to authenticate a person using a mobile device, or to authenticate software–identity is not just for humans</p>
                    </div>
                </div>
                <div className="col col--6">
                    <div className="text--center">
                        <Fido className={styles.standardSvg} />
                    </div>
                    <div className="text--center padding-horiz--md">
                        <h3>FIDO</h3>
                        <p>A set of standards that enable the hardware, operating system, browser, and identity provider to authenticate a person using phishing-resistent, client-side biometrics.</p>
                    </div>
                </div>
                <div className="col col--6">
                    <div className="text--center">
                        <Saml className={styles.standardSvg} />
                    </div>
                    <div className="text--center padding-horiz--md">
                        <h3>SAML</h3>
                        <p>SAML, the XML federation ancestor of OpenID Connect, is used primiarily by SaaS and older web-based enterprise applications for SSO.</p>
                    </div>
                </div>
                <div className="col col--12">
                    <div className="text--center">
                        <img src="/img/scim.webp" className={styles.standardImg} />
                    </div>
                    <div className="text--center padding-horiz--md">
                        <h3>SCIM2.0</h3>
                        <p>The SCIM protocol is an application-level REST protocol for provisioning and managing identity data on the web. </p>
                    </div>
                </div>
            </div>
        </div>
    </section>);
}