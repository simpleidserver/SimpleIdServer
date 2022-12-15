using TechTalk.SpecFlow;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class DataPreparationSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public DataPreparationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given("authenticate a user")]
        public void GivenUserIsAuthenticated()
        {
            _scenarioContext.EnableUserAuthentication();
        }
    }
}
