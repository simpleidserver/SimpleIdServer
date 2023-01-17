// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace SimpleIdServer.IdServer.Api
{
    public class HandlerContextResponse
    {
        public HandlerContextResponse()
        {
            Parameters = new Dictionary<string, string>();
        }

        public HandlerContextResponse(IResponseCookies cookies) : this()
        {
            Cookies = cookies;
        }

        public IResponseCookies Cookies { get; set; }
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
        public HandlerContextRequest(string issuerName, string userSubject)
        {
            IssuerName = issuerName;
            UserSubject = userSubject;
        }

        public HandlerContextRequest(string issuerName, string userSubject, JsonObject data) : this(issuerName, userSubject)
        {
            var allKeys = data.Select(r => r.Key).ToList();
            foreach (var key in allKeys)
            {
                var val = data[key] as JsonValue;
                if(val != null)
                {
                    if (val.TryGetValue<string>(out string v))
                        data[key] = HttpUtility.UrlDecode(v);
                }
            }

            OriginalRequestData = data;
            RequestData = data;
        }

        public HandlerContextRequest(string issuerName, string userSubject, JsonObject data, JsonObject httpHeader) : this(issuerName, userSubject, data)
        {
            HttpHeader = httpHeader;
        }

        public HandlerContextRequest(string issuerName, string userSubject, JsonObject data, JsonObject httpHeader, IRequestCookieCollection cookies): this(issuerName, userSubject, data, httpHeader)
        {
            Cookies = cookies;
        }

        public HandlerContextRequest(string issuerName, string userSubject, JsonObject data, JsonObject httpHeader, IRequestCookieCollection cookies, X509Certificate2 certificate) : this(issuerName, userSubject, data, httpHeader, cookies)
        {
            Certificate = certificate;
        }

        public string IssuerName { get; private set; }
        public string UserSubject { get; private set; }
        public JsonObject OriginalRequestData { get; private set; }
        public JsonObject RequestData { get; private set; }
        public JsonObject HttpHeader { get; private set; }
        public IRequestCookieCollection Cookies { get; set; }
        public X509Certificate2 Certificate { get; set; }

        public void SetRequestData(JsonObject data)
        {
            RequestData = data;
        }

        public string GetToken(params string[] prefixes)
        {
            if (!HttpHeader.ContainsKey("Authorization"))
            {
                return null;
            }

            var authorizationValue = HttpHeader["Authorization"];
            var values = new List<string>();
            if (authorizationValue is JsonArray)
            {
                var jArr = authorizationValue.AsArray();
                foreach(var rec in jArr)
                {
                    values.Add(rec.ToString());
                }
            }
            else
            {
                values.Add(authorizationValue.ToString());
            }

            return values.Select(_ => GetToken(_, prefixes)).FirstOrDefault(_ => _ != null);
        }

        private static string GetToken(string value, params string[] prefixes)
        {
            var splitted = value.Split(' ');
            var prefix = prefixes.FirstOrDefault(_ => value.StartsWith(_));
            if (prefix == null || splitted.Count() != 2)
            {
                return null;
            }

            return splitted[1];
        }
    }

    public class HandlerContext
    {
        public HandlerContext(HandlerContextRequest request)
        {
            Request = request;
            Response = new HandlerContextResponse();
        }

        public HandlerContext(HandlerContextRequest request, HandlerContextResponse response) : this(request)
        {
            Response = response;
        }

        public User User { get; private set; }
        public Client Client { get; private set; }
        public HandlerContextRequest Request { get; private set; }
        public JsonObject OriginalRequest { get; private set; }
        public HandlerContextResponse Response { get; private set; }

        public void SetClient(Client client) => Client = client;

        public void SetUser(User user) => User = user;

        public void SetResponse(HandlerContextResponse response) => Response = response;

        public void SetRequest(HandlerContextRequest request) => Request = request;

        public void SetOriginalRequest(JsonObject request) => OriginalRequest = request;
    }
}
