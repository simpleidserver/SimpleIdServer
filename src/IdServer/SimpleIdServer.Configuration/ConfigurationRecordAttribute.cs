// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace SimpleIdServer.Configuration;

[AttributeUsage(AttributeTargets.Property)]
public class ConfigurationRecordAttribute : Attribute
{
	public ConfigurationRecordAttribute(string displayName, string description = null)
	{
		DisplayName = displayName;
		Description = description;
	}

	public ConfigurationRecordAttribute(string displayName, string description = null, Dictionary<string, string> values = null) : this(displayName, description)
	{
		Values = values;
	}

	public string DisplayName { get; set; } = null!;
	public string? Description { get; set; } = null;
	public Dictionary<string, string>? Values { get; set; } = null;
}
