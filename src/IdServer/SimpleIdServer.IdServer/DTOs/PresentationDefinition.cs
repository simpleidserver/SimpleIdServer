// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.DTOs
{
    //https://identity.foundation/presentation-exchange/#term:presentation-definition

    public class PresentationDefinition
    {
        /// <summary>
        /// MUST contain an ID property.
        /// The value of this property MUST be a string.
        /// The string SHOIOULD provide a unique ID for the desired context.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Id)]
        public string Id { get; set; }
        /// <summary>
        /// MUST contain an input_descriptors property.
        /// Its value must be an array of Input Descriptor Objects.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.InputDescriptors)]
        public ICollection<InputDescriptor> InputDescriptors { get; set; }
    }

    /// <summary>
    /// Object used to describe the information a Verifier requires of a Holder.
    /// All Input Descriptors MUST be satisfied.
    /// </summary>
    public class InputDescriptor
    {
        /// <summary>
        /// MAY contain a name property.
        /// SHOULD be a human-friendly string intended to constitute a distinctive designation of the Presentation Definition.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Name)]
        public string Name { get; set; }
        /// <summary>
        /// MAY contain a purpose property.
        /// If present, its value MUST be a string that describes the purpose for which the Presentation Definition's inputs are being used for.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Purpose)]
        public string Purpose { get; set; }
        /// <summary>
        /// MAY inbclude a format property.
        /// Value MUST be an object with one or more properties matching the registered Claim Format Desigations.
        /// The properties inform the Holder of the Claim format configurations that Verifier can process.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Format)]
        public Dictionary<string, ClaimFormatDesignation> Format { get; set; }
        /// <summary>
        /// MUST be an object composed of at least one of the following properties : fields or limit_disclosure.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Constraints)]
        public Constraints Constraints { get; set; }
    }

    public class ClaimFormatDesignation
    {
        [JsonPropertyName(PresentationDefinitionParameters.Alg)]
        public IEnumerable<string> Alg { get; set; }
        [JsonPropertyName(PresentationDefinitionParameters.ProofType)]
        public IEnumerable<string> ProofType { get; set; }
    }

    public class Constraints
    {
        /// <summary>
        /// SHALL be processed forward from 0-index.
        /// MUST be an array of objects.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Fields)]
        public IEnumerable<ConstraintsFields> Fields { get; set; }
        /// <summary>
        /// MAY contain a limit_disclosure property.
        /// MUST be required or preferred.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.LimitDisclosure)]
        public string LimitDisclosure { get; set; }
    }

    public class ConstraintsFields
    {
        /// <summary>
        /// MUST contain a path property.
        /// Array of one or more JSONPath string expressions that select a target value from the input.
        /// Array must be evaluated from 0-index forward.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Path)]
        public IEnumerable<string> Path { get; set; }
        /// <summary>
        /// MAY contain an id property.
        /// MUST be a string that is unique from every other field object's id property.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Id)]
        public string Id { get; set; }
        /// <summary>
        /// MAY contain a purpose property.
        /// MUST be a string that describes the purpose for which the field is being requested.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Purpose)]
        public string Purpose { get; set; }
        /// <summary>
        /// MAY contain a name property.
        /// MUST be a string, and SHOULD be a human-friendly name that describes what the target field represents.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Name)]
        public string Name { get; set; }
        /// <summary>
        /// MAY contain a filter property.
        /// UST be a JSON Schema descriptor used to filter against the values returned from evaluation of the JSONPath string expressions in the path array.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Filter)]
        public JsonObject Filter { get; set; }
        /// <summary>
        /// MAY contain an optional property.
        /// MUST be a boolean, wherein true indicates the field is optional, and false or non-presence of the property indicates the field is required.
        ///  Even when the optional property is present, the value located at the indicated path of the field MUST validate against the JSON Schema filter, if a filter is present.
        /// </summary>
        [JsonPropertyName(PresentationDefinitionParameters.Optional)]
        public bool Optional { get; set; }
    }
}
