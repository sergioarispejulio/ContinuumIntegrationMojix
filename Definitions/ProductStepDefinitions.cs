using Continuum_Integration.class_objects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using TechTalk.SpecFlow;

namespace Continuum_Integration.Definitions
{
    [Binding]
    public class ProductStepDefinitions
    {
        RestClient client;
        RestResponse response;
        RestRequest request;

        private string nameServer = "http://demostore.gatling.io";
        private string codeAuthorization;

        int idCategory;

        [BeforeScenario]
        public void SetUpUserForScenario()
        {
            client = new RestClient(nameServer);
        }

        [Given(@"I want all products")]
        public void GivenIWantAllProducts()
        {
            request = new RestRequest("/api/product", Method.Get);
        }

        [When(@"I send a request - Product")]
        public void WhenISendARequest_Product()
        {
            response = client.Execute(request);
        }

        [Then(@"I expected a list of products")]
        public void ThenIExpectedAListOfProducts()
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Error code in the request: " + response.StatusCode);

            JArray jsonArray = JArray.Parse(response.Content.ToString());
            List<Product> listProducts = JsonConvert.DeserializeObject<List<Product>>(jsonArray.ToString());
            Assert.That(jsonArray.Count, Is.AtLeast(0), "The list of product is empty");
        }

        [Given(@"I need login to the API")]
        public void GivenINeedLoginToTheAPI()
        {
            request = new RestRequest("/api/authenticate", Method.Post);
            request.AddJsonBody(new { username = "admin", password = "admin" });
            response = client.Execute(request); 

            dynamic jsonObj = JsonConvert.DeserializeObject(response.Content.ToString());
            codeAuthorization = (string)jsonObj.token;

            client.AddDefaultHeader("Authorization", "Bearer " + codeAuthorization);
            request = new RestRequest("/api/product", Method.Post);
        }

        [When(@"I setting the product to create")]
        public void WhenISettingTheProductToCreate()
        {
            request.AddJsonBody(new { name = "ASDF", description = "ASDF", image = "ASDF.jpg", price = "15.99", categoryId = "7" });
        }

        [Then(@"I expected a bad request to confirm the creation \(API rules\)")]
        public void ThenIExpectedABadRequestToConfirmTheCreationAPIRules()
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "The product wasn't create");
        }

        [Given(@"I have the ID of a category with value (.*)")]
        public void GivenIHaveTheIDOfACategoryWithValue(int p0)
        {
            request = new RestRequest("/api/product?category={idcategory}", Method.Get);
            request.AddUrlSegment("idcategory", p0);
            idCategory = p0;
        }

        [Then(@"I expected a list of products only with the category")]
        public void ThenIExpectedAListOfProductsOnlyWithTheCategory()
        {
            bool result = true;
            JArray jsonArray = JArray.Parse(response.Content.ToString());
            List<Product> listProducts = JsonConvert.DeserializeObject<List<Product>>(jsonArray.ToString());
            Console.WriteLine("Count: " + listProducts.Count);
            foreach (Product element in listProducts)
            {
                Console.WriteLine("ID: " + element.id);
                Console.WriteLine("Category ID: " + element.categoryId);
                if (element.categoryId != idCategory)
                {
                    result = false;
                    break;
                }
            }
            Assert.That(result, Is.EqualTo(true), "The list have a products with wrong category");
        }
    }
}
