// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Layout.RegisterFormLayout;

public class StandardRegisterFormComponents
{
    public static IFormElementRecord NewOtpValue(List<LabelTranslation> translations)
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = nameof(OTPRegisterViewModel.Value),
            Labels = translations,
            Transformations = new List<ITransformationRule>
             {
                 new IncomingTokensTransformationRule
                 {
                     Source = $"$.{nameof(OTPRegisterViewModel.Value)}"
                 },
                 new PropertyTransformationRule
                 {
                     Condition = new LogicalParameter
                     {
                         LeftExpression = new PresentParameter
                         {
                            Source = $"$.{nameof(OTPRegisterViewModel.Value)}"
                         },
                         RightExpression = new UserAuthenticatedParameter(),
                         Operator = LogicalOperators.AND
                     },
                     PropertyName = nameof(FormInputFieldRecord.Disabled),
                     PropertyValue = "true"
                 }
             }
        };
    }

    public static IFormElementRecord NewOtpValueHidden()
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = nameof(OTPRegisterViewModel.Value),
            FormType = FormInputTypes.HIDDEN,
            Transformations = new List<ITransformationRule>
            {
                new IncomingTokensTransformationRule
                {
                    Source = $"$.{nameof(OTPRegisterViewModel.Value)}"
                }
            }
        };
    }

    public static IFormElementRecord NewRegister()
    {
        return new FormButtonRecord
        {
            Id = Guid.NewGuid().ToString(),
            Labels = LayoutTranslations.Register
        };
    }

    public static List<ITransformationRule> BuildConditionUseToDisplayRegistrationForm()
    {
        return new List<ITransformationRule>
        {
            new PropertyTransformationRule
            {
                PropertyName = nameof(FormStackLayoutRecord.IsNotVisible),
                PropertyValue = "true",
                Condition = new LogicalParameter
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
                }
            }
        };
    }
}
