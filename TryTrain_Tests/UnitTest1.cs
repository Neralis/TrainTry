using Microsoft.Playwright;
using Microsoft.Win32;
using System.Security.Principal;
using System.Text;

namespace TryTrain_Tests
{
    public class AuthorizeTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task LoginTest()
        {
            var playwright = await Playwright.CreateAsync();
            var requestContext = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://localhost:7149/",
                IgnoreHTTPSErrors = true
            });

            var response = await requestContext.PostAsync(url:"Account/login", new APIRequestContextOptions()
            {
                DataObject = new
                {
                   login = "Neralis",
                   password = "9001"
                }
            });

            var jsonString = await response.JsonAsync();

        }
    }
}