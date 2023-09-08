// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Store;

namespace SimpleIdServer.Configuration.Apis;

public class ControllerDefinitionsController : Controller
{
	private readonly IConfigurationDefinitionStore _store;

	public ControllerDefinitionsController(IConfigurationDefinitionStore store)
	{
		_store = store;
	}
}