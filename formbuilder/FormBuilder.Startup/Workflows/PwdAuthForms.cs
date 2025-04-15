// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;

namespace FormBuilder.Startup.Workflows;

public class PwdAuthForms
{
    public static string authPwdFormId = "pwdAuthId";
    public static string authPwdFormCorrelationId = "pwdAuthCorrelationId";
    public static string forgetMyPasswordId = "forgetMyPwdId";
    public static string forgetMyPasswordCorrelationId = "forgetMyPwdCorrelationId";

    public static FormRecord LoginPwdAuthForm =
        FormRecordBuilder.New("pwd", "pwd", Constants.DefaultRealm, true)
        .AddImage("https://upload.wikimedia.org/wikipedia/commons/a/a2/OpenID_logo_2.svg")
        .AddStackLayout(authPwdFormId, authPwdFormCorrelationId, (c) =>
        {
            c.EnableForm()
                .AddInputHiddenField("ReturnUrl", new List<ITransformationRule>
                {
                    new IncomingTokensTransformationRule
                    {
                        Source = "$.ReturnUrl"
                    }
                })
                .AddInputTextField("Login", i =>
                {
                    i.AddLabel("fr", "Login").SetTransformations(new List<ITransformationRule>
                    {
                        new IncomingTokensTransformationRule
                        {
                            Source = "$.Login"
                        },
                        new PropertyTransformationRule
                        {
                            Condition = new PresentParameter
                            {
                                Source = "$.Login"
                            },
                            PropertyName = "Disabled",
                            PropertyValue = "true"
                        }
                    });
                })
                .AddPasswordField("Password", p =>
                {
                    p.AddLabel("fr", "Password");
                })
                .AddCheckbox("RememberLogin", a => a.AddTranslation("fr", "Remember me"))
                .AddButton("Authenticate", c => c.AddTranslation("fr", "Authenticate"));
        })
        .AddDivider(c => c.AddTranslation("fr", "OR"))
        .AddAnchor(forgetMyPasswordId, forgetMyPasswordCorrelationId, c => c.AddTranslation("fr", "Forget my password"))
        .AddDivider(c => c.AddTranslation("fr", "OR"))
        .AddAnchor(cb: c => c.AddTranslation("fr", "Register"))
        .AddDivider(c => c.AddTranslation("fr", "OR"))
        .Build();
}