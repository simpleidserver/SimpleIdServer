import React from 'react';

import './cards.css';

function DocsCards(props) {
  return <div className="grid grid-cols-1 gap-12 md:grid-cols-2 lg:grid-cols-3">{props.children}</div>;
}

export default DocsCards;