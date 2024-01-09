// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Jobs;


public class ExtractedResult
{
    public List<ExtractedUser> Users { get; set; } = new List<ExtractedUser>();
    public List<ExtractedGroup> Groups { get; set; } = new List<ExtractedGroup>();
    public int CurrentPage { get; set; }
}

public class ExtractedUser
{
    public string Id { get; set; }
    public string Version { get; set; }
    public List<string> Values { get; set; }
}

public class ExtractedGroup
{
    public string Id { get; set; }
    public string Version { get; set; }
    public List<string> Values { get; set; }
    public string UserId { get; set; }
}