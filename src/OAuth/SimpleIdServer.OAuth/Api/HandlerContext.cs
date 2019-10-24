// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Api
{
    public class HandlerContextResponse
    {
        public HandlerContextResponse()
        {
            Parameters = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Add parameter.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            Parameters.Add(key, value);
        }

        public bool TryGet(string key, out string value)
        {
            value = null;
            if (Parameters.ContainsKey(key))
            {
                value = Parameters[key];
                return true;
            }

            return false;
        }
    }

    public class HandlerContextRequest
    {
        public HandlerContextRequest(string issuerName, string userSubject, DateTime? authDateTime)
        {
            IssuerName = issuerName;
            UserSubject = userSubject;
            AuthDateTime = authDateTime;
        }

        public HandlerContextRequest(string issuerName, string userSubject, DateTime? authDateTime, JObject data) : this(issuerName, userSubject, authDateTime)
        {
            Data = data;
        }

        public HandlerContextRequest(string issuerName, string userSubject, DateTime? authDateTime, JObject data, JObject httpHeader) : this(issuerName, userSubject, authDateTime)
        {
            Data = data;
            HttpHeader = httpHeader;
        }

        public string IssuerName { get; private set; }
        public string UserSubject { get; private set; }
        public DateTime? AuthDateTime { get; private set; }
        public JObject Data { get; private set; }
        public JObject HttpHeader { get; private set; }

        public void SetData(JObject data)
        {
            Data = data;
        }
    }

    public class HandlerContext
    {
        public HandlerContext(HandlerContextRequest request)
        {
            Request = request;
            Response = new HandlerContextResponse();
        }

        public OAuthUser User { get; private set; }
        public OAuthClient Client { get; private set; }
        public HandlerContextRequest Request { get; private set; }
        public HandlerContextResponse Response { get; private set; }

        public void SetClient(OAuthClient client)
        {
            Client = client;
        }

        public void SetUser(OAuthUser user)
        {
            User = user;
        }

        public void SetResponse(HandlerContextResponse response)
        {
            Response = response;
        }

        public void SetRequest(HandlerContextRequest request)
        {
            Request = request;
        }
    }
}
