// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Models;

namespace FormBuilder.Tailwindcss;

internal static class TailwindCssTemplate
{
    public const string Name = "TailwindCss";
    private static string _jsContent = "function initTailwind() {\r\n\ttry {\t\r\n\t\ttailwind.config = {\r\n\t\t  darkMode: 'class',\r\n\t\t  theme: {\r\n\t\t\textend: {\r\n\t\t\t  colors: {\r\n\t\t\t\tprimary: {\"50\":\"#eff6ff\",\"100\":\"#dbeafe\",\"200\":\"#bfdbfe\",\"300\":\"#93c5fd\",\"400\":\"#60a5fa\",\"500\":\"#3b82f6\",\"600\":\"#2563eb\",\"700\":\"#1d4ed8\",\"800\":\"#1e40af\",\"900\":\"#1e3a8a\",\"950\":\"#172554\"}\r\n\t\t\t  }\r\n\t\t\t},\r\n\t\t\tfontFamily: {\r\n\t\t\t  'body': [\r\n\t\t\t'Inter', \r\n\t\t\t'ui-sans-serif', \r\n\t\t\t'system-ui', \r\n\t\t\t'-apple-system', \r\n\t\t\t'system-ui', \r\n\t\t\t'Segoe UI', \r\n\t\t\t'Roboto', \r\n\t\t\t'Helvetica Neue', \r\n\t\t\t'Arial', \r\n\t\t\t'Noto Sans', \r\n\t\t\t'sans-serif', \r\n\t\t\t'Apple Color Emoji', \r\n\t\t\t'Segoe UI Emoji', \r\n\t\t\t'Segoe UI Symbol', \r\n\t\t\t'Noto Color Emoji'\r\n\t\t  ],\r\n\t\t\t  'sans': [\r\n\t\t\t'Inter', \r\n\t\t\t'ui-sans-serif', \r\n\t\t\t'system-ui', \r\n\t\t\t'-apple-system', \r\n\t\t\t'system-ui', \r\n\t\t\t'Segoe UI', \r\n\t\t\t'Roboto', \r\n\t\t\t'Helvetica Neue', \r\n\t\t\t'Arial', \r\n\t\t\t'Noto Sans', \r\n\t\t\t'sans-serif', \r\n\t\t\t'Apple Color Emoji', \r\n\t\t\t'Segoe UI Emoji', \r\n\t\t\t'Segoe UI Symbol', \r\n\t\t\t'Noto Color Emoji'\r\n\t\t  ]\r\n\t\t\t}\r\n\t\t  }\r\n\t\t};\r\n\t}\r\n\tcatch(ex) {\t\t\r\n\t\tsetTimeout(() => {\r\n\t\t  initTailwind();\r\n\t\t}, \"1000\");\r\n\t\treturn;\r\n\t}\r\n}\r\n\r\ninitTailwind();";

    public static Template DefaultTemplate
    {
        get
        {
            return TemplateBuilder.New(Name)
                .AddJsLibrary("https://cdn.tailwindcss.com")
                .AddCustomJs(_jsContent)
                .SetAuthModalClasses(
                    "bg-gray-50 dark:bg-gray-900 h-full",
                    "flex flex-col items-center justify-center px-6 py-8 mx-auto h-full lg:py-0",
                    "w-full bg-white rounded-lg shadow dark:border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700",
                    "p-6 space-y-4 md:space-y-6 sm:p-8"
                )
                .SetStepperClasses(
                    "flex items-center w-full text-sm text-gray-500 font-medium sm:text-base",
                    "flex md:w-full items-center text-gray-600",
                    "sm:after:content-[''] after:w-full after:h-1 after:border-b after:border-gray-200 after:border-1 after:hidden sm:after:inline-block after:m-[4px] xl:after:m-[8px]",
                    "flex items-center whitespace-nowrap after:content-['/'] sm:after:hidden after:mx-2",
                    "w-6 h-6 rounded-full border bg-gray-100 border-gray-200 flex justify-center items-center mr-3 text-sm text-white lg:w-10 lg:h-10",
                    "!bg-indigo-600 !border-indigo-200",
                    "",
                    "!text-indigo-600"
                )
                .SetImageClasses(string.Empty, "h-auto mx-auto")
                .SetDividerClasses("my-4 flex items-center before:mt-0.5 before:flex-1 before:border-t before:border-neutral-300 after:mt-0.5 after:flex-1 after:border-t after:border-neutral-300 dark:before:border-neutral-500 dark:after:border-neutral-500", "absolute px-3 font-medium text-gray-900 -translate-x-1/2 bg-white left-1/2 dark:text-white dark:bg-gray-800")
                .SetInputTextFieldClasses("mb-4", "w-full block mb-2 text-sm font-medium text-gray-900 dark:text-white", "bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500")
                .SetPasswordFieldClasses("mb-4 w-full", "w-full block mb-2 text-sm font-medium text-gray-900 dark:text-white", "bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500")
                .SetCheckboxClass("flex items-center mb-4", "w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded-sm focus:ring-blue-500 dark:focus:ring-blue-600 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600", "ms-2 text-sm font-medium text-gray-900 dark:text-gray-300")
                .SetBackClass("w-full text-white bg-primary-600 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800")
                .SetButtonClass("w-full text-white bg-primary-600 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800")
                .SetAnchorClass("text-sm font-medium text-primary-600 hover:underline dark:text-primary-500", "w-full text-white bg-primary-600 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800")
                .Build();
        }
    }
}