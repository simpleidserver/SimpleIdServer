// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public enum SingleSignOnActions
    {
        AUTHENTICATE = 0,
        REDIRECT = 1,
        HTML = 2
    }

    public class SingleSignOnResult
    {
        public SingleSignOnActions Action { get; private set; }
        public string Amr { get; private set; }
        public string Location { get; private set; }
        public string Content { get; private set; }

        public static SingleSignOnResult Authenticate(string amr)
        {
            return new SingleSignOnResult
            {
                Action = SingleSignOnActions.AUTHENTICATE,
                Amr = amr
            };
        }

        public static SingleSignOnResult Redirect(string location)
        {
            return new SingleSignOnResult
            {
                Action = SingleSignOnActions.REDIRECT,
                Location = location
            };
        }

        public static SingleSignOnResult Html(string html)
        {
            return new SingleSignOnResult
            {
                Action = SingleSignOnActions.HTML,
                Content = html
            };
        }
    }
}
