// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace SimpleIdServer.Configuration;

[AttributeUsage(AttributeTargets.Property)]
public class ConfigurationRecordAttribute : Attribute
{
	public ConfigurationRecordAttribute(string displayName, string description = null, int order = 0)
	{
		DisplayName = displayName;
		Description = description;
		Order = order;
	}

	public ConfigurationRecordAttribute(string displayName, string description = null, int order = 0, string displayCondition = null) : this(displayName, description, order)
	{
        DisplayCondition = displayCondition;
    }

    public ConfigurationRecordAttribute(string displayName, string description, int order, string displayCondition, CustomConfigurationRecordType customType) : this(displayName, description, order, displayCondition)
    {
        CustomType = customType;
    }

    public string DisplayName { get; set; } = null!;
	public string? Description { get; set; } = null;
	public int Order { get; set; } = 0;
	public string DisplayCondition { get; set; }
	public CustomConfigurationRecordType? CustomType { get; set; } = null;

    public Dictionary<string, string>? Values { get; set; } = null;
}

public enum CustomConfigurationRecordType
{
	OTPVALUE = 0,
	PASSWORD = 1,
	NOTIFICATIONMODE = 2
}
