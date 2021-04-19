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
namespace SimpleIdServer.OpenID.Host.Acceptance.Tests.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.7.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class BCDeviceRegistrationFeature : object, Xunit.IClassFixture<BCDeviceRegistrationFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "BCDeviceRegistration.feature"
#line hidden
        
        public BCDeviceRegistrationFeature(BCDeviceRegistrationFeature.FixtureData fixtureData, SimpleIdServer_OpenID_Host_Acceptance_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "BCDeviceRegistration", "\tCheck /bc-device-registration endpoint", ProgrammingLanguage.CSharp, ((string[])(null)));
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
        
        [Xunit.SkippableFactAttribute(DisplayName="Update \'device_registration_token\'")]
        [Xunit.TraitAttribute("FeatureTitle", "BCDeviceRegistration")]
        [Xunit.TraitAttribute("Description", "Update \'device_registration_token\'")]
        public virtual void UpdateDevice_Registration_Token()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Update \'device_registration_token\'", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
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
                TechTalk.SpecFlow.Table table223 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table223.AddRow(new string[] {
                            "redirect_uris",
                            "[https://web.com]"});
                table223.AddRow(new string[] {
                            "grant_types",
                            "[implicit]"});
                table223.AddRow(new string[] {
                            "response_types",
                            "[id_token]"});
                table223.AddRow(new string[] {
                            "scope",
                            "openid email role"});
                table223.AddRow(new string[] {
                            "id_token_signed_response_alg",
                            "none"});
#line 5
 testRunner.When("execute HTTP POST JSON request \'http://localhost/register\'", ((string)(null)), table223, "When ");
#line hidden
#line 13
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 14
 testRunner.And("extract parameter \'client_id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 15
 testRunner.And("extract parameter \'client_secret\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 16
 testRunner.And("add user consent : user=\'administrator\', scope=\'email role\', clientId=\'$client_id" +
                        "$\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table224 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table224.AddRow(new string[] {
                            "response_type",
                            "id_token"});
                table224.AddRow(new string[] {
                            "client_id",
                            "$client_id$"});
                table224.AddRow(new string[] {
                            "state",
                            "state"});
                table224.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table224.AddRow(new string[] {
                            "scope",
                            "openid email role"});
                table224.AddRow(new string[] {
                            "redirect_uri",
                            "https://web.com"});
                table224.AddRow(new string[] {
                            "ui_locales",
                            "en fr"});
                table224.AddRow(new string[] {
                            "nonce",
                            "nonce"});
#line 18
 testRunner.And("execute HTTP GET request \'http://localhost/authorization\'", ((string)(null)), table224, "And ");
#line hidden
#line 29
 testRunner.And("extract \'id_token\' from callback", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table225 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table225.AddRow(new string[] {
                            "id_token_hint",
                            "$id_token$"});
                table225.AddRow(new string[] {
                            "device_registration_token",
                            "device"});
#line 31
 testRunner.When("execute HTTP POST JSON request \'http://localhost/bc-device-registration\'", ((string)(null)), table225, "When ");
#line hidden
#line 36
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 38
 testRunner.Then("HTTP status code equals to \'204\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
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
                BCDeviceRegistrationFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                BCDeviceRegistrationFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
