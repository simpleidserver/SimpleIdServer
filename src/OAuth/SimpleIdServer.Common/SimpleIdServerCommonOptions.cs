// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Common
{
    public class SimpleIdServerCommonOptions
    {
        public SimpleIdServerCommonOptions()
        {
            SupportedUICultures = new List<UICultureOption>
            {
                new UICultureOption("fr", "French"),
                new UICultureOption("en", "English")
            };
            DefaultCulture = "en";
        }

        /// <summary>
        /// Supported cultures.
        /// </summary>
        public IEnumerable<UICultureOption> SupportedUICultures { get; set; }
        /// <summary>
        /// Set the default UI culture.
        /// </summary>
        public string DefaultCulture { get; set; }
    }
}
