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
namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Features.GrantTypes
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class AuthorizationCodeGrantTypeErrorsFeature : object, Xunit.IClassFixture<AuthorizationCodeGrantTypeErrorsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "AuthorizationCodeGrantTypeErrors.feature"
#line hidden
        
        public AuthorizationCodeGrantTypeErrorsFeature(AuthorizationCodeGrantTypeErrorsFeature.FixtureData fixtureData, SimpleIdServer_IdServer_Host_Acceptance_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/GrantTypes", "AuthorizationCodeGrantTypeErrors", "\tCheck errors returned when using \'authorization_code\' grant-type", ProgrammingLanguage.CSharp, featureTags);
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
        
        [Xunit.SkippableFactAttribute(DisplayName="Send \'grant_type=authorization_code\' with no code parameter")]
        [Xunit.TraitAttribute("FeatureTitle", "AuthorizationCodeGrantTypeErrors")]
        [Xunit.TraitAttribute("Description", "Send \'grant_type=authorization_code\' with no code parameter")]
        public void SendGrant_TypeAuthorization_CodeWithNoCodeParameter()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send \'grant_type=authorization_code\' with no code parameter", null, tagsOfScenario, argumentsOfScenario, featureTags);
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
                TechTalk.SpecFlow.Table table160 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table160.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
#line 5
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table160, "When ");
#line hidden
#line 9
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 10
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 11
 testRunner.And("JSON \'$.error\'=\'invalid_request\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 12
 testRunner.And("JSON \'$.error_description\'=\'missing parameter code\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Send \'grant_type=authorization_code,code=code\' with no redirect_uri")]
        [Xunit.TraitAttribute("FeatureTitle", "AuthorizationCodeGrantTypeErrors")]
        [Xunit.TraitAttribute("Description", "Send \'grant_type=authorization_code,code=code\' with no redirect_uri")]
        public void SendGrant_TypeAuthorization_CodeCodeCodeWithNoRedirect_Uri()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send \'grant_type=authorization_code,code=code\' with no redirect_uri", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 14
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table161 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table161.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table161.AddRow(new string[] {
                            "code",
                            "code"});
#line 15
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table161, "When ");
#line hidden
#line 20
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 21
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 22
 testRunner.And("JSON \'$.error\'=\'invalid_request\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 23
 testRunner.And("JSON \'$.error_description\'=\'missing parameter redirect_uri\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost,clien" +
            "t_id=firstClient,client_secret=password\' with unauthorized grant_type")]
        [Xunit.TraitAttribute("FeatureTitle", "AuthorizationCodeGrantTypeErrors")]
        [Xunit.TraitAttribute("Description", "Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost,clien" +
            "t_id=firstClient,client_secret=password\' with unauthorized grant_type")]
        public void SendGrant_TypeAuthorization_CodeCodeCodeRedirect_UriHttpLocalhostClient_IdFirstClientClient_SecretPasswordWithUnauthorizedGrant_Type()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost,clien" +
                    "t_id=firstClient,client_secret=password\' with unauthorized grant_type", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 25
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table162 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table162.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table162.AddRow(new string[] {
                            "code",
                            "code"});
                table162.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost"});
                table162.AddRow(new string[] {
                            "client_id",
                            "firstClient"});
                table162.AddRow(new string[] {
                            "client_secret",
                            "password"});
#line 26
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table162, "When ");
#line hidden
#line 34
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 35
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 36
 testRunner.And("JSON \'$.error\'=\'invalid_client\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 37
 testRunner.And("JSON \'$.error_description\'=\'grant type authorization_code is not supported by the" +
                        " client\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080," +
            "client_id=thirdClient,client_secret=password\' with previous issued token")]
        [Xunit.TraitAttribute("FeatureTitle", "AuthorizationCodeGrantTypeErrors")]
        [Xunit.TraitAttribute("Description", "Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080," +
            "client_id=thirdClient,client_secret=password\' with previous issued token")]
        public void SendGrant_TypeAuthorization_CodeCodeCodeRedirect_UriHttpLocalhost8080Client_IdThirdClientClient_SecretPasswordWithPreviousIssuedToken()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080," +
                    "client_id=thirdClient,client_secret=password\' with previous issued token", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 39
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 40
 testRunner.Given("authenticate a user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table163 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table163.AddRow(new string[] {
                            "response_type",
                            "code"});
                table163.AddRow(new string[] {
                            "client_id",
                            "thirdClient"});
                table163.AddRow(new string[] {
                            "state",
                            "state"});
                table163.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
                table163.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table163.AddRow(new string[] {
                            "scope",
                            "secondScope"});
#line 41
 testRunner.When("execute HTTP GET request \'https://localhost:8080/authorization\'", ((string)(null)), table163, "When ");
