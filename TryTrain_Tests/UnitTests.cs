using Microsoft.Playwright;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnitTests
{
    internal class UnitTestsMemorableDates
    {
        private IPlaywright _playwright;
        private IAPIRequestContext _request;

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();

            _request = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = "https://localhost:7149/",
                IgnoreHTTPSErrors = true
            });
        }

        #region [Тест удаления нулевого id]

        [Test]
        public async Task DeleteMemorableDateTest_Zero()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "MemorableDates/DeleteMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                DataObject = new Dictionary<string, object>
                {
                    { "id", "0" }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(404, response.Status);
        }

        #endregion

        #region [Тест удаления id меньше нуля]

        [Test]
        public async Task DeleteMemorableDateTest_BelowZero()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "MemorableDates/DeleteMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                DataObject = new Dictionary<string, object>
                {
                    { "id", "-1" }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(404, response.Status);
        }

        #endregion

        #region [Тест удаления id с максимальным значением Int32]

        [Test]
        public async Task DeleteMemorableDateTest_MaxInt32()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "MemorableDates/DeleteMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                DataObject = new Dictionary<string, object>
                {
                    { "id", int.MaxValue.ToString() }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(404, response.Status);
        }

        #endregion

        #region [Тест удаления id с вводом неверного типа данных ]

        [Test]
        public async Task DeleteMemorableDateTest_WrongValueType()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "MemorableDates/DeleteMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                DataObject = new Dictionary<string, object>
                {
                    { "id", "s" }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(404, response.Status);
        }

        #endregion

        #region [Тест удаления несуществующего id]

        [Test]
        public async Task DeleteMemorableDateTest_NonExistentId()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "MemorableDates/DeleteMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                DataObject = new Dictionary<string, object>
                {
                    { "id", "123456" } // Assuming this ID doesn't exist
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(404, response.Status);
        }

        #endregion

        #region [Тест удаления id без авторизации]

        [Test]
        public async Task DeleteMemorableDateTest_Unauthorized()
        {
            var response = await _request.DeleteAsync(url: "MemorableDates/DeleteMemorablesDates", new APIRequestContextOptions()
            {
                DataObject = new Dictionary<string, object>
                {
                    { "id", "1" }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(401, response.Status);
        }

        #endregion

        #region [Тест удаление с использованием неправильного HTTP метода]

        [Test]
        public async Task DeleteMemorableDateTest_WrongHttpMethod()
        {
            var token = await GetToken();

            var response = await _request.PostAsync(url: "MemorableDates/DeleteMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                DataObject = new Dictionary<string, object>
                {
                    { "id", "7" }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(405, response.Status); // Assuming 405 Method Not Allowed
        }

        #endregion

        #region [Таск получения токена для тестов]

        private async Task<string?> GetToken()
        {

            var response = await _request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "Neralis",
                    password = "9001"
                }
            });

            var responseBody = await response.TextAsync();

            // Десериализация JSON тела ответа в объект (предполагаем, что токен находится в поле 'token')
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            var token = responseObject["token"].ToString();

            return token;
        }

        #endregion
    }
}
