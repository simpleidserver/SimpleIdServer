// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using SimpleIdServer.IdServer.Layout.RegisterFormLayout;
using SimpleIdServer.IdServer.Pwd.UI.ViewModels;
using SimpleIdServer.IdServer.Resources;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Pwd;

public static class StandardPwdRegisterForms
{
    public static string pwdRegisterFormId = "e501ca67-2bc9-477e-959c-9752a603fcd1";
    public static string backBtnId = "ecb00170-5b5c-408d-ba67-dad2a81210e3";

    public static FormRecord PwdForm = RegisterLayoutBuilder.New("d45d4e28-3720-4d0f-9ce5-637a342257de", "pwdRegister", Constants.AreaPwd)
        .AddElement(new FormStackLayoutRecord
        {
            Id = pwdRegisterFormId,
            CorrelationId = pwdRegisterFormId,
            IsFormEnabled = true,
            Transformations = StandardRegisterFormComponents.BuildConditionUseToDisplayRegistrationForm(),
            Elements = new ObservableCollection<IFormElementRecord>
            {
                // ReturnUrl.
                StandardFormComponents.NewReturnUrl(),
                // Realm.
                StandardFormComponents.NewRealm(),
                // Login.
                StandardFormComponents.NewLogin(),
                // Password.
                StandardFormComponents.NewPassword(nameof(PwdRegisterViewModel.Password)),
                // Confirmation password.                    
                StandardFormComponents.NewPassword(nameof(PwdRegisterViewModel.ConfirmedPassword)),
                // Register
                StandardRegisterFormComponents.NewRegister()
            }
        })
        .ConfigureBackButton(backBtnId)
        .AddErrorMessage(RegisterFormErrorMessages.NotAllowedToRegister, Global.NotAllowedToRegister)
        .AddErrorMessage(RegisterFormErrorMessages.MissingLogin, Global.MissingLogin)
        .AddErrorMessage(RegisterFormErrorMessages.MissingPassword, Global.MissingPassword)
        .AddErrorMessage(RegisterFormErrorMessages.MissingConfirmedPassword, Global.MissingConfirmedPassword)
        .AddErrorMessage(RegisterFormErrorMessages.PasswordMismatch, Global.PasswordMismatch)
        .AddErrorMessage(RegisterFormErrorMessages.UserWithSameLoginAlreadyExists, Global.UserWithSameLoginAlreadyExists)
        .AddErrorMessage(AuthFormErrorMessages.PasswordTooShort, Global.PasswordTooShort)
        .AddErrorMessage(AuthFormErrorMessages.PasswordRequiresNonAlphanumeric, Global.PasswordRequiresNonAlphanumeric)
        .AddErrorMessage(AuthFormErrorMessages.PasswordRequiresDigit, Global.PasswordRequiresDigit)
        .AddErrorMessage(AuthFormErrorMessages.PasswordRequiresLower, Global.PasswordRequiresLower)
        .AddErrorMessage(AuthFormErrorMessages.PasswordRequiresUpper, Global.PasswordRequiresUpper)
        .AddErrorMessage(AuthFormErrorMessages.RequiredUniqueChars, Global.RequiredUniqueChars)
        .AddSuccessMessage(RegisterFormSuccessMessages.UserIsUpdated, Global.UserIsUpdated)
        .AddSuccessMessage(RegisterFormSuccessMessages.UserIsCreated, Global.UserIsCreated)
        .Build(DateTime.UtcNow);
}
