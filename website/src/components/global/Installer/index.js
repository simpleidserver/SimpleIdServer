import React from 'react';

import styles from './styles.module.css';

function Installer(props) {  
  return <a href={props.link} className={styles["installer"]} target="_blank">
    <span className={styles['installerIcon'] + " " + styles[props.icon]}></span>
    <h3 className={styles["installerTitle"]}>{props.title}</h3>
  </a>;
}

export default Installer;