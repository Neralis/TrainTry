using Microsoft.Playwright;
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


        #region [���� �����������]

        [Test]
        public async Task RegisterTest()
        {

            var response = await _request.PostAsync(url: "Account/register", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "NewUser",
                    password = "qwerty"
                }
            });

            Assert.AreEqual(200, response.Status, "����������� �� �������. ��� ������: " + response.Status);

            var responseBody = await response.TextAsync();
        }

        #endregion

        #region [���� ��������� ������ ��� �����]

        [Test]
        public async Task LoginTest()
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

            // �������������� JSON ���� ������ � ������ (������������, ��� ����� ��������� � ���� 'token')
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            if (responseObject != null && responseObject.ContainsKey("token"))
            {
                var token = responseObject["token"].ToString();
                // ����������� 'token' ��� ���������� �������� ��� ��������
                Console.WriteLine($"���������� �����: {token}");
            }
            else
            {
                Assert.Fail("�� ������� ������� ����� �� ������.");
            }

        }

        #endregion

        #region [���� ������ ���� ������������]

        [Test]
        public async Task setRoleTest()
        {
            var token = await GetToken();

            var response = await _request.PostAsync(url: "Account/setRole", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    login = "NewUser",
                    accessRole = "NewUser"
                },
                Headers = new Dictionary<string, string>
                {
                    {"Authorization", $"Bearer {token}" }
                }
            });

            Assert.AreEqual(200, response.Status, "������ ���� �� �������. ��� ������: " + response.Status);

        }

        #endregion

        #region [���� ��������� ������ �������������]

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

            Assert.AreEqual(200, response.Status, "��������� ������ ������������� �� �������. ��� ������: " + response.Status);
        }

        #endregion

        #region [���� �������� ������������� �� ID]

        [Test]
        public async Task DeleteUser()
        {
            var token = await GetToken();

            var response = await _request.DeleteAsync(url: "Account/DeleteUser", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "id", "21" } // <-------- ID ������������ ��� ����� ��������
                },
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                }
            });

            var data = response.JsonAsync();

            Assert.AreEqual(204, response.Status);
        }

        #endregion

        #region [���� ��������� ������ ��� ������]

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

            // �������������� JSON ���� ������ � ������ (������������, ��� ����� ��������� � ���� 'token')
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


        #region [���� ��������� ��������]

        [Test]
        public async Task GetNewsTest()
        {
            var response = await _request.GetAsync(url: "News/GetNews");

            var data = await response.JsonAsync();
            Console.WriteLine(data);
        }

        #endregion

        #region [���� ��������� �������� �� �������� ����]

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

        #region [��������� ������� �� ������������ ����]

        [Test]
        public async Task GetNewsBySingleDateTest()
        {
            var date = new Dictionary<string, object>
            {
                { "date", "09/09/2009" }
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

        #region [���� ���������� �������]

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

            var response = await _request.PutAsync(url: "News/PutNews", new APIRequestContextOptions()
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

        #region [���� �������� �������� �� ID]

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
                    { "id", "3" }
                }
            });

            Assert.AreEqual(204, response.Status);
        }

        #endregion

        #region [���� ��������� ������ ��� ������]

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

            // �������������� JSON ���� ������ � ������ (������������, ��� ����� ��������� � ���� 'token')
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

        #region [���� �������� �������� ����]

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
                Params = new Dictionary<string, object>
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

        #region [���� ������� �������� ���]

        [Test]
        public async Task GetMemorableDateTest()
        {
            var token = await GetToken();

            var response = await _request.GetAsync(url: "MemorableDates/GetMemorablesDates", new APIRequestContextOptions()
            {
                Params = new Dictionary<string, object>
                {
                    { "date", "01/01/2024" }
                }
            });

            var data = await response.JsonAsync();
            Console.WriteLine(data);

            Assert.AreEqual(200, response.Status);
        }

        #endregion

        #region [���� �������� �������� ���]

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
                Params = new Dictionary<string, object>
                {
                    { "id", "4" }
                }
            });

            Assert.AreEqual(204, response.Status);
        }

        #endregion

        #region [���� ��������� ������ ��� ������]

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

            // �������������� JSON ���� ������ � ������ (������������, ��� ����� ��������� � ���� 'token')
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            var token = responseObject["token"].ToString();

            return token;
        }

        #endregion

    }
}