#line hidden
#line 50
 testRunner.And("extract parameter \'code\' from redirect url", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table164 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table164.AddRow(new string[] {
                            "client_id",
                            "thirdClient"});
                table164.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table164.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table164.AddRow(new string[] {
                            "code",
                            "$code$"});
                table164.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
#line 52
 testRunner.And("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table164, "And ");
#line hidden
                TechTalk.SpecFlow.Table table165 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table165.AddRow(new string[] {
                            "client_id",
                            "thirdClient"});
                table165.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table165.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table165.AddRow(new string[] {
                            "code",
                            "$code$"});
                table165.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
#line 60
 testRunner.And("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table165, "And ");
#line hidden
#line 68
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 69
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 70
 testRunner.And("JSON \'$.error\'=\'invalid_grant\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 71
 testRunner.And("JSON \'$.error_description\'=\'authorization code has already been used, all tokens " +
                        "previously issued have been revoked\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080," +
            "client_id=thirdClient,client_secret=password\' with bad code")]
        [Xunit.TraitAttribute("FeatureTitle", "AuthorizationCodeGrantTypeErrors")]
        [Xunit.TraitAttribute("Description", "Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080," +
            "client_id=thirdClient,client_secret=password\' with bad code")]
        public void SendGrant_TypeAuthorization_CodeCodeCodeRedirect_UriHttpLocalhost8080Client_IdThirdClientClient_SecretPasswordWithBadCode()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080," +
                    "client_id=thirdClient,client_secret=password\' with bad code", null, tagsOfScenario, argumentsOfScenario, featureTags);
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
                TechTalk.SpecFlow.Table table166 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table166.AddRow(new string[] {
                            "client_id",
                            "thirdClient"});
                table166.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table166.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table166.AddRow(new string[] {
                            "code",
                            "invalidCode"});
                table166.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
#line 74
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table166, "When ");
#line hidden
#line 82
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 83
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 84
 testRunner.And("JSON \'$.error\'=\'invalid_grant\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 85
 testRunner.And("JSON \'$.error_description\'=\'bad authorization code\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080," +
            "client_id=thirdClient,client_secret=password\' with code not issued by the client" +
            "")]
        [Xunit.TraitAttribute("FeatureTitle", "AuthorizationCodeGrantTypeErrors")]
        [Xunit.TraitAttribute("Description", "Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080," +
            "client_id=thirdClient,client_secret=password\' with code not issued by the client" +
            "")]
        public void SendGrant_TypeAuthorization_CodeCodeCodeRedirect_UriHttpLocalhost8080Client_IdThirdClientClient_SecretPasswordWithCodeNotIssuedByTheClient()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send \'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080," +
                    "client_id=thirdClient,client_secret=password\' with code not issued by the client" +
                    "", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 87
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 88
 testRunner.Given("authenticate a user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table167 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table167.AddRow(new string[] {
                            "response_type",
                            "code"});
                table167.AddRow(new string[] {
                            "client_id",
                            "thirdClient"});
                table167.AddRow(new string[] {
                            "state",
                            "state"});
                table167.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
                table167.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table167.AddRow(new string[] {
                            "scope",
                            "secondScope"});
#line 89
 testRunner.When("execute HTTP GET request \'https://localhost:8080/authorization\'", ((string)(null)), table167, "When ");
#line hidden
#line 98
 testRunner.And("extract parameter \'code\' from redirect url", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table168 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table168.AddRow(new string[] {
                            "client_id",
                            "thirdClient"});
                table168.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table168.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table168.AddRow(new string[] {
                            "code",
                            "$code$"});
                table168.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:9080"});
#line 100
 testRunner.And("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table168, "And ");
#line hidden
#line 108
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 109
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 110
 testRunner.And("JSON \'$.error\'=\'invalid_grant\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 111
 testRunner.And("JSON \'$.error_description\'=\'not the same redirect_uri\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="authorization code cannot be used twice")]
        [Xunit.TraitAttribute("FeatureTitle", "AuthorizationCodeGrantTypeErrors")]
        [Xunit.TraitAttribute("Description", "authorization code cannot be used twice")]
        public void AuthorizationCodeCannotBeUsedTwice()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("authorization code cannot be used twice", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 113
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 114
 testRunner.Given("authenticate a user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table169 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table169.AddRow(new string[] {
                            "response_type",
                            "code"});
                table169.AddRow(new string[] {
                            "client_id",
                            "thirdClient"});
                table169.AddRow(new string[] {
                            "state",
                            "state"});
                table169.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
                table169.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table169.AddRow(new string[] {
                            "scope",
                            "secondScope"});
#line 115
 testRunner.When("execute HTTP GET request \'https://localhost:8080/authorization\'", ((string)(null)), table169, "When ");
#line hidden
#line 124
 testRunner.And("extract parameter \'code\' from redirect url", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table170 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table170.AddRow(new string[] {
                            "client_id",
                            "thirdClient"});
                table170.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table170.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table170.AddRow(new string[] {
                            "code",
                            "$code$"});
                table170.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
#line 126
 testRunner.And("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table170, "And ");
#line hidden
                TechTalk.SpecFlow.Table table171 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table171.AddRow(new string[] {
                            "client_id",
                            "thirdClient"});
                table171.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table171.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table171.AddRow(new string[] {
                            "code",
                            "$code$"});
                table171.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
#line 134
 testRunner.And("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table171, "And ");
#line hidden
#line 142
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 143
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 144
 testRunner.Then("JSON \'error\'=\'invalid_grant\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 145
 testRunner.Then("JSON \'error_description\'=\'authorization code has already been used, all tokens pr" +
                        "eviously issued have been revoked\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
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
                AuthorizationCodeGrantTypeErrorsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                AuthorizationCodeGrantTypeErrorsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
