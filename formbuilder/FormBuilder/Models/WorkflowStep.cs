﻿namespace FormBuilder.Models;

public class WorkflowStep : ICloneable
{
    public string Id { get; set; }
    public string FormRecordId { get; set; }
    public bool IsEmptyStep
    {
        get
        {
            return FormRecordId == Constants.EmptyStep.CorrelationId;
        }
    }

    public object Clone()
    {
        return new WorkflowStep
        {
            Id = Id,
            FormRecordId = FormRecordId
        };
    }
}