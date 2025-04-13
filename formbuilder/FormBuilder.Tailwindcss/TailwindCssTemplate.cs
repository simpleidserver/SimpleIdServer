// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Models;

namespace FormBuilder.Tailwindcss;

internal static class TailwindCssTemplate
{
    public const string Name = "TailwindCss";

    public static Template DefaultTemplate
    {
        get
        {
            return TemplateBuilder.New(Name)
                .AddJsLibrary("https://cdn.jsdelivr.net/npm/@tailwindcss/browser@4")
                .SetAuthModalClasses("bg-gray-50 dark:bg-gray-900", "flex flex-col items-center justify-center px-6 py-8 mx-auto md:h-screen lg:py-0", "w-full bg-white rounded-lg shadow dark:border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700", "p-6 space-y-4 md:space-y-6 sm:p-8")
                .SetDividerClasses("inline-flex items-center justify-center w-full", "w-64 h-px my-8 bg-gray-200 border-0 dark:bg-gray-700", "absolute px-3 font-medium text-gray-900 -translate-x-1/2 bg-white left-1/2 dark:text-white dark:bg-gray-800")
                .SetInputTextFieldClasses("form-group", "block mb-2 text-sm font-medium text-gray-900 dark:text-white", "bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500")
                .SetAnchorClass("text-sm font-medium text-primary-600 hover:underline dark:text-primary-500")
                .Build();
        }
    }
}