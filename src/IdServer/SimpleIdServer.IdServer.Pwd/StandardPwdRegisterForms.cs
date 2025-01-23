// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.RegisterFormLayout;
using SimpleIdServer.IdServer.Pwd.UI.ViewModels;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Pwd;

public static class StandardPwdRegisterForms
{
    public static string pwdRegisterFormId = "e501ca67-2bc9-477e-959c-9752a603fcd1";
    public static string backBtnId = "ecb00170-5b5c-408d-ba67-dad2a81210e3";

    public static FormRecord PwdForm = RegisterLayoutBuilder.New("pwdRegister", Constants.Areas.Password)
        .AddElement(new FormStackLayoutRecord
        {
            Id = pwdRegisterFormId,
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
        .Build();
}
