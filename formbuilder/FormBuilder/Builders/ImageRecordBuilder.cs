// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.Image;
using FormBuilder.Models.Rules;

namespace FormBuilder.Builders;

public class ImageRecordBuilder
{
    private readonly ImageRecord _record;

    internal ImageRecordBuilder(string url, string id, string correlationId)
    {
        _record = new ImageRecord
        {
            Id = id ?? Guid.NewGuid().ToString(),
            CorrelationId = correlationId ?? Guid.NewGuid().ToString(),
            Url = url
        };
    }

    public ImageRecordBuilder SetTransformations(List<ITransformationRule> transformations)
    {
        _record.Transformations = transformations;
        return this;
    }

    internal ImageRecord Build()
    {
        return _record;
    }
}
