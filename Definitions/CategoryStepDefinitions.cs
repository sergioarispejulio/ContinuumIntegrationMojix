using Continuum_Integration.class_objects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using TechTalk.SpecFlow;

namespace Continuum_Integration.Definitions
{
    [Binding]
    public class CategoryStepDefinitions
    {
        RestClient client;
        RestResponse response;
        RestRequest request;

        private string nameServer = "http://demostore.gatling.io";
        private string codeAuthorization;

        [BeforeScenario]
        public void SetUpUserForScenario()
        {
            client = new RestClient(nameServer);
        }

        [Given(@"I want all categories")]
        public void GivenIWantAllCategories()
        {
            request = new RestRequest("/api/category", Method.Get);
        }

        [When(@"I send a request - Category")]
        public void WhenISendARequest_Category()
        {
            response = client.Execute(request);
        }

        [Then(@"I expected a list of categories")]
        public void ThenIExpectedAListOfCategories()
        {
            JArray jsonArray = JArray.Parse(response.Content.ToString());
            List<Category> listProducts = JsonConvert.DeserializeObject<List<Category>>(jsonArray.ToString());
            Assert.That(jsonArray.Count, Is.AtLeast(1), "The list of categories is empty");
        }
    }
}
