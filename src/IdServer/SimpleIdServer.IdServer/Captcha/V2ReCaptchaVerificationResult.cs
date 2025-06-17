// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Captcha;

public class V2ReCaptchaVerificationResult
{
    public bool Success { get; set; }

    [JsonPropertyName("challenge_ts")]
    public DateTimeOffset ChallengeTimestamp 
    { 
        get; set; 
    }

    public string Hostname 
    { 
        get; set; 
    }

    [JsonPropertyName("error-codes")]
    public string[] ErrorCodes 
    { 
        get; set; 
    } = new string[0];
}
