﻿using System.Text.Json.Serialization;

namespace FormBuilder.Models;

public class WorkflowLinkSource
{
    public string EltId { get; set; }
    [JsonIgnore]
    public Size Size { get; set; }
    [JsonIgnore]
    public Coordinate CoordinateRelativeToStep { get; set; }

    public WorkflowLinkSource Clone()
    {
        return new WorkflowLinkSource
        {
            EltId = EltId,
            Size = Size.Clone(),
            CoordinateRelativeToStep = CoordinateRelativeToStep.Clone()
        };
    }
}