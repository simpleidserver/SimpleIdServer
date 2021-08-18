﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.7.0.0
//      SpecFlow Generator Version:3.7.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SimpleIdServer.Scim.Host.Acceptance.Tests.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.7.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class GroupsFeature : object, Xunit.IClassFixture<GroupsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "Groups.feature"
#line hidden
        
        public GroupsFeature(GroupsFeature.FixtureData fixtureData, SimpleIdServer_Scim_Host_Acceptance_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Groups", "\tCheck the /Groups endpoint", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check users cannot be added twice to a group")]
        [Xunit.TraitAttribute("FeatureTitle", "Groups")]
        [Xunit.TraitAttribute("Description", "Check users cannot be added twice to a group")]
        public virtual void CheckUsersCannotBeAddedTwiceToAGroup()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check users cannot be added twice to a group", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 4
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table3.AddRow(new string[] {
                            "schemas",
                            "[ \"urn:ietf:params:scim:schemas:core:2.0:User\", \"urn:ietf:params:scim:schemas:ext" +
                                "ension:enterprise:2.0:User\" ]"});
                table3.AddRow(new string[] {
                            "userName",
                            "bjen"});
                table3.AddRow(new string[] {
                            "externalId",
                            "externalid"});
                table3.AddRow(new string[] {
                            "name",
                            "{ \"formatted\" : \"formatted\", \"familyName\": \"familyName\", \"givenName\": \"givenName\"" +
                                " }"});
                table3.AddRow(new string[] {
                            "employeeNumber",
                            "number"});
#line 5
 testRunner.When("execute HTTP POST JSON request \'http://localhost/Users\'", ((string)(null)), table3, "When ");
#line hidden
#line 12
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 13
 testRunner.And("extract \'id\' from JSON body into \'userid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table4.AddRow(new string[] {
                            "schemas",
                            "[ \"urn:ietf:params:scim:schemas:core:2.0:Group\" ]"});
                table4.AddRow(new string[] {
                            "displayName",
                            "Tour Guides"});
                table4.AddRow(new string[] {
                            "members",
                            "[ { \"value\": \"$userid$\" } ]"});
#line 15
 testRunner.And("execute HTTP POST JSON request \'http://localhost/Groups\'", ((string)(null)), table4, "And ");
#line hidden
#line 20
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 21
 testRunner.And("extract \'id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table5.AddRow(new string[] {
                            "schemas",
                            "[ \"urn:ietf:params:scim:api:messages:2.0:PatchOp\" ]"});
                table5.AddRow(new string[] {
                            "Operations",
                            "[ { \"op\": \"add\", \"path\": \"members\", \"value\": [ { \"value\": \"$userid$\" } ] } ]"});
#line 23
 testRunner.And("execute HTTP PATCH JSON request \'http://localhost/Groups/$id$\'", ((string)(null)), table5, "And ");
#line hidden
#line 27
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 29
 testRunner.Then("HTTP status code equals to \'409\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 30
 testRunner.Then("JSON \'response.status\'=\'409\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 31
 testRunner.Then("JSON \'response.scimType\'=\'uniqueness\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check user can be added to a group")]
        [Xunit.TraitAttribute("FeatureTitle", "Groups")]
        [Xunit.TraitAttribute("Description", "Check user can be added to a group")]
        public virtual void CheckUserCanBeAddedToAGroup()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check user can be added to a group", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 33
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table6.AddRow(new string[] {
                            "schemas",
                            "[ \"urn:ietf:params:scim:schemas:core:2.0:User\", \"urn:ietf:params:scim:schemas:ext" +
                                "ension:enterprise:2.0:User\" ]"});
                table6.AddRow(new string[] {
                            "userName",
                            "bjen"});
                table6.AddRow(new string[] {
                            "externalId",
                            "externalid"});
                table6.AddRow(new string[] {
                            "name",
                            "{ \"formatted\" : \"formatted\", \"familyName\": \"familyName\", \"givenName\": \"givenName\"" +
                                " }"});
                table6.AddRow(new string[] {
                            "employeeNumber",
                            "number"});
#line 34
 testRunner.When("execute HTTP POST JSON request \'http://localhost/Users\'", ((string)(null)), table6, "When ");
#line hidden
#line 42
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 43
 testRunner.And("extract \'id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table7.AddRow(new string[] {
                            "schemas",
                            "[ \"urn:ietf:params:scim:schemas:core:2.0:Group\" ]"});
                table7.AddRow(new string[] {
                            "displayName",
                            "Tour Guides"});
                table7.AddRow(new string[] {
                            "members",
                            "[ { \"value\": \"$id$\" } ]"});
