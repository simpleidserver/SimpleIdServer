# Tailwind CSS Template

By default, SimpleIdServer ships with the Radzen CSS template. If you prefer the utility‑first approach of [Tailwind CSS](https://tailwindcss.com/), you can easily swap in a Tailwind‑based stylesheet for all your authentication and registration forms. This guide shows you how to install the Tailwind CSS template package, register it in your application, and explains what happens under the hood when a user hits your login or signup page.

## Install the Tailwind CSS Form Builder Package

First, add the Tailwind CSS integration package to your Identity Server project. From the root of your web application, run:

```batch title="cmd.exe"
dotnet add package FormBuilder.Tailwindcss
```

This NuGet package contains a pre‑configured Tailwind CSS template and the extension methods you’ll need to wire it up.

## Register Tailwind CSS in Program.cs

Open your Program.cs (or wherever you configure your IdentityServerBuilder) and locate the call to `ConfigureFormBuilder`. 
Inside that delegate, call `AddTailwindcss()` to register the Tailwind template:

```csharp  title="Program.cs"
var builder = WebApplication.CreateBuilder(args);

// ... your other IdentityServer setup, e.g. in‑memory clients & scopes

builder.Services
    .AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryClients(clients)
    .AddInMemoryScopes(scopes)
    .ConfigureFormBuilder(c =>
    {
        c.AddTailwindcss();
    });

var app = builder.Build();
// ... app pipeline configuration
app.Run();
```

After rebuilding and restarting your application, the Tailwind CSS template becomes available alongside (or in place of) the default Radzen template.

## What Happens at Runtime

When an end user navigates to any authentication or registration page, the form renderer now performs three key steps:

1. **Load Tailwind via CDN** : This single script fetches the latest Tailwind utilities so you can use classes like bg-blue-500, p-4, or flex directly in your form markup.

```html
<script src="https://cdn.tailwindcss.com"></script>
```

2. **Include Your Custom JavaScript** 

3. **Apply CSS classes to Form components** : Every form element (titles, paragraphs, inputs, buttons, etc.) is annotated with Tailwind utility classes. For example, submit buttons might carry class="mt-4 w-full bg-indigo-600 hover:bg-indigo-700 text-white font-bold py-2 px-4 rounded", ensuring consistent styling across all authentication workflows.