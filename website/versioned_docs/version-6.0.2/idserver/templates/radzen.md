# Radzen CSS Template

When you install and configure your Identity Server with one or more authentication methods, a clean, responsive stylesheet is applied by default. 
Out of the box, SimpleIdServer uses the Radzen CSS template—no extra NuGet packages are required. 
This article explains how the Radzen template is loaded and extended on your login and registration pages.

SimpleIdServer ships with [Radzen's](https://www.radzen.com/) default styling to ensure a modern look and feel immediately. You don’t need to install or reference any additional packages; the Radzen CSS is already bundled into the server’s content pipeline.

## What Happens When the Page Loads

Every time an end‑user navigates to an authentication or registration page, the server’s HTML includes a small snippet that orchestrates three key CSS operations:

1. **Load the Radzen Default Stylesheet** - A link to Radzen’s core CSS is injected. This file provides all the base styles for forms, buttons, inputs, and layout utilities.

```css
<link href="/_content/Radzen.Blazor/css/default.css" rel="stylesheet" />
```

2. **Include Your Custom CSS** : Immediately after the default style, the server loads your personalized stylesheet. This allows you to override colors, spacing, or typography to match your brand guidelines.

```css
<link href="/custom-styles/auth.css" rel="stylesheet" />
```

3. **Apply CSS Classes to Form Components** : Each form element (titles, paragraphs, inputs, buttons, etc.) is automatically assigned CSS classes. These classes map back to both Radzen’s defaults and any custom rules you’ve defined—ensuring consistency across every authentication flow.