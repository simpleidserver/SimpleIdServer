// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.VerifiablePresentation.UI.ViewModels;

public class VpAuthenticateViewModel : BaseAuthenticateViewModel
{
    public override void CheckRequiredFields(ModelStateDictionary modelStateDictionary)
    {
    }

    public ICollection<PresentationDefinitionViewModel> PresentationDefinitions { get; set; } = new List<PresentationDefinitionViewModel>();
}

public class PresentationDefinitionViewModel
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string Purpose { get; set; }
}