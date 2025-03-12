// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public class AuthenticateConsoleViewModel : BaseOTPAuthenticateViewModel
{
    public override List<string> SpecificValidate() => new List<string>();
}
