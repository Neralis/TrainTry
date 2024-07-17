using Microsoft.Playwright;
using System.Net;
using System.Text.Json;

namespace TryTrain_Tests
{
    public class LoginTests
    {
        [SetUp]
        public async Task SetUp()
        {

        }

        [Test]
        public async Task LoginTest()
        {
            var playwright = await Playwright.CreateAsync();

            var request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://localhost:7149/",
                IgnoreHTTPSErrors = true
            });

            var loginData = new Dictionary<string, object>  // Обратите внимание на тип значений object
            {
                { "login", "Neralis" },
                { "password", "9001" }
            };

            var response = await request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                Params = loginData
            });

            var responseBody = await response.TextAsync();

            // Десериализация JSON тела ответа в объект (предполагаем, что токен находится в поле 'token')
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            if (responseObject != null && responseObject.ContainsKey("token"))
            {
                var token = responseObject["token"].ToString();
                // Используйте 'token' для дальнейших проверок или действий
                Console.WriteLine($"Полученный токен: {token}");
            }
            else
            {
                Assert.Fail("Не удалось извлечь токен из ответа.");
            }

            //Assert.AreEqual(HttpStatusCode.OK, HttpStatusCode.OK);
        }
    }
}