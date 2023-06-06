using Continuum_Integration.class_objects;
using Gherkin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using SpecFlow.Internal.Json;
using System.Net;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace Continuum_Integration
{
    public class Tests
    {
        RestClient client;
        RestResponse response;

        private string nameServer = "http://demostore.gatling.io";
        private string codeAuthorization;

        int idCategory = 5;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(nameServer);
        }

        private void Login()
        {
            RestRequest request = new RestRequest("/api/authenticate", Method.Post);
            /*string json = "{ \"username\": \"admin\",  \"password\": \"admin\"\r\n}";
            request.AddJsonBody(json);*/

            request.AddJsonBody(new { username = "admin", password = "admin" });
            response = client.Execute(request); ;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "The Login wasn't success");

            dynamic jsonObj = JsonConvert.DeserializeObject(response.Content.ToString());
            codeAuthorization = (string)jsonObj.token;
        }

        [Test]
        public void Test0_Login()
        {
            RestRequest request = new RestRequest("/api/authenticate", Method.Post);
            /*string json = "{ \"username\": \"admin\",  \"password\": \"admin\"\r\n}";
            request.AddJsonBody(json);*/

            request.AddJsonBody(new { username = "admin", password = "admin" });
            response = client.Execute(request);;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "The Login wasn't success");

            dynamic jsonObj = JsonConvert.DeserializeObject(response.Content.ToString());
            string token = (string)jsonObj.token;
            Console.WriteLine("Token: " + token);
            Assert.That(token, Is.Not.EqualTo(""), "No Token");
        }

        [Test]
        public void Test1_CreateProduct()
        {
            Login();
            client.AddDefaultHeader("Authorization", "Bearer " + codeAuthorization);
            RestRequest request = new RestRequest("/api/product", Method.Post);

            /*string json = "{ \"name\": \"ASDF\",  \"description\": \"ASDF\", \"image\": \"ASDF.jpg\", \"price\": \"15.99\", \"categoryId\": \"5\" }";
            request.AddJsonBody(json); */
            request.AddJsonBody(new { name = "ASDF", description = "ASDF", image = "ASDF.jpg", price = "15.99", categoryId = "7" });
            response = client.Execute(request);

            Console.WriteLine("Request: " + response.Content.ToString());
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "The product wasn't create");
            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "The product wasn't create");
        }
        
        [Test]
        public void Test2_GetAllProducts()
        {
            RestRequest request = new RestRequest("/api/product", Method.Get);
            response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            JArray jsonArray = JArray.Parse(response.Content.ToString());
            List<Product> listProducts = JsonConvert.DeserializeObject<List<Product>>(jsonArray.ToString());
            /*Console.WriteLine("List elements: " + listProducts.Count);
            Console.WriteLine("First ID: " + listProducts[0].id);*/
            Assert.That(jsonArray.Count, Is.AtLeast(0), "The list of product is empty"); 
        }

        [Test]
        public void Test3_GetProductsWithCategory()
        {
            bool result = true;
            RestRequest request = new RestRequest("/api/product?category={idcategory}", Method.Get);
            request.AddUrlSegment("idcategory", idCategory);
            response = client.Execute(request);

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

        
        [Test]
        public void Test4_GetAllCategories()
        {
            RestRequest request = new RestRequest("/api/category", Method.Get);
            response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            JArray jsonArray = JArray.Parse(response.Content.ToString());
            List<Category> listProducts = JsonConvert.DeserializeObject<List<Category>>(jsonArray.ToString());
            Assert.That(jsonArray.Count, Is.AtLeast(1), "The list of categories is empty");
        }

        /*[Test]
        public void Test4_CreateCategory()
        {
            Assert.Pass();
        }*/
    }
}