// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Conditions;
using FormBuilder.Models;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Layout;

public static class LayoutTranslations
{
    public static List<LabelTranslation> Login = LabelTranslationBuilder.New().AddTranslation("en", "Login").AddTranslation("fr", "Login").Build();
    public static List<LabelTranslation> Email = LabelTranslationBuilder.New().AddTranslation("en", "Email").AddTranslation("fr", "Email").Build();
    public static List<LabelTranslation> Password = LabelTranslationBuilder.New().AddTranslation("en", "Password").AddTranslation("fr", "Mot de passe").Build();
    public static List<LabelTranslation> RepeatPassword = LabelTranslationBuilder.New().AddTranslation("en", "Repeat the password").AddTranslation("fr", "Répéter le mot de passe").Build();
    public static List<LabelTranslation> RememberMe = LabelTranslationBuilder.New().AddTranslation("en", "Remember me").AddTranslation("fr", "Se souvenir de moi").Build();
    public static List<LabelTranslation> Authenticate = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").AddTranslation("fr", "Authentifier").Build();
    public static List<LabelTranslation> Separator = LabelTranslationBuilder.New().AddTranslation("en", "OR").AddTranslation("fr", "OU").Build();
    public static List<LabelTranslation> ForgetPassword = LabelTranslationBuilder.New().AddTranslation("en", "Forget my password").AddTranslation("fr", "Avez-vous oublié votre mot de passe ?").Build();
    public static List<LabelTranslation> Register = LabelTranslationBuilder.New().AddTranslation("en", "Register").AddTranslation("en", "Update", new UserAuthenticatedParameter()).AddTranslation("fr", "Enregister").AddTranslation("en", "Mettre à jour", new UserAuthenticatedParameter()).Build();
    public static List<LabelTranslation> ResetPassword = LabelTranslationBuilder.New().AddTranslation("en", "Reset your password").AddTranslation("fr", "Réinitialiser le mot de passe").Build();
    public static List<LabelTranslation> SendLink = LabelTranslationBuilder.New().AddTranslation("en", "Send link").AddTranslation("fr", "Envoyer le lien").Build();
    public static List<LabelTranslation> Back = LabelTranslationBuilder.New().AddTranslation("en", "Back").AddTranslation("fr", "Précédent").Build();
    public static List<LabelTranslation> Code = LabelTranslationBuilder.New().AddTranslation("en", "Code").AddTranslation("fr", "Code").Build();
    public static List<LabelTranslation> Update = LabelTranslationBuilder.New().AddTranslation("en", "Update").AddTranslation("fr", "Mettre à jour").Build();
    public static List<LabelTranslation> PhoneNumber = LabelTranslationBuilder.New().AddTranslation("en", "Phone number").AddTranslation("fr", "Numéro de téléphone").Build();
    public static List<LabelTranslation> SendConfirmationCode = LabelTranslationBuilder.New().AddTranslation("en", "Send confirmation code").AddTranslation("fr", "Envoyer le code de confirmation").Build();
    public static List<LabelTranslation> ConfirmationCode = LabelTranslationBuilder.New().AddTranslation("en", "Confirmation code").AddTranslation("fr", "Code de confirmation").Build();
    public static List<LabelTranslation> DisplayName = LabelTranslationBuilder.New().AddTranslation("en", "Display name").AddTranslation("fr", "Nom").Build();
    public static List<LabelTranslation> GenerateQrCode = LabelTranslationBuilder.New().AddTranslation("en", "Generate the qr code").AddTranslation("fr", "Générer le code qr").Build();
    public static List<LabelTranslation> ScanQrCode = LabelTranslationBuilder.New().AddTranslation("en", "Scan the QR code with your mobile application").AddTranslation("fr", "Scanner le code qr avec votre application mobile").Build();
}
