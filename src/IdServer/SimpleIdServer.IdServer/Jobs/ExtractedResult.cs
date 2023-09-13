// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Jobs;


public class ExtractedResult
{
    public List<ExtractedUserResult> Users { get; set; } = new List<ExtractedUserResult>();
    public int CurrentPage { get; set; }
}

public class ExtractedUserResult
{
    public string Id { get; set; }
    public string Version { get; set; }
    public List<string> Values { get; set; }
}