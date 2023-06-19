import React from 'react';

import './cards.css';

function DocsCards(props) {
  return <docs-cards class={props.className}>{props.children}</docs-cards>;
}

export default DocsCards;