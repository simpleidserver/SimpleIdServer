// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.Vp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

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

    public VerifiablePresentation BuildAndVerify(VerifiablePresentationDefinition vpDef, List<VcRecord> vcRecords)
    {
        if(vpDef.InputDescriptors != null)
        {
            foreach(var inputDescriptor in vpDef.InputDescriptors)
            {
                var vc = GetVc(inputDescriptor, vcRecords);
                if (vc == null) return; // THROW ERROR.
                // CHECK THE FORMAT
                if (!IsFormatValid(inputDescriptor, vc)) return; // THROW ERROR.
            }
        }

        return _verifiablePresentation;
    }

    private bool IsFormatValid(InputDescriptor inputDescriptor, VcRecord vcRecord)
    {
        // check the format
        if (inputDescriptor.Format != null && inputDescriptor.Format.Any())
        {
            foreach (var kvp in inputDescriptor.Format)
            {
                
            }
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
                    var elt = vcRecord.JsonObj.SelectToken(path);
                    if (elt == null) return false;
                    if (field.Filter != null)
                    {
                        var schema = JsonSchema.Parse(field.Filter.ToString());
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
    public JObject JsonObj { get; set; }
}