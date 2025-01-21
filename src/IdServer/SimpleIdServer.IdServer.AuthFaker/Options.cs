// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.AuthFaker;

using CommandLine;

public class Options
{

    [Option('u', "url", Required = false, HelpText = "Base url of SimpleIdServer", Default = "https://localhost:5001")]
    public string Url { get; set; }
    [Option('i', "id", Required = false, HelpText = "Browser identifier", Default = "ba7a5c69-db62-425f-b0c4-970da83d46db")]
    public string BrowserId { get; set; }
    [Option('p', "port", Required = false, HelpText = "Port")]
    public int Port { get; set; } = 9223;
}