#line 44
 testRunner.And("execute HTTP POST JSON request \'http://localhost/Groups\'", ((string)(null)), table7, "And ");
#line hidden
#line 49
 testRunner.And("execute HTTP GET request \'http://localhost/Users/$id$\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 50
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 52
 testRunner.Then("HTTP status code equals to \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 53
 testRunner.Then("HTTP HEADER contains \'Location\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 54
 testRunner.Then("HTTP HEADER contains \'ETag\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 55
 testRunner.Then("JSON exists \'id\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 56
 testRunner.Then("JSON exists \'meta.created\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 57
 testRunner.Then("JSON exists \'meta.lastModified\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 58
 testRunner.Then("JSON exists \'meta.version\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 59
 testRunner.Then("JSON exists \'meta.location\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 60
 testRunner.Then("JSON exists \'groups[0].value\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 61
 testRunner.Then("JSON \'groups[0].display\'=\'Tour guides\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check group can be updated with multiple users")]
        [Xunit.TraitAttribute("FeatureTitle", "Groups")]
        [Xunit.TraitAttribute("Description", "Check group can be updated with multiple users")]
        public virtual void CheckGroupCanBeUpdatedWithMultipleUsers()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check group can be updated with multiple users", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 63
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table8.AddRow(new string[] {
                            "schemas",
                            "[ \"urn:ietf:params:scim:schemas:core:2.0:User\", \"urn:ietf:params:scim:schemas:ext" +
                                "ension:enterprise:2.0:User\" ]"});
                table8.AddRow(new string[] {
                            "userName",
                            "bjen"});
                table8.AddRow(new string[] {
                            "externalId",
                            "externalid"});
                table8.AddRow(new string[] {
                            "name",
                            "{ \"formatted\" : \"formatted\", \"familyName\": \"familyName\", \"givenName\": \"givenName\"" +
                                " }"});
                table8.AddRow(new string[] {
                            "employeeNumber",
                            "number"});
#line 64
 testRunner.When("execute HTTP POST JSON request \'http://localhost/Users\'", ((string)(null)), table8, "When ");
#line hidden
#line 72
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 73
 testRunner.And("extract \'id\' from JSON body into \'firstuserid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table9.AddRow(new string[] {
                            "schemas",
                            "[ \"urn:ietf:params:scim:schemas:core:2.0:User\", \"urn:ietf:params:scim:schemas:ext" +
                                "ension:enterprise:2.0:User\" ]"});
                table9.AddRow(new string[] {
                            "userName",
                            "bjen2"});
                table9.AddRow(new string[] {
                            "externalId",
                            "externalid"});
                table9.AddRow(new string[] {
                            "name",
                            "{ \"formatted\" : \"formatted\", \"familyName\": \"familyName\", \"givenName\": \"givenName\"" +
                                " }"});
                table9.AddRow(new string[] {
                            "employeeNumber",
                            "number"});
#line 74
 testRunner.And("execute HTTP POST JSON request \'http://localhost/Users\'", ((string)(null)), table9, "And ");
#line hidden
#line 81
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 82
 testRunner.And("extract \'id\' from JSON body into \'seconduserid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table10.AddRow(new string[] {
                            "schemas",
                            "[ \"urn:ietf:params:scim:schemas:core:2.0:Group\" ]"});
                table10.AddRow(new string[] {
                            "displayName",
                            "Tour Guides"});
                table10.AddRow(new string[] {
                            "members",
                            "[ { \"value\": \"$firstuserid$\" } ]"});
#line 83
 testRunner.And("execute HTTP POST JSON request \'http://localhost/Groups\'", ((string)(null)), table10, "And ");
#line hidden
#line 88
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 89
 testRunner.And("extract \'id\' from JSON body into \'groupid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table11.AddRow(new string[] {
                            "schemas",
                            "[ \"urn:ietf:params:scim:schemas:core:2.0:Group\" ]"});
                table11.AddRow(new string[] {
                            "members",
                            "[ { \"value\": \"$firstuserid$\" }, { \"value\": \"$seconduserid$\" } ]"});
#line 90
 testRunner.And("execute HTTP PUT JSON request \'http://localhost/Groups/$groupid$\'", ((string)(null)), table11, "And ");
#line hidden
#line 94
 testRunner.And("execute HTTP GET request \'http://localhost/Groups/$groupid$\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 95
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 97
 testRunner.Then("HTTP status code equals to \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 98
 testRunner.Then("JSON exists \'members[0].value\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 99
 testRunner.Then("JSON exists \'members[1].value\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.7.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                GroupsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                GroupsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
