// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.ExternalEvents
{
    public class UserLoginFailureEvent : IExternalEvent
    {
        public string EventName => nameof(UserLoginFailureEvent);
        public string Realm { get; set; }
        public string Amr { get; set; }
        public string Login { get; set; }
    }
}