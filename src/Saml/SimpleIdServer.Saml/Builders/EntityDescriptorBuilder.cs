// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Saml.Builders
{
    public class EntityDescriptorBuilder
    {
        private static EntityDescriptorBuilder _instance;
        private EntityDescriptorType _entityDescriptor;

        private EntityDescriptorBuilder(string entityID) 
        {
            _entityDescriptor = new EntityDescriptorType
            {
                 entityID = entityID,
                 ID = $"pfx_{Guid.NewGuid()}"
            };
        }

        /// <summary>
        /// Specifies metadata for a single SAML entity.
        /// Concrete roles : SOO Identity Provider, SSO Service Provider, Authentication Authority, Attribute Authority, Policy Decision Point, Affiliation.
        /// </summary>
        /// <param name="entityID">The unique identifier of the SAML entity which is described by this definition.</param>
        /// <returns></returns>
        public static EntityDescriptorBuilder Instance(string entityID)
        {
            if (_instance == null)
            {
                _instance = new EntityDescriptorBuilder(entityID);
            }

            return _instance;
        }

        #region Actions

        /// <summary>
        /// The expiration time of the metadata.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public EntityDescriptorBuilder SetValidUntil(DateTime dateTime)
        {
            _entityDescriptor.validUntilSpecified = true;
            _entityDescriptor.validUntil = dateTime;
            return this;
        }

        /// <summary>
        /// Maximum length of time a consumer should cache the metadata.
        /// </summary>
        /// <param name="cacheDuration"></param>
        /// <returns></returns>
        public EntityDescriptorBuilder SetCacheDuration(string cacheDuration)
        {
            _entityDescriptor.cacheDuration = cacheDuration;
            return this;
        }

        /// <summary>
        /// Used to describe identity provider (Idp) supporting SSO.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public EntityDescriptorBuilder AddIdpSSODescriptor(Action<IDPSSODescriptorBuilder> callback)
        {
            var items = RemoveDescriptor<IDPSSODescriptorType>();
            var ssoDescriptor = new IDPSSODescriptorType();
            var builder = new IDPSSODescriptorBuilder(ssoDescriptor);
            callback(builder);
            items.Add(ssoDescriptor);
            _entityDescriptor.Items = items.ToArray();
            return this;
        }

        /// <summary>
        /// Used to describe service provider (SP) supporting SSO.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public EntityDescriptorBuilder AddSpSSODescriptor(Action<SPSSODescriptorBuilder> callback)
        {
            var items = RemoveDescriptor<SPSSODescriptorType>();
            var ssoDescriptor = new SPSSODescriptorType();
            var builder = new SPSSODescriptorBuilder(ssoDescriptor);
            callback(builder);
            items.Add(ssoDescriptor);
            _entityDescriptor.Items = items.ToArray();
            return this;
        }

        /// <summary>
        /// Return the EntityDescriptor.
        /// </summary>
        /// <returns></returns>
        public EntityDescriptorType Build()
        {
            return _entityDescriptor;
        }

        #endregion

        private List<object> RemoveDescriptor<T>()
        {
            var items = new List<object>();
            if (_entityDescriptor.Items != null)
            {
                items = _entityDescriptor.Items.ToList();
            }

            var item = items.FirstOrDefault(i => i.GetType() == typeof(T));
            if (item != null)
            {
                items.Remove(item);
            }

            return items;
        }
    }
}
