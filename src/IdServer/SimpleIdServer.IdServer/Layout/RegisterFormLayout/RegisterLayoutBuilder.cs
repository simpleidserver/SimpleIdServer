// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Layout.RegisterFormLayout;

public class RegisterLayoutBuilder
{
    protected RegisterLayoutBuilder(FormRecord formRecord)
    {
        FormRecord = formRecord;
    }

    protected FormRecord FormRecord { get; private set; }

    public static RegisterLayoutBuilder New(string id, string correlationId, string name)
    {
        var record = new FormRecord
        {
            Id = id,
            CorrelationId = correlationId,
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

    public RegisterLayoutBuilder ConfigureBackButton(string id)
    {
        var elt = new FormStackLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            IsFormEnabled = false,
            Transformations = BuildConditionUseToDisplayBackForm(),
            Elements = new ObservableCollection<IFormElementRecord>
            {                
                // Back button - ReturnUrl.
                StandardFormComponents.NewBackToReturnUrl(id)
            }
        };
        AddElement(elt);
        return this;
    }

    private static List<ITransformationRule> BuildConditionUseToDisplayBackForm()
    {
        return new List<ITransformationRule>
        {
            new PropertyTransformationRule
            {
                PropertyName = nameof(FormStackLayoutRecord.IsNotVisible),
                PropertyValue = "true",
                Condition = new LogicalParameter
                {
                    LeftExpression = new LogicalParameter
                    {
                        LeftExpression = new ComparisonParameter
                        {
                            Source = $"$.{nameof(IRegisterViewModel.IsCreated)}",
                            Operator = ComparisonOperators.EQ,
                            Value = "false"
                        },
                        RightExpression = new ComparisonParameter
                        {
                            Source = $"$.{nameof(IRegisterViewModel.IsUpdated)}",
                            Operator = ComparisonOperators.EQ,
                            Value = "false"
                        },
                        Operator = LogicalOperators.AND
                    },
                    RightExpression = new NotPresentParameter
                    {
                        Source = $"$.{nameof(IRegisterViewModel.ReturnUrl)}"
                    },
                    Operator = LogicalOperators.OR
                }
            },
            new PropertyTransformationRule
            {
                PropertyName = nameof(FormStackLayoutRecord.IsNotVisible),
                PropertyValue = "false",
                Condition = new LogicalParameter
                {
                    LeftExpression = new LogicalParameter
                    {
                        LeftExpression = new ComparisonParameter
                        {
                            Source = $"$.{nameof(IRegisterViewModel.IsCreated)}",
                            Operator = ComparisonOperators.EQ,
                            Value = "true"
                        },
                        RightExpression = new ComparisonParameter
                        {
                            Source = $"$.{nameof(IRegisterViewModel.IsUpdated)}",
                            Operator = ComparisonOperators.EQ,
                            Value = "true"
                        },
                        Operator = LogicalOperators.OR
                    },
                    RightExpression = new PresentParameter
                    {
                        Source = $"$.{nameof(IRegisterViewModel.ReturnUrl)}"
                    },
                    Operator = LogicalOperators.AND
                }
            }
        };
    }

    public FormRecord Build(DateTime currentDateTime)
    {
        FormRecord.Publish(currentDateTime);
        return FormRecord;
    }
}
