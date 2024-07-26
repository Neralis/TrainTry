using Microsoft.Playwright;
using System.Text.Json;

namespace UnitTests
{
    public class UnitTestsMemorableDates
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

    public class UnitTestsAccountController
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

        // Тестирование контроллера аккаунтов

        // Тестирование регистрации

        #region [Тест успешной регистрации]

        [Test]
        public async Task AccountControllerTest_RegisterSuccess()
        {
            var response = await _request.PostAsync(url: "Account/register", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "TestUser",
                    password = "9001"
                }
            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест регистрации c символами юникода в полях]

        [Test]
        public async Task AccountControllerTest_RegisterUnicode()
        {
            var response = await _request.PostAsync(url: "Account/register", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "☁",
                    password = "☁"
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест регистрации с отсутствующими полями]

        [Test]
        public async Task AccountControllerTest_RegisterMissingFields()
        {
            var response = await _request.PostAsync(url: "Account/register", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "NewUser"
                    // Пароль отсутствует
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест регистрации с null значениями]

        [Test]
        public async Task AccountControllerTest_RegisterNullValues()
        {
            var response = await _request.PostAsync(url: "Account/register", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = (string?)null,
                    password = (string?)null
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        // Тестирование входа

        #region [Тест успешного входа]

        [Test]
        public async Task AccountControllerTest_LoginSuccess()
        {
            var response = await _request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "TestUser",
                    password = "9001"
                }
            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест входа с неверными данными]

        [Test]
        public async Task AccountControllerTest_LoginWrondCredentials()
        {
            var response = await _request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "TestUser",
                    password = "9002"
                }
            });

            Assert.AreEqual(401, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест входа с символами юникода в запросе]

        [Test]
        public async Task AccountControllerTest_LoginUnicode()
        {
            var response = await _request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "☁",
                    password = "☁"
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        // Тестирование выдачи роли

        #region [Тест успешной выдачи роли]

        [Test]
        public async Task AccountControllerTest_SetRoleSuccess()
        {
            var token = await GetToken();

            var response = await _request.PostAsync(url: "Account/setRole", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                DataObject = new
                {
                    login = "TestUser",
                    accessRole = "TestRole"
                }
            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест выдачи роли без авторизации админа]

        [Test]
        public async Task AccountControllerTest_SetRoleUnauthorized()
        {
            var token = await GetToken();

            var response = await _request.PostAsync(url: "Account/setRole", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "TestUser",
                    accessRole = "TestRole"
                }
            });

            Assert.AreEqual(401, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        // Тестирование выборки списка пользователей

        #region [Тест удачной выборки списка пользователей]

        [Test]
        public async Task AccountControllerTest_GetUsersSuccess()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "Account/GetUsers", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                }
            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест выборки списка пользователей без авторизации админа]

        [Test]
        public async Task AccountControllerTest_GetUsersUnauthorized()
        {
            var response = await _request.GetAsync(url: "Account/GetUsers", new APIRequestContextOptions()
            {

            });

            Assert.AreEqual(401, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        // Тестирование удаления пользователей

        #region [Тест успешного удаления пользователя]

        [Test]
        public async Task AccountControllerTest_DeleteUsersSuccess()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "Account/DeleteUser", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "id", "6" }
                }
            });

            Assert.AreEqual(204, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест удаления несуществующего пользователя]

        [Test]
        public async Task AccountControllerTest_DeleteUsersWrongId()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "Account/DeleteUser", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "id", "999" }
                }
            });

            Assert.AreEqual(404, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест удаления с неправильным типом данных в id]

        [Test]
        public async Task AccountControllerTest_DeleteUsersWrongTypeId()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "Account/DeleteUser", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "id", "try" }
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест удаления с id меньше нуля]

        [Test]
        public async Task AccountControllerTest_DeleteUsersBelowZero()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "Account/DeleteUser", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "id", "-1" }
                }
            });

            Assert.AreEqual(404, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
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

    public class UnitTestsNewsController
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


        // Тестирование контроллера новостей

        // Тестирование получения новостей

        #region [Тест получения новостей]

        [Test]
        public async Task NewsControllerTest_GetNewsSuccess()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "News/GetNews", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                }
            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест получения новостей без авторизации]

        [Test]
        public async Task NewsControllerTest_GetNewsUnauthorized()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "News/GetNews", new APIRequestContextOptions()
            {

            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        // Тестирование добавления новостей

        #region [Тест добавления успешного добавления новостей]

        [Test]
        public async Task NewsControllerTest_PutNewsSuccess()
        {
            var token = await GetToken();

            var response = await _request.PutAsync(url: "News/PutNews", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "dateBegin", "09/09/2009" },
                    { "dateEnd", "09/09/2009" },
                    { "topic", "Something" },
                    { "article", "dasdasdasdas" },
                    { "importance", 3 },
                    { "author", "TestUser" }
                }
            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест добавления новостей без авторизации]

        [Test]
        public async Task NewsControllerTest_PutNewsUnauthorized()
        {

            var response = await _request.PutAsync(url: "News/PutNews", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "dateBegin", "09/09/2009" },
                    { "dateEnd", "09/09/2009" },
                    { "topic", "Something" },
                    { "article", "dasdasdasdas" },
                    { "importance", 3 },
                    { "author", "TestUser" }
                }
            });

            Assert.AreEqual(401, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест добавления успешного добавления новостей]

        [Test]
        public async Task NewsControllerTest_PutNewsWrongCredentials()
        {
            var token = await GetToken();

            var response = await _request.PutAsync(url: "News/PutNews", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "dateBegin", "09F09F2009" },
                    { "dateEnd", "09F09F2009" },
                    { "topic", "Something" },
                    { "article", "dasdasdasdas" },
                    { "importance", 3 },
                    { "author", "TestUser" }
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        // Тестирование получения новостей за одну определенную дату

        #region [Тест получения новостей за одну определенную дату]

        [Test]
        public async Task NewsControllerTest_GetNewsBySingleDateSuccess()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "News/GetNewsBySingleDate", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "date", "12/12/2024" }
                }
            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест получения новостей за одну определенную дату с неверным типом данных]

        [Test]
        public async Task NewsControllerTest_GetNewsBySingleDateWrongValueType()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "News/GetNewsBySingleDate", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "date", "ddd" }
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест получения новостей за одну определенную дату неавторизованным]

        [Test]
        public async Task NewsControllerTest_GetNewsBySingleDateUnauthorized()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "News/GetNewsBySingleDate", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "date", "12/12/2024" }
                }
            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        // Тестирование получения новостей за диапазон дат

        #region [Тест получения новостей за диапазон дат]

        [Test]
        public async Task NewsControllerTest_GetNewsByDateSuccess()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "News/GetNewsByDate", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "startDate", "12/12/2005" },
                    { "endDate", "12/12/2012" }
                }
            });

            Assert.AreEqual(200, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест получения новостей за диапазон дат при начальной дате позже конечной]

        [Test]
        public async Task NewsControllerTest_GetNewsByDateEndLaterBegining()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "News/GetNewsByDate", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "startDate", "12/12/2012" },
                    { "endDate", "12/12/2005" }
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест получения новостей за диапазон дат при неправильном типе данных]

        [Test]
        public async Task NewsControllerTest_GetNewsByDateWrongValueType()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "News/GetNewsByDate", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "startDate", "ddd" },
                    { "endDate", "ddd" }
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        // Тестирование удаления новостей

        #region [Тест успешного удаления новости]

        [Test]
        public async Task NewsControllerTest_DeleteNews()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "News/DeleteNews", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "id", "19" }
                }
            });

            Assert.AreEqual(204, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест удаления новости с id меньше нуля]

        [Test]
        public async Task NewsControllerTest_DeleteNewsBelowZero()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "News/DeleteNews", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "id", "-1" }
                }
            });

            Assert.AreEqual(404, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест удаления новости с неверным типом данных в id]

        [Test]
        public async Task NewsControllerTest_DeleteNewsWrongValueType()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "News/DeleteNews", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Params = new Dictionary<string, object>
                {
                    { "id", "ddd" }
                }
            });

            Assert.AreEqual(400, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
        }

        #endregion

        #region [Тест удаления новости с без авторизации]

        [Test]
        public async Task NewsControllerTest_DeleteNewsUnauthorized()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "News/DeleteNews", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "id", "18" }
                }
            });

            Assert.AreEqual(401, response.Status);

            var responseBody = await response.TextAsync();
            Console.WriteLine(responseBody);
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
