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
    public partial class UMAResourceFeature : object, Xunit.IClassFixture<UMAResourceFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "UMAResource.feature"
#line hidden
        
        public UMAResourceFeature(UMAResourceFeature.FixtureData fixtureData, SimpleIdServer_IdServer_Host_Acceptance_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "UMAResource", "\tCheck the endpoint /rreguri\t", ProgrammingLanguage.CSharp, featureTags);
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
        
        [Xunit.SkippableFactAttribute(DisplayName="add UMA resource")]
        [Xunit.TraitAttribute("FeatureTitle", "UMAResource")]
        [Xunit.TraitAttribute("Description", "add UMA resource")]
        public void AddUMAResource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("add UMA resource", null, tagsOfScenario, argumentsOfScenario, featureTags);
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
                TechTalk.SpecFlow.Table table429 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table429.AddRow(new string[] {
                            "client_id",
                            "fiftyThreeClient"});
                table429.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table429.AddRow(new string[] {
                            "scope",
                            "uma_protection"});
                table429.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
#line 5
 testRunner.When("execute HTTP POST request \'http://localhost/token\'", ((string)(null)), table429, "When ");
#line hidden
#line 12
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 13
 testRunner.And("extract parameter \'access_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table430 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table430.AddRow(new string[] {
                            "resource_scopes",
                            "[scope1,scope2]"});
                table430.AddRow(new string[] {
                            "subject",
                            "user1"});
                table430.AddRow(new string[] {
                            "icon_uri",
                            "icon"});
                table430.AddRow(new string[] {
                            "name#fr",
                            "nom"});
                table430.AddRow(new string[] {
                            "name#en",
                            "name"});
                table430.AddRow(new string[] {
                            "description#fr",
                            "descriptionFR"});
                table430.AddRow(new string[] {
                            "description#en",
                            "descriptionEN"});
                table430.AddRow(new string[] {
                            "type",
                            "type"});
                table430.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
#line 15
 testRunner.And("execute HTTP POST JSON request \'http://localhost/rreguri\'", ((string)(null)), table430, "And ");
