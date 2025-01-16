// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.UIs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.Resources;

namespace SimpleIdServer.IdServer.Pwd.UI.ViewModels
{
    public class ConfirmResetPasswordViewModel : IStepViewModel
    {
        public string? Destination { get; set; }
        public string? Code { get; set; }
        public string? Password { get; set; } = null;
        public string? ConfirmationPassword { get; set; } = null;
        public bool IsPasswordUpdated { get; set; }
        public string? ReturnUrl { get; set; } = null;
        public string StepId { get; set; }
        public string WorkflowId { get; set; }
        public string CurrentLink { get; set; }
        public string Realm { get; set; }

        public List<string> Validate(ModelStateDictionary modelState)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(Code)) result.Add(Global.MissingConfirmationCode);
            if (string.IsNullOrWhiteSpace(Destination)) result.Add(Global.MissingLogin);
            if (string.IsNullOrWhiteSpace(Password)) result.Add(Global.MissingPassword);
            if (string.IsNullOrWhiteSpace(ConfirmationPassword)) result.Add(Global.MissingConfirmedPassword);
            if (Password != ConfirmationPassword) result.Add(Global.PasswordMismatch);
            return result;
        }
    }
}
