import React from 'react';
import styles from './styles.module.css';
import Link from '@docusaurus/Link';
const Greencheck = require('@site/static/img/greencheck.svg').default;
const Redcheck = require('@site/static/img/redCheck.svg').default;

export default function HomepagePhilosophy() {
    return (<section>
        <div class="container">
            <h1>Why SimpleIdServer</h1>
                SimpleIdServer is the first Free Open Source Identity Management solution using .NET.
                <br />
                <b>The comparison table was written on July 24, 2024.</b>
                <div class="table-container" role="table" aria-label="Destinations">
                    <div class="flex-table header" role="rowgroup">
                        <div class="flex-row first" role="columnheader"></div>
                        <div class="flex-row" role="columnheader">
                            SimpleIdServer
                        </div>
                        <div class="flex-row" role="columnheader">
                            <a href="#">Duende Software</a>
                        </div>
                        <div class="flex-row" role="columnheader">
                            <a href="#">Openiddct</a>
                        </div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">Language</div>
                        <div class="flex-row" role="cell">.NET</div>
                        <div class="flex-row" role="cell">.NET</div>
                        <div class="flex-row" role="cell">.NET</div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">Open source</div>
                        <div class="flex-row" role="cell">Apache 2.0</div>
                        <div class="flex-row" role="cell"><a href="https://duendesoftware.com/license">Product license</a></div>
                        <div class="flex-row" role="cell">Apache 2.0</div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-group" role="group">Open Standards</div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">OAUTH2.0</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">OPENID</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">OPENID FAPI</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell">Not Fully supported</div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">OPENID Credential Issuer</div>
                        <div class="flex-row" role="cell">Preview</div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">WS-Federation</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">SAM2.0</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">SCIM2.0</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">Openid federation</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-group" role="group">Identity Provisioning</div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">Manual identity provisioning</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">Automatic identity provisioning</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-group" role="group">Other Features</div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">Administration website</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">Mobile application</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                    <div class="flex-table" role="rowgroup">
                        <div class="flex-row first" role="cell">Multi tenant</div>
                        <div class="flex-row" role="cell"><Greencheck className={styles.greencheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                        <div class="flex-row" role="cell"><Redcheck className={styles.redCheck} /></div>
                    </div>
                </div>
        </div>
    </section>);
}