﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Helpers.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface ITranslationRepository
{
    void Add(Translation translation);
    Task<List<Translation>> GetAllByKey(string key, CancellationToken cancellationToken);
}
