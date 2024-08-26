// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.Vp.Models;
using SimpleIdServer.Vp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Vp;

public class VpBuilder
{
    private readonly VerifiablePresentation _verifiablePresentation;

    private VpBuilder(VerifiablePresentation verifiablePresentation)
    {
        _verifiablePresentation = verifiablePresentation;    
    }

    public static VpBuilder New(string id, string holder, string type = null, string jsonLdContext = null)
    {
        var record = new VerifiablePresentation
        {
            Id = id,
            Holder = holder
        };
        record.Context.Add(VpConstants.VerifiablePresentationContext);
        if (!string.IsNullOrWhiteSpace(jsonLdContext)) record.Context.Add(jsonLdContext);
        record.Type.Add(VpConstants.VerifiablePresentationType);
        if (!string.IsNullOrWhiteSpace(type)) record.Type.Add(type);
        return new VpBuilder(record);
    }

    public VpBuilderResult BuildAndVerify(VerifiablePresentationDefinition vpDef, List<VcRecord> vcRecords, string vpFormat = "jwt_vp")
    {
        var descriptorMaps = new List<DescriptorMap>();
        if (vpDef.InputDescriptors != null)
        {
            int i = 0;
            foreach(var inputDescriptor in vpDef.InputDescriptors)
            {
                var vc = GetVc(inputDescriptor, vcRecords);
                if (vc == null) return VpBuilderResult.Nok(Global.VcCannotBeResolved);
                if (!IsFormatValid(inputDescriptor, vc)) return VpBuilderResult.Nok(Global.FormatNotSatisfied);
                descriptorMaps.Add(new DescriptorMap
                {
                    Id = inputDescriptor.Id,
                    Path = "$",
                    Format = vpFormat,
                    PathNested = new PathNested
                    {
                        Id = inputDescriptor.Id,
                        Format = vc.Format,
                        Path = $"$.vp.verifiableCredential[{i}]"
                    }
                });
                i++;
            }
        }

        foreach (var vc in vcRecords)
            _verifiablePresentation.VerifiableCredential.Add(vc.Vc.ToString());
        return VpBuilderResult.Ok(_verifiablePresentation, new PresentationSubmission
        {
            Id = Guid.NewGuid().ToString(),
            DefinitionId = vpDef.Id,
            DescriptorMap = descriptorMaps
        });
    }

    private bool IsFormatValid(InputDescriptor inputDescriptor, VcRecord vcRecord)
    {
        if (inputDescriptor.Format != null && inputDescriptor.Format.Any())
        {
            foreach (var kvp in inputDescriptor.Format)
            {
                if (kvp.Key != vcRecord.Format) break;
                if (kvp.Value.Alg == null) return true;
                var vcRecordAlg = vcRecord.JsonHeader.SelectToken("$.alg")?.ToString();
                if (vcRecordAlg != null && kvp.Value.Alg.Contains(vcRecordAlg)) return true;
            }

            return false;
        }

        return true;
    }

    private VcRecord GetVc(InputDescriptor inputDescriptor, List<VcRecord> vcRecords)
    {
        if (inputDescriptor.Constraints == null || inputDescriptor.Constraints.Fields == null  || !inputDescriptor.Constraints.Fields.Any()) return null;
        foreach (var vcRecord in vcRecords)
        {
            var isCorrect = inputDescriptor.Constraints.Fields.All(field =>
            {
                foreach (var path in field.Path)
                {
                    var elt = vcRecord.JsonPayload.SelectToken(path);
                    if (elt == null) return false;
                    if (field.Filter != null)
                    {
                        var schema = JSchema.Parse(field.Filter.ToString());
                        if (!elt.IsValid(schema)) return false;
                    }
                }

                return true;
            });

            if (isCorrect) return vcRecord;
        }

        return null;
    }
}

public class VcRecord
{
    public object Vc { get; set; }
    public W3CVerifiableCredential DeserializedVc { get; set; }
    public string Format { get; set; }
    public JObject JsonHeader { get; set; }
    public JObject JsonPayload { get; set; }
}