import React from 'react';
import Link from '@docusaurus/Link';

function DocsCard(props)  {
   return  (<div className="feature-card transition-all duration-300 ease-in-out p-6 bg-white rounded-lg border border-gray-200 hover:border-primary-200">
    <div className="mt-6">
      <Link to={props.href}>
        <h3 className="text-lg font-medium text-gray-900">{props.header}</h3>
        <p className="mt-2 text-base text-gray-500">{props.children}</p>
      </Link>
    </div>
  </div>)
}
  
export default DocsCard;