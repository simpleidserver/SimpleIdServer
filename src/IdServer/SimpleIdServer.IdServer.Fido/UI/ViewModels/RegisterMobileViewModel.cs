// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Fido.UI.ViewModels
{
    public class RegisterMobileViewModel : IRegisterViewModel
    {
        public string Login { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string BeginRegisterUrl { get; set; } = null!;
        public string RegisterStatusUrl { get; set; } = null!;
        public bool IsDeveloperModeEnabled { get; set; } = false;
        public bool IsNotAllowed { get; set; }
        public bool IsUpdated { get; set; }
        public string Amr { get; set; }
        public List<string> Steps { get; set; }
    }
}
