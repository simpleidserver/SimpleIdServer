// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Models;
using System;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Layout.RegisterFormLayout;

public class RegisterLayoutBuilder
{
    protected RegisterLayoutBuilder(FormRecord formRecord)
    {
        FormRecord = formRecord;
    }

    protected FormRecord FormRecord { get; private set; }

    public static RegisterLayoutBuilder New(string name)
    {
        var record = new FormRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            ActAsStep = true,
            Elements = new ObservableCollection<IFormElementRecord>()
        };
        return new RegisterLayoutBuilder(record);
    }

    public RegisterLayoutBuilder AddElement(IFormElementRecord record)
    {
        FormRecord.Elements.Add(record);
        return this;
    }

    public RegisterLayoutBuilder ConfigureBackButton()
    {
        return this;
    }

    public FormRecord Build() => FormRecord;
}
