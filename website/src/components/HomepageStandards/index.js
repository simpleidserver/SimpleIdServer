import React from 'react';
import clsx from 'clsx';

const StandardList = [
  {
    title: 'OpenID',
    Svg: require('@site/static/img/sso.svg').default,
    description: (
      <>
      </>
    ),
  },
  {
    title: 'SAML',
    Svg: require('@site/static/img/id-provisioning.svg').default,
    description: (
      <>
      </>
    ),
  },
  {
    title: 'FIDO',
    Svg: require('@site/static/img/fapi.svg').default,
    description: (
      <>
      </>
    ),
  },
  {
    title: 'OAuth',
    Svg: require('@site/static/img/workspace.svg').default,
    description: (
      <>
      </>
    ),
    full: true
  }
];

function Standard({Svg, title, description, full}) {
  return (
    <div className={full == true ? clsx('col col--12') : clsx('col col--4')}>
      <div className="text--center">
      </div>
      <div className="text--center padding-horiz--md">
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageStandards() {
    return (
      <section>
        <div className="container">
          <h1>Open Standards</h1>
          <div className="row">
            {StandardList.map((props, idx) => (
              <Standard key={idx} {...props} />
            ))}
          </div>
        </div>
      </section>);
}