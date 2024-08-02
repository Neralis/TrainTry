using Microsoft.Playwright;
using System.Net;
using System.Text.Json;

namespace TestControllers
{
    public class AccountControllerTests
    {
        private IPlaywright _playwright;
        private IAPIRequestContext _request;

        [SetUp]
        public async Task SetUp()
        {
            _playwright = await Playwright.CreateAsync();

            _request = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://localhost:7149/",
                IgnoreHTTPSErrors = true
            });
        }

        
        #region [Тест регистрации]

        [Test]
        public async Task RegisterTest()
        {

            var registerData = new Dictionary<string, object>  // Обратите внимание на тип значений object
            {
                { "login", "TestUser1" },
                { "password", "9001" }
            };

            var response = await _request.PostAsync(url: "Account/register", new APIRequestContextOptions()
            {
                DataObject = registerData
            });

            Assert.AreEqual(200, response.Status, "Регистрация не удалась. Код ответа: " + response.Status);

            var responseBody = await response.TextAsync();
        }

        #endregion

        #region [Тест получения токена при входе]

        [Test]
        public async Task LoginTest()
        {
            var loginData = new Dictionary<string, object>  // Обратите внимание на тип значений object
            {
                { "login", "w" },
                { "password", "w" }
            };

            var response = await _request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                DataObject = loginData
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

        }
        [Test]
        public async Task BadLoginTest()
        {
            var loginData = new Dictionary<string, object>  // Обратите внимание на тип значений object
            {
                { "login", "sd" },
                { "password", "sd" }
            };

            var response = await _request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                DataObject = loginData
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
                Assert.Fail("Неверные данные.");
            }

        }

        #endregion

        #region [Тест выдачи роли пользователю]

        [Test]
        public async Task setRoleTest()
        {
            var token = await GetToken();

            var roleData = new Dictionary<string, object>
            {
                { "login", "we" },
                { "role", "reader" }
            };

            var response = await _request.PostAsync(url: "Account/setRole", new APIRequestContextOptions()
            {
                DataObject =  roleData,
                Headers = new Dictionary<string, string>
                {
                    {"Authorization", $"Bearer {token}" }
                }
            });

            Assert.AreEqual(200, response.Status, "Выдача роли не удалась. Код ответа: " + response.Status);

        }

        #endregion

        #region [Тест получения списка пользователей]

        [Test]
        public async Task GetUsers()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "Account/GetUsers", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    {"Authorization", $"Bearer {token}" }
                }
            });

            var data = await response.JsonAsync();
            Console.WriteLine(data);

            Assert.AreEqual(200, response.Status, "Получение списка пользователей не удалось. Код ответа: " + response.Status);
        }

        #endregion

        #region [Тест удаления пользователей по ID]

        [Test]
        public async Task DeleteUser()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "Account/DeleteUser", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "id", "16" } // <-------- ID пользователя для теста удаления
                },
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                }
            });

            var data = response.JsonAsync();

            Assert.AreEqual(204, response.Status);
        }
        [Test]
        public async Task DeleteUser1()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "Account/DeleteUser", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "id", "-1" } // <-------- ID пользователя для теста удаления
                },
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                }
            });

            var data = response.JsonAsync();

            Assert.AreEqual(404, response.Status);
        }
        [Test]
        public async Task DeleteUser2()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "Account/DeleteUser", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "id", "0" } // <-------- ID пользователя для теста удаления
                },
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                }
            });

            var data = response.JsonAsync();

            Assert.AreEqual(404, response.Status);
        }
        [Test]
        public async Task DeleteUser3()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "Account/DeleteUser", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "id", "99999999999999999999999999999999999" } // <-------- ID пользователя для теста удаления
                },
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                }
            });

            var data = response.JsonAsync();

            Assert.AreEqual(400, response.Status);
        }


        #endregion

        #region [Таск получения токена для тестов]

        private async Task<string?> GetToken()
        {
            var loginData = new Dictionary<string, object>  // Обратите внимание на тип значений object
            {
                { "login", "s" },
                { "password", "s" }
            };

            var response = await _request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                DataObject = loginData
            });

            var responseBody = await response.TextAsync();

            // Десериализация JSON тела ответа в объект (предполагаем, что токен находится в поле 'token')
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            
            var token = responseObject["token"].ToString();

            return token;
        }

        #endregion


    }

    public class NewsControllerTests
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


        #region [Тест получения новостей]

        [Test]
        public async Task GetNewsTest()
        {
            var response = await _request.GetAsync(url: "News/GetNews");

            var data = await response.JsonAsync();
            Console.WriteLine(data);
        }

        #endregion

            #region [Тест получения новостей за диапазон даты]

        [Test]
        public async Task GetNewsByDateTest()
        {
            var dates = new Dictionary<string, object>
            {
                { "startDate", "12/12/2005" },
                { "endDate", "12/12/2024" }
            };

            var response = await _request.GetAsync(url: "News/GetNewsByDate", new APIRequestContextOptions()
            {
                Params = dates
            });

            var data = await response.JsonAsync();
            Console.WriteLine(data);

            Assert.AreEqual(200, response.Status);
        }

        #endregion

        #region [Получение новости за определенную дату]

        [Test]
        public async Task GetNewsBySingleDateTest()
        {
            var date = new Dictionary<string, object>
            {
                { "date", "12/12/2005" }
            };

            var response = await _request.GetAsync(url: "News/GetNewsBySingleDate", new APIRequestContextOptions()
            {
                Params = date
            });

            var data = await response.JsonAsync();
            Console.WriteLine(data);

            Assert.AreEqual(200, response.Status);
        }

        #endregion

        #region [Тест размещения новости]

        [Test]
        public async Task PutNewsTest()
        {
            var token = await GetToken();

            var credentials = new Dictionary<string, object>
            {
                    { "dateBegin", "09/09/2009" },
                    { "dateEnd", "09/09/2009" },
                    { "topic", "Something" },
                    { "article", "dasdasdasdas" },
                    { "importance", 3 },
                    { "author", "TestUser" }
            };

            var response = await _request.PutAsync(url:"News/PutNews", new APIRequestContextOptions()
            {

                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },

                Params = credentials

            });
            
            var data = await response.JsonAsync();
            Console.WriteLine(data);

        }

        #endregion

        #region [Тест удаления новостей по ID]

        [Test]
        public async Task DeleteNewsTest()
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
                    { "id", "14" }
                }
            });

            Assert.AreEqual(204, response.Status);
        }

        #endregion

        #region [Таск получения токена для тестов]

        private async Task<string?> GetToken()
        {
            var loginData = new Dictionary<string, object>  // Обратите внимание на тип значений object
            {
                { "login", "s" },
                { "password", "s" }
            };

            var response = await _request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                Params = loginData
            });

            var responseBody = await response.TextAsync();

            // Десериализация JSON тела ответа в объект (предполагаем, что токен находится в поле 'token')
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            var token = responseObject["token"].ToString();

            return token;
        }

        #endregion


    }

    public class MemorableDatesTests
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

        #region [Тест создания памятной даты]

        [Test]
        public async Task PutMemorableDateTest()
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
                    { "eventDate", "01/01/2024" },
                    { "notificationText", "Happy New YEAR!!!!" },
                    { "author", "Father Frost" }
                }
            });

            var data = await response.JsonAsync();
            Console.WriteLine(data);

            Assert.AreEqual(200, response.Status);
        }

        #endregion

        #region [Тест выборки памятных дат]

        [Test]
        public async Task GetMemorableDateTest()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "MemorableDates/GetMemorablesDates", new APIRequestContextOptions()
            {
                DataObject = new Dictionary<string, object>
                {
                    { "date", "25/07/2024" }
                }
            });

            var data = await response.JsonAsync();
            Console.WriteLine(data);

            Assert.AreEqual(200, response.Status);
        }

        #endregion

        #region [Тест удалению памятных дат]

        [Test]
        public async Task DeleteMemorableDateTest()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "MemorableDates/DeleteMemorablesDates", new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                DataObject   = new Dictionary<string, object>
                {
                    { "id", "5" }
                }
            });

            Assert.AreEqual(204, response.Status);
        }
        [Test]
        public async Task DeleteMemorableDateTest1()
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

            Assert.AreEqual(404, response.Status);
        }
        [Test]
        public async Task DeleteMemorableDateTest2()
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

            Assert.AreEqual(404, response.Status);
        }

        [Test]
        public async Task DeleteMemorableDateTest3()
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
                    { "id", "999999999999999999999999999999" }
                }
            });

            Assert.AreEqual(404, response.Status);
        }

        #endregion

        #region [Таск получения токена для тестов]

        private async Task<string?> GetToken()
        {
            var loginData = new Dictionary<string, object>  // Обратите внимание на тип значений object
            {
                { "login", "s" },
                { "password", "s" }
            };

            var response = await _request.PostAsync(url: "Account/login", new APIRequestContextOptions()
            {
                DataObject = loginData
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