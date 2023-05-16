﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class RegisterFeature : object, Xunit.IClassFixture<RegisterFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "Register.feature"
#line hidden
        
        public RegisterFeature(RegisterFeature.FixtureData fixtureData, SimpleIdServer_IdServer_Host_Acceptance_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Register", "\tCheck client can be registered", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public void TestInitialize()
        {
        }
        
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Register a complete client")]
        [Xunit.TraitAttribute("FeatureTitle", "Register")]
        [Xunit.TraitAttribute("Description", "Register a complete client")]
        public void RegisterACompleteClient()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Register a complete client", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 4
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table350 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table350.AddRow(new string[] {
                            "client_id",
                            "fiftySevenClient"});
                table350.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table350.AddRow(new string[] {
                            "scope",
                            "register"});
                table350.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
#line 5
 testRunner.When("execute HTTP POST request \'http://localhost/token\'", ((string)(null)), table350, "When ");
#line hidden
#line 12
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 13
 testRunner.And("extract parameter \'access_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table351 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table351.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
                table351.AddRow(new string[] {
                            "redirect_uris",
                            "[https://web.com]"});
                table351.AddRow(new string[] {
                            "response_types",
                            "[token]"});
                table351.AddRow(new string[] {
                            "grant_types",
                            "[implicit]"});
                table351.AddRow(new string[] {
                            "client_name",
                            "name"});
                table351.AddRow(new string[] {
                            "client_name#fr",
                            "nom"});
                table351.AddRow(new string[] {
                            "client_name#en",
                            "name"});
                table351.AddRow(new string[] {
                            "application_type",
                            "web"});
                table351.AddRow(new string[] {
                            "token_endpoint_auth_method",
                            "client_secret_jwt"});
                table351.AddRow(new string[] {
                            "sector_identifier_uri",
                            "https://localhost/sector"});
                table351.AddRow(new string[] {
                            "subject_type",
                            "public"});
                table351.AddRow(new string[] {
                            "id_token_signed_response_alg",
                            "RS256"});
                table351.AddRow(new string[] {
                            "id_token_encrypted_response_alg",
                            "RSA-OAEP"});
                table351.AddRow(new string[] {
                            "id_token_encrypted_response_enc",
                            "A256CBC-HS512"});
                table351.AddRow(new string[] {
                            "userinfo_signed_response_alg",
                            "RS256"});
                table351.AddRow(new string[] {
                            "userinfo_encrypted_response_alg",
                            "RSA-OAEP"});
                table351.AddRow(new string[] {
                            "userinfo_encrypted_response_enc",
                            "A256CBC-HS512"});
                table351.AddRow(new string[] {
                            "request_object_signing_alg",
                            "RS256"});
                table351.AddRow(new string[] {
                            "request_object_encryption_alg",
                            "RSA-OAEP"});
                table351.AddRow(new string[] {
                            "request_object_encryption_enc",
                            "A256CBC-HS512"});
                table351.AddRow(new string[] {
                            "default_max_age",
                            "2"});
                table351.AddRow(new string[] {
                            "require_auth_time",
                            "true"});
                table351.AddRow(new string[] {
                            "default_acr_values",
                            "[a,b]"});
                table351.AddRow(new string[] {
                            "post_logout_redirect_uris",
                            "[http://localhost/logout]"});
                table351.AddRow(new string[] {
                            "initiate_login_uri",
                            "https://localhost/loginuri"});
#line 15
 testRunner.And("execute HTTP POST JSON request \'http://localhost/register\'", ((string)(null)), table351, "And ");
#line hidden
#line 43
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 45
 testRunner.Then("HTTP status code equals to \'201\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 46
 testRunner.Then("JSON exists \'client_id\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 47
 testRunner.Then("JSON exists \'client_secret\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 48
 testRunner.Then("JSON exists \'client_id_issued_at\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 49
 testRunner.Then("JSON exists \'grant_types\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 50
 testRunner.Then("JSON exists \'redirect_uris\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 51
 testRunner.Then("JSON exists \'response_types\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 52
 testRunner.Then("JSON exists \'default_acr_values\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 53
 testRunner.Then("JSON exists \'post_logout_redirect_uris\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 54
 testRunner.Then("JSON \'token_endpoint_auth_method\'=\'client_secret_jwt\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 55
 testRunner.Then("JSON \'client_name\'=\'name\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 56
 testRunner.Then("JSON \'client_name#fr\'=\'nom\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 57
 testRunner.Then("JSON \'client_name#en\'=\'name\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 58
 testRunner.Then("JSON \'application_type\'=\'web\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 59
 testRunner.Then("JSON \'subject_type\'=\'public\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 60
 testRunner.Then("JSON \'id_token_signed_response_alg\'=\'RS256\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 61
 testRunner.Then("JSON \'id_token_encrypted_response_alg\'=\'RSA-OAEP\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 62
 testRunner.Then("JSON \'id_token_encrypted_response_enc\'=\'A256CBC-HS512\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 63
 testRunner.Then("JSON \'userinfo_signed_response_alg\'=\'RS256\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 64
 testRunner.Then("JSON \'userinfo_encrypted_response_alg\'=\'RSA-OAEP\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 65
 testRunner.Then("JSON \'userinfo_encrypted_response_enc\'=\'A256CBC-HS512\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 66
 testRunner.Then("JSON \'request_object_signing_alg\'=\'RS256\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 67
 testRunner.Then("JSON \'request_object_encryption_alg\'=\'RSA-OAEP\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 68
 testRunner.Then("JSON \'request_object_encryption_enc\'=\'A256CBC-HS512\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 69
 testRunner.Then("JSON \'default_max_age\'=\'2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 70
 testRunner.Then("JSON \'require_auth_time\'=\'true\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 71
 testRunner.Then("JSON \'initiate_login_uri\'=\'https://localhost/loginuri\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Get a client")]
        [Xunit.TraitAttribute("FeatureTitle", "Register")]
        [Xunit.TraitAttribute("Description", "Get a client")]
        public void GetAClient()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get a client", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 73
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table352 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table352.AddRow(new string[] {
                            "client_id",
                            "fiftySevenClient"});
                table352.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table352.AddRow(new string[] {
                            "scope",
                            "register"});
                table352.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
#line 74
 testRunner.When("execute HTTP POST request \'http://localhost/token\'", ((string)(null)), table352, "When ");
#line hidden
#line 81
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 82
 testRunner.And("extract parameter \'access_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table353 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table353.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
                table353.AddRow(new string[] {
                            "redirect_uris",
                            "[https://web.com]"});
                table353.AddRow(new string[] {
                            "response_types",
                            "[token]"});
                table353.AddRow(new string[] {
                            "grant_types",
                            "[implicit]"});
                table353.AddRow(new string[] {
                            "client_name",
                            "name"});
                table353.AddRow(new string[] {
                            "client_name#fr",
                            "nom"});
                table353.AddRow(new string[] {
                            "client_name#en",
                            "name"});
                table353.AddRow(new string[] {
                            "application_type",
                            "web"});
                table353.AddRow(new string[] {
                            "token_endpoint_auth_method",
                            "client_secret_jwt"});
                table353.AddRow(new string[] {
                            "sector_identifier_uri",
                            "https://localhost/sector"});
                table353.AddRow(new string[] {
                            "subject_type",
                            "public"});
                table353.AddRow(new string[] {
                            "id_token_signed_response_alg",
                            "RS256"});
                table353.AddRow(new string[] {
                            "id_token_encrypted_response_alg",
                            "RSA-OAEP"});
                table353.AddRow(new string[] {
                            "id_token_encrypted_response_enc",
                            "A256CBC-HS512"});
                table353.AddRow(new string[] {
                            "userinfo_signed_response_alg",
                            "RS256"});
                table353.AddRow(new string[] {
                            "userinfo_encrypted_response_alg",
                            "RSA-OAEP"});
                table353.AddRow(new string[] {
                            "userinfo_encrypted_response_enc",
                            "A256CBC-HS512"});
                table353.AddRow(new string[] {
                            "request_object_signing_alg",
                            "RS256"});
                table353.AddRow(new string[] {
                            "request_object_encryption_alg",
                            "RSA-OAEP"});
                table353.AddRow(new string[] {
                            "request_object_encryption_enc",
                            "A256CBC-HS512"});
                table353.AddRow(new string[] {
                            "default_max_age",
                            "2"});
                table353.AddRow(new string[] {
                            "require_auth_time",
                            "true"});
                table353.AddRow(new string[] {
                            "default_acr_values",
                            "[a,b]"});
                table353.AddRow(new string[] {
                            "post_logout_redirect_uris",
                            "[http://localhost/logout]"});
                table353.AddRow(new string[] {
                            "initiate_login_uri",
                            "https://localhost/loginuri"});
#line 84
 testRunner.And("execute HTTP POST JSON request \'http://localhost/register\'", ((string)(null)), table353, "And ");
#line hidden
#line 112
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 113
 testRunner.And("extract parameter \'client_id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 114
 testRunner.And("extract parameter \'registration_access_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table354 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table354.AddRow(new string[] {
                            "Authorization",
                            "Bearer $registration_access_token$"});
#line 115
 testRunner.And("execute HTTP GET request \'http://localhost/register/$client_id$\'", ((string)(null)), table354, "And ");
#line hidden
#line 119
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 121
 testRunner.Then("HTTP status code equals to \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 122
 testRunner.Then("JSON exists \'client_id\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 123
 testRunner.Then("JSON exists \'client_secret\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 124
 testRunner.Then("JSON exists \'client_id_issued_at\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 125
 testRunner.Then("JSON exists \'grant_types\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 126
 testRunner.Then("JSON exists \'redirect_uris\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 127
 testRunner.Then("JSON exists \'response_types\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 128
 testRunner.Then("JSON exists \'default_acr_values\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 129
 testRunner.Then("JSON exists \'post_logout_redirect_uris\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 130
 testRunner.Then("JSON \'token_endpoint_auth_method\'=\'client_secret_jwt\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 131
 testRunner.Then("JSON \'client_name\'=\'name\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 132
 testRunner.Then("JSON \'client_name#fr\'=\'nom\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 133
 testRunner.Then("JSON \'client_name#en\'=\'name\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 134
 testRunner.Then("JSON \'application_type\'=\'web\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 135
 testRunner.Then("JSON \'subject_type\'=\'public\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 136
 testRunner.Then("JSON \'id_token_signed_response_alg\'=\'RS256\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 137
 testRunner.Then("JSON \'id_token_encrypted_response_alg\'=\'RSA-OAEP\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 138
 testRunner.Then("JSON \'id_token_encrypted_response_enc\'=\'A256CBC-HS512\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 139
 testRunner.Then("JSON \'userinfo_signed_response_alg\'=\'RS256\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 140
 testRunner.Then("JSON \'userinfo_encrypted_response_alg\'=\'RSA-OAEP\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 141
 testRunner.Then("JSON \'userinfo_encrypted_response_enc\'=\'A256CBC-HS512\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 142
 testRunner.Then("JSON \'request_object_signing_alg\'=\'RS256\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 143
 testRunner.Then("JSON \'request_object_encryption_alg\'=\'RSA-OAEP\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 144
 testRunner.Then("JSON \'request_object_encryption_enc\'=\'A256CBC-HS512\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 145
 testRunner.Then("JSON \'default_max_age\'=\'2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 146
 testRunner.Then("JSON \'require_auth_time\'=\'true\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 147
 testRunner.Then("JSON \'initiate_login_uri\'=\'https://localhost/loginuri\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                RegisterFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                RegisterFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
