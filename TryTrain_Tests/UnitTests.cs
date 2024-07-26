using Microsoft.Playwright;
using System.Text.Json;

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

        // Тестирование памятных дат

        // Тестирование методов удаления памятных дат

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

        // Тестирование добавления даты

        #region [Тест добавления даты с неправльным типом данных]

        [Test]
        public async Task PutMemorableDates_WrongValueType()
        {
            var token = await GetToken();

            var response = await _request.PutAsync(url: "MemorableDates/PutMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "eventDate", "1111" },
                    { "notificationText", "Test" },
                    { "author", "TestUser" }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(400, response.Status);
        }

        #endregion

        #region [Тест добавления корректной даты]

        [Test]
        public async Task PutMemorableDates_ValidDate()
        {
            var token = await GetToken();

            var response = await _request.PutAsync(url: "MemorableDates/PutMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "eventDate", DateTime.UtcNow.ToString("yyyy-MM-dd") },
                    { "notificationText", "Test Notification" },
                    { "author", "TestUser" }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(200, response.Status);
        }

        #endregion

        #region [Тест добавления даты без авторизации]

        [Test]
        public async Task PutMemorableDates_Unauthorized()
        {
            var response = await _request.PutAsync(url: "MemorableDates/PutMemorablesDates", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "eventDate", DateTime.UtcNow.ToString("yyyy-MM-dd") },
                    { "notificationText", "Test Notification" },
                    { "author", "TestUser" }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(401, response.Status);
        }

        #endregion

        #region [Тест добавления даты с отсутствующими полями]

        [Test]
        public async Task PutMemorableDates_MissingFields()
        {
            var token = await GetToken();

            var response = await _request.PutAsync(url: "MemorableDates/PutMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "eventDate", DateTime.UtcNow.ToString("yyyy-MM-dd") }
                    // Автор и название отсутствуют
                }
            }); 

            Console.WriteLine(response.Status);
            Assert.AreEqual(400, response.Status);
        }

        #endregion

        #region [Тест добавления даты с null полями]

        [Test]
        public async Task PutMemorableDates_NullFields()
        {
            var token = await GetToken();

            var response = await _request.PutAsync(url: "MemorableDates/PutMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                DataObject = new Dictionary<string, object>
                {
                    { "eventDate", null },
                    { "notificationText", null },
                    { "author", null }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(400, response.Status);
        }

        #endregion

        // Тестирование получения даты

        #region [Тест получения даты]

        [Test]
        public async Task GetMemorableDates_Success()
        {
            var response = await _request.GetAsync(url: "MemorableDates/GetMemorablesDates", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "date", "01/01/2024" }
                }
            });

            var data = await response.JsonAsync();

            Console.WriteLine(data);
            Console.WriteLine(response.Status);
            Assert.AreEqual(200, response.Status);
        }

        #endregion

        #region [Тест получения даты с неправильным форматом даты]

        [Test]
        public async Task GetMemorableDates_InvalidDateFormat()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "MemorableDates/GetMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "date", "invalid-date" }
                }
            });

            Console.WriteLine(response.Status);
            Assert.AreEqual(400, response.Status);
        }

        #endregion

        #region [Тест получения даты с null параметром даты]

        [Test]
        public async Task GetMemorableDates_NullDate()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "MemorableDates/GetMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {token}" }
        }
                // No date parameter
            });

            var data = await response.JsonAsync();

            Console.WriteLine(response.Status);
            Console.WriteLine(data);

            Assert.AreEqual(200, response.Status);
            Assert.IsTrue(data.ToString() == "[]");
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
