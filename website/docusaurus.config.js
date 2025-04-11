// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion
const {themes} = require('prism-react-renderer');
const lightTheme = themes.github;
const darkTheme = themes.dracula;

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'SimpleIdServer',
  tagline: 'Introducing the First Open Source Identity and Access Management Solution Developed with .NET',
  favicon: 'img/favicon.ico',
  // Set the production url of your site here
  url: 'https://simpleidserver.com',
  // Set the /<baseUrl>/ pathname under which your site is served
  // For GitHub pages deployment, it is often '/<projectName>/'
  baseUrl: '/',

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: 'simpleidserver', // Usually your GitHub org/user name.
  projectName: 'simpleidserver', // Usually your repo name.

  onBrokenLinks: 'ignore',
  onBrokenMarkdownLinks: 'warn',
  markdown: {
    mermaid: true
  },
  // Even if you don't use internalization, you can use this field to set useful
  // metadata like html lang. For example, if your site is Chinese, you may want
  // to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: require.resolve('./sidebars.js'),
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl:
            'https://github.com/facebook/docusaurus/tree/main/packages/create-docusaurus/templates/shared/',
        },
        blog: {
          showReadingTime: true,
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl:
            'https://github.com/facebook/docusaurus/tree/main/packages/create-docusaurus/templates/shared/',
        },
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
        sitemap: {
          changefreq: 'weekly',
          priority: 0.5,
          ignorePatterns: ['/tags/**'],
          filename: 'sitemap.xml',
        },
      })
    ]
  ],
  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      metadata: [{name: 'keywords', content: 'openid, oauth2, scim, fapi, iam, identity, access, simpleidserver'}],
      colorMode: {
        disableSwitch: true
      },
      // Replace with your project's social card
      navbar: {
        title: 'SimpleIdServer',
        logo: {
          alt: 'SimpleIdServer',
          src: 'img/logo-no-shield.svg',
        },
        items: [
          { type: 'doc', docId: 'overview', label: 'Documentation', position: 'left' },
          { type: 'doc', docId: 'download', label: 'Download', position: 'left' },
          { type: 'doc', docId: 'consultancy', label: 'Consultancy', position: 'left' },
          { type: 'doc', docId: 'tutorial/overview', label: 'Tutorial', position: 'left' },
          { type: 'doc', docId: 'comparison', label: 'Comparison', position: 'left' },
          { type: 'doc', docId: 'contactus', label: 'Contact us', position: 'left' },
          {
            type: 'docsVersionDropdown',
            position: 'right',
            dropdownItemsAfter: [{to: '/versions', label: 'All versions'}],
            dropdownActiveClassDisabled: true,
          },
          { to: 'blog', label: 'Blog', position: 'left'},
          {
            href: 'https://github.com/simpleidserver',
            label: 'GitHub',
            position: 'right',
          }
        ],
      },      
      docs: {
        sidebar: {
          hideable: true,
        },
      },
      footer: {
        style: 'dark',
        links: [
          {
            title: 'Docs',
            items: [
              {
                label: 'Documentation',
                to: '/docs/installation',
              },
              {
                label: 'Privacy',
                to: '/privacy',
              },
            ],
          },
          {
            title: 'Community',
            items: [
              {
                label: 'Gitter',
                href: 'https://app.gitter.im/#/room/#simpleidserver:gitter.im',
              }
            ],
          },
          {
            title: 'More',
            items: [
              {
                label: 'Blog',
                to: '/blog',
              },
              {
                label: 'GitHub',
                href: 'https://github.com/simpleidserver',
              },
            ],
          }
        ],
        copyright: `Copyright © ${new Date().getFullYear()} Lokit - BE0794.185.124`,
      },
      prism: {
        theme: lightTheme,
        darkTheme: darkTheme,
        additionalLanguages: ['csharp', 'batch']
      }
	}),
  scripts: [
    {
      src: 'https://cdn.tailwindcss.com',
      async: true
    },
    {
      src: '/website.js',
      async: true
    }
  ],
  stylesheets: [
    {
      href: 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css'
    }
  ]
};

module.exports = config;
