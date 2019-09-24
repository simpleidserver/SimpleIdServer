// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Client.Selectors
{
    internal sealed class TokenRequestBuilder
    {
        private ICollection<KeyValuePair<string, string>> _header;
        private ICollection<KeyValuePair<string, string>> _body;
        private Func<Task<OAuthHttpResult>> _callback;

        public TokenRequestBuilder(Func<Task<OAuthHttpResult>> callback)
        {
            _header = new List<KeyValuePair<string, string>>();
            _body = new List<KeyValuePair<string, string>>();
            _callback = callback;
        }

        public TokenRequestBuilder AddHeader(string key, string value)
        {
            _header.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public TokenRequestBuilder AddBody(string key, string value)
        {
            _body.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public void RemoveFromBody(params string[] keys)
        {
            _body =_body.Except(_body.Where(b => keys.Contains(b.Key))).ToList();
        }

        public ICollection<KeyValuePair<string, string>> Header => _header;
        public ICollection<KeyValuePair<string, string>> Body => _body;
        public Func<Task<OAuthHttpResult>> GetDiscovery => _callback;

        private static void Add(ICollection<KeyValuePair<string, string>> dic, string key, string value)
        {
            dic.Add(new KeyValuePair<string, string>(key, value));
        }
    }
}