#line hidden
#line 27
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 29
 testRunner.Then("HTTP status code equals to \'201\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 30
 testRunner.And("JSON exists \'_id\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 31
 testRunner.And("JSON exists \'user_access_policy_uri\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="get UMA resource")]
        [Xunit.TraitAttribute("FeatureTitle", "UMAResource")]
        [Xunit.TraitAttribute("Description", "get UMA resource")]
        public void GetUMAResource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("get UMA resource", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 33
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table431 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table431.AddRow(new string[] {
                            "client_id",
                            "fiftyThreeClient"});
                table431.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table431.AddRow(new string[] {
                            "scope",
                            "uma_protection"});
                table431.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
#line 34
 testRunner.When("execute HTTP POST request \'http://localhost/token\'", ((string)(null)), table431, "When ");
#line hidden
#line 41
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 42
 testRunner.And("extract parameter \'access_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table432 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table432.AddRow(new string[] {
                            "resource_scopes",
                            "[scope1,scope2]"});
                table432.AddRow(new string[] {
                            "subject",
                            "user1"});
                table432.AddRow(new string[] {
                            "icon_uri",
                            "icon"});
                table432.AddRow(new string[] {
                            "name#fr",
                            "nom"});
                table432.AddRow(new string[] {
                            "name#en",
                            "name"});
                table432.AddRow(new string[] {
                            "description#fr",
                            "descriptionFR"});
                table432.AddRow(new string[] {
                            "description#en",
                            "descriptionEN"});
                table432.AddRow(new string[] {
                            "type",
                            "type"});
                table432.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
#line 44
 testRunner.And("execute HTTP POST JSON request \'http://localhost/rreguri\'", ((string)(null)), table432, "And ");
#line hidden
#line 56
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 57
 testRunner.And("extract parameter \'_id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table433 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table433.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
#line 59
 testRunner.And("execute HTTP GET request \'http://localhost/rreguri/$_id$\'", ((string)(null)), table433, "And ");
#line hidden
#line 63
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 65
 testRunner.Then("HTTP status code equals to \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 66
 testRunner.And("JSON exists \'resource_scopes\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 67
 testRunner.And("JSON \'icon_uri\'=\'icon\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 68
 testRunner.And("JSON \'name#fr\'=\'nom\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 69
 testRunner.And("JSON \'name#en\'=\'name\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 70
 testRunner.And("JSON \'description#fr\'=\'descriptionFR\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 71
 testRunner.And("JSON \'description#en\'=\'descriptionEN\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 72
 testRunner.And("JSON \'type\'=\'type\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="delete UMA resource")]
        [Xunit.TraitAttribute("FeatureTitle", "UMAResource")]
        [Xunit.TraitAttribute("Description", "delete UMA resource")]
        public void DeleteUMAResource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("delete UMA resource", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 74
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table434 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table434.AddRow(new string[] {
                            "client_id",
                            "fiftyThreeClient"});
                table434.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table434.AddRow(new string[] {
                            "scope",
                            "uma_protection"});
                table434.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
#line 75
 testRunner.When("execute HTTP POST request \'http://localhost/token\'", ((string)(null)), table434, "When ");
#line hidden
#line 82
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 83
 testRunner.And("extract parameter \'access_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table435 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table435.AddRow(new string[] {
                            "resource_scopes",
                            "[scope1,scope2]"});
                table435.AddRow(new string[] {
                            "subject",
                            "user1"});
                table435.AddRow(new string[] {
                            "icon_uri",
                            "icon"});
                table435.AddRow(new string[] {
                            "name#fr",
                            "nom"});
                table435.AddRow(new string[] {
                            "name#en",
                            "name"});
                table435.AddRow(new string[] {
                            "description#fr",
                            "descriptionFR"});
                table435.AddRow(new string[] {
                            "description#en",
                            "descriptionEN"});
                table435.AddRow(new string[] {
                            "type",
                            "type"});
                table435.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
#line 85
 testRunner.And("execute HTTP POST JSON request \'http://localhost/rreguri\'", ((string)(null)), table435, "And ");
#line hidden
#line 97
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 98
 testRunner.And("extract parameter \'_id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table436 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table436.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
#line 100
 testRunner.And("execute HTTP DELETE request \'http://localhost/rreguri/$_id$\'", ((string)(null)), table436, "And ");
#line hidden
#line 104
 testRunner.Then("HTTP status code equals to \'204\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="add UMA permissions")]
        [Xunit.TraitAttribute("FeatureTitle", "UMAResource")]
        [Xunit.TraitAttribute("Description", "add UMA permissions")]
        public void AddUMAPermissions()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("add UMA permissions", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 106
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table437 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table437.AddRow(new string[] {
                            "client_id",
                            "fiftyThreeClient"});
                table437.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table437.AddRow(new string[] {
                            "scope",
                            "uma_protection"});
                table437.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
#line 107
 testRunner.When("execute HTTP POST request \'http://localhost/token\'", ((string)(null)), table437, "When ");
#line hidden
#line 114
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 115
 testRunner.And("extract parameter \'access_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table438 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table438.AddRow(new string[] {
                            "resource_scopes",
                            "[scope1,scope2]"});
                table438.AddRow(new string[] {
                            "subject",
                            "user1"});
                table438.AddRow(new string[] {
                            "icon_uri",
                            "icon"});
                table438.AddRow(new string[] {
                            "name#fr",
                            "nom"});
                table438.AddRow(new string[] {
                            "name#en",
                            "name"});
                table438.AddRow(new string[] {
                            "description#fr",
                            "descriptionFR"});
                table438.AddRow(new string[] {
                            "description#en",
                            "descriptionEN"});
                table438.AddRow(new string[] {
                            "type",
                            "type"});
                table438.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
#line 117
 testRunner.And("execute HTTP POST JSON request \'http://localhost/rreguri\'", ((string)(null)), table438, "And ");
#line hidden
#line 129
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 130
 testRunner.And("extract parameter \'_id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table439 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table439.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
                table439.AddRow(new string[] {
                            "permissions",
                            "[ { \"claims\": [ { \"name\": \"sub\", \"value\": \"user\" } ], \"scopes\": [ \"scope\" ] } ]"});
#line 132
 testRunner.And("execute HTTP PUT JSON request \'http://localhost/rreguri/$_id$/permissions\'", ((string)(null)), table439, "And ");
#line hidden
#line 137
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 139
 testRunner.Then("HTTP status code equals to \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 140
 testRunner.And("JSON exists \'_id\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="delete UMA permissions")]
        [Xunit.TraitAttribute("FeatureTitle", "UMAResource")]
        [Xunit.TraitAttribute("Description", "delete UMA permissions")]
        public void DeleteUMAPermissions()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("delete UMA permissions", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 142
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table440 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table440.AddRow(new string[] {
                            "client_id",
                            "fiftyThreeClient"});
                table440.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table440.AddRow(new string[] {
                            "scope",
                            "uma_protection"});
                table440.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
#line 143
 testRunner.When("execute HTTP POST request \'http://localhost/token\'", ((string)(null)), table440, "When ");
#line hidden
#line 150
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 151
 testRunner.And("extract parameter \'access_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table441 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table441.AddRow(new string[] {
                            "resource_scopes",
                            "[scope1,scope2]"});
                table441.AddRow(new string[] {
                            "subject",
                            "user1"});
                table441.AddRow(new string[] {
                            "icon_uri",
                            "icon"});
                table441.AddRow(new string[] {
                            "name#fr",
                            "nom"});
                table441.AddRow(new string[] {
                            "name#en",
                            "name"});
                table441.AddRow(new string[] {
                            "description#fr",
                            "descriptionFR"});
                table441.AddRow(new string[] {
                            "description#en",
                            "descriptionEN"});
                table441.AddRow(new string[] {
                            "type",
                            "type"});
                table441.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
#line 153
 testRunner.And("execute HTTP POST JSON request \'http://localhost/rreguri\'", ((string)(null)), table441, "And ");
#line hidden
#line 165
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 166
 testRunner.And("extract parameter \'_id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table442 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table442.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
                table442.AddRow(new string[] {
                            "permissions",
                            "[ { \"claims\": [ { \"name\": \"sub\", \"value\": \"user\" } ], \"scopes\": [ \"scope\" ] } ]"});
#line 168
 testRunner.And("execute HTTP POST JSON request \'http://localhost/rreguri/$_id$/permissions\'", ((string)(null)), table442, "And ");
#line hidden
                TechTalk.SpecFlow.Table table443 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table443.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
#line 173
 testRunner.And("execute HTTP DELETE request \'http://localhost/rreguri/$_id$\'", ((string)(null)), table443, "And ");
#line hidden
#line 177
 testRunner.Then("HTTP status code equals to \'204\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
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
                UMAResourceFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                UMAResourceFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
