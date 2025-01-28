﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SimpleIdServer.IdServer.VerifiablePresentation.Resources {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Global {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Global() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SimpleIdServer.IdServer.VerifiablePresentation.Resources.Global", typeof(Global).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the verifiable credential cannot be extracted from the path {0}.
        /// </summary>
        public static string CannotExtractVcFromPath {
            get {
                return ResourceManager.GetString("CannotExtractVcFromPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Forms.
        /// </summary>
        public static string Forms {
            get {
                return ResourceManager.GetString("Forms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the incoming request is not valid.
        /// </summary>
        public static string InvalidIncomingRequest {
            get {
                return ResourceManager.GetString("InvalidIncomingRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the presentation submission is not valid.
        /// </summary>
        public static string InvalidPresentationSubmission {
            get {
                return ResourceManager.GetString("InvalidPresentationSubmission", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the verifiable credential is not valid.
        /// </summary>
        public static string InvalidVerifiableCredential {
            get {
                return ResourceManager.GetString("InvalidVerifiableCredential", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the vp_token is an invalid verifiable presentation.
        /// </summary>
        public static string InvalidVerifiablePresentation {
            get {
                return ResourceManager.GetString("InvalidVerifiablePresentation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the parameter {0} is missing.
        /// </summary>
        public static string MissingParameter {
            get {
                return ResourceManager.GetString("MissingParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the path_nested parameter is missing.
        /// </summary>
        public static string PresentationSubmissinMissingPathNested {
            get {
                return ResourceManager.GetString("PresentationSubmissinMissingPathNested", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the format {0} present in the presentation submission is not correct.
        /// </summary>
        public static string PresentationSubmissionBadFormat {
            get {
                return ResourceManager.GetString("PresentationSubmissionBadFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the verifiable credential {0} is missing from the presentation submission.
        /// </summary>
        public static string PresentationSubmissionMissingVerifiableCredential {
            get {
                return ResourceManager.GetString("PresentationSubmissionMissingVerifiableCredential", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à either the state is invalid or the vp offer is expired.
        /// </summary>
        public static string StateIsNotValid {
            get {
                return ResourceManager.GetString("StateIsNotValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the presentation definition {0} doesn&apos;t exist.
        /// </summary>
        public static string UnknownPresentationDefinition {
            get {
                return ResourceManager.GetString("UnknownPresentationDefinition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the issuer of the verifiable credential must be a Did.
        /// </summary>
        public static string VcIssuerNotDid {
            get {
                return ResourceManager.GetString("VcIssuerNotDid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the proof of the verifiable credential {0} is invalid.
        /// </summary>
        public static string VerifiableCredentialProofInvalid {
            get {
                return ResourceManager.GetString("VerifiableCredentialProofInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the verifiable presentation proof is incorrect.
        /// </summary>
        public static string VerifiablePresentationProofInvalid {
            get {
                return ResourceManager.GetString("VerifiablePresentationProofInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à the verifiable presentation is not received.
        /// </summary>
        public static string VpNotReceived {
            get {
                return ResourceManager.GetString("VpNotReceived", resourceCulture);
            }
        }
    }
}
