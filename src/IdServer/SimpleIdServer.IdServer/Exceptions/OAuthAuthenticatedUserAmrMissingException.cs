﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Exceptions;

public class OAuthAuthenticatedUserAmrMissingException : OAuthException
{
    public OAuthAuthenticatedUserAmrMissingException(string acr, string amr, IEnumerable<string> allAmrs) : base(string.Empty, string.Empty)
    {
        Acr = acr;
        Amr = amr;
        AllAmrs = allAmrs;
    }

    public string Acr { get; set; }
    public string Amr {  get; set; }
    public IEnumerable<string> AllAmrs { get; set; }
}
