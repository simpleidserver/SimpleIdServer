// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Notification.Fcm;

public class FcmOptions
{
    [ConfigurationRecord("Service account", "Path of the service account file", order: 0)]
    public string ServiceAccountFilePath { get; set; }
}