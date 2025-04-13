// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder.Tailwindcss;
using Microsoft.Extensions.DependencyInjection;

namespace FormBuilder;

public static class FormBuilderRegistrationExtensions
{
    public static FormBuilderRegistration AddTailwindcss(this FormBuilderRegistration formBuilder)
    {
        formBuilder.Services.AddTransient<IDataSeeder, ConfigureTailwindcssTemplateDataSeeder>();
        return formBuilder;
    }
}
