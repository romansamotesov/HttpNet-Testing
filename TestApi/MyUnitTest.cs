using REST_Testing;
using System.Net;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;
using Allure.Net.Commons;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Rest_Testing;
using System.Net.Http.Headers;

namespace TestApi
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("ApiTests")]
    public class MyUnitTests
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [OneTimeSetUp]
        public void Setup()
        {
            Logger.Info("Test execution started");
        }

        [Test]
        [AllureTag("ZipCodes")]
        [AllureOwner("Roma")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureSubSuite("Get")]
        [AllureIssue("Issue Status Code is not 200")]
        public async Task GetZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "23456",
                "ABCDE",
            };

            AllureApi.Step("Send Request");
            var client = await MyReadHttpClient.getInstance();
            HttpResponseMessage response = await client.GetAsync("http://localhost:49000/zip-codes");
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code is not 200");
                var receivedZipcodes = ZipCode.ZipCodesToList(responseContent);
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "list of zipCodes is not equal");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("ZipCodes")]
        [AllureOwner("Roma")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureSubSuite("Post")]
        public async Task PostZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "23456",
                "ABCDE",
                "12333",
                "12344"
            };

            AllureApi.Step("Send Request");
            var client = await MyWriteHttpClient.getInstance();
            var zipcodes = new List<string>() { "12333", "12344" };
            var json = JsonConvert.SerializeObject(zipcodes);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(json), "json");
            var response = await client.PostAsync("http://localhost:49000/zip-codes/expand", data);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
                var receivedZipcodes = ZipCode.ZipCodesToList(responseContent);
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("ZipCodes")]
        [AllureOwner("Roma")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureSubSuite("Post")]
        [AllureIssue("Issue Duplicate ZipCode is created")]
        public async Task PostDuplicateZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "ABCDE",
                "12333",
                "12344",
                "12355"
            };

            AllureApi.Step("Send Request");
            var client = await MyWriteHttpClient.getInstance();
            var zipcodes = new List<string>() { "12355", "12355" };
            var json = JsonConvert.SerializeObject(zipcodes);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(json), "json");
            var response = await client.PostAsync("http://localhost:49000/zip-codes/expand", data);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
                var receivedZipcodes = ZipCode.ZipCodesToList(responseContent);
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("ZipCodes")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureIssue("Issue Already used ZipCode is created")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task PostAlreadyUsedZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "23456",
                "ABCDE",
                "12366"
            };

            AllureApi.Step("Send Request");
            var client = await MyWriteHttpClient.getInstance();
            var zipcodes = new List<string>() { "12366", "12345" };
            var json = JsonConvert.SerializeObject(zipcodes);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(json), "json");
            var response = await client.PostAsync("http://localhost:49000/zip-codes/expand", data);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
                var receivedZipcodes = ZipCode.ZipCodesToList(responseContent);
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");//bug
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task PostCreateUser_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "ABCDE"
            };

            AllureApi.Step("Send Request");
            var writeClient = await MyWriteHttpClient.getInstance();
            var user = new User(20, "TestName", Enums.Sex.FEMALE, "23456");
            JsonContent jsonContent = JsonContent.Create(user);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(user)), "json");
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user exist");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            var createdUsers = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(createdUsers.Any(u => u.Name.Equals(user.Name)), "User is not created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that used zip code is removed from the list");
            response = await readClient.GetAsync("http://localhost:49000/zip-codes");
            responseContent = response.Content.ReadAsStringAsync().Result;
            var receivedZipcodes = ZipCode.ZipCodesToList(responseContent);
            try
            {
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "list of zipCodes is not equal");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task PostRequiredFieldsCreateUser_Test() 
        {
            var writeClient = await MyWriteHttpClient.getInstance();
            var user = new User("TestNameRF", Enums.Sex.FEMALE);
            var jsonContent = JsonContent.Create(user);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(user)), "json");
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user exist");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            var createdUsers = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(createdUsers.Any(u => u.Name.Equals(user.Name)), "User is not created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }


        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task PostNotAvailableZipCodeCreateUser_UserIsNotCreated_Test()
        {
            AllureApi.Step("Send Request");
            var writeClient = await MyWriteHttpClient.getInstance();
            var user = new User(20, "TestNameZC", Enums.Sex.FEMALE, "111");
            var jsonContent = JsonContent.Create(user);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(user)), "json");
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status Code is not 424");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is not exist");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            var createdUsers = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(createdUsers.Any(u => u.Name.Equals(user.Name)), Is.False, "User is created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureIssue("Issue Status Code is not 400, but 424")]
        public async Task PostDuplicateCreateUser_UserIsNotCreated_Test()
        {
            AllureApi.Step("Send request");
            var writeClient = await MyWriteHttpClient.getInstance();
            var user = new User(20, "TestName", Enums.Sex.FEMALE, "23456");
            var jsonContent = JsonContent.Create(user);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(user)), "json");
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Status Code is not 400");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is not exist");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            var createdUsers = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(createdUsers.Any(u => u.Name.Equals(user.Name)), Is.False, "User is created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Get")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetUsers_Test()
        {
            var users = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            AllureApi.Step("Create users");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(users[0]);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(users[1]);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(users[2]);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);

            AllureApi.Step("Check that users are exist");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            var responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var createdUsers = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(createdUsers.All(x => users.Any(y => y.Name == x.Name)), "Users are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Get")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task GetUsersByOlderAge_Test()
        {
            var users = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };
            var expectedUsers = new List<User>()
            {
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            AllureApi.Step("Create users");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(users[0]);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(users[1]);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(users[2]);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);

            AllureApi.Step("Send request");
            var readClient = await MyReadHttpClient.getInstance();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:49000/users");
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent("29"), "olderThan");
            request.Content = multipartContent;
            response = await readClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var createdUsers = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);

            AllureApi.Step("Check response");
            try
            {
                Assert.That(expectedUsers.All(x => createdUsers.Any(y => y.Name == x.Name)), "Users filtered by age are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Get")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task GetUsersByYoungerAge_Test()
        {
            var users = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            var expectedUsers = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
            };

            AllureApi.Step("Create users");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(users[0]);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(users[1]);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(users[2]);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);

            AllureApi.Step("Send request");
            var readClient = await MyReadHttpClient.getInstance();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:49000/users");
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent("39"), "youngerThan");
            request.Content = multipartContent;
            response = await readClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var createdUsers = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);

            AllureApi.Step("Check response");
            try
            {
                Assert.That(expectedUsers.All(x => createdUsers.Any(y => y.Name == x.Name)), "Users filtered by age are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Get")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task GetUsersBySex_Test()
        {
            var users = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            var expectedUsers = new List<User>()
            {
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            AllureApi.Step("Create users");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(users[0]);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(users[1]);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(users[2]);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);

            AllureApi.Step("Send request");
            var readClient = await MyReadHttpClient.getInstance();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:49000/users");
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent("FEMALE"), "sex");
            request.Content = multipartContent;
            response = await readClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var createdUsers = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);

            AllureApi.Step("Check response");
            try
            {
                Assert.That(expectedUsers.All(x => createdUsers.Any(y => y.Name == x.Name)), "Users filtered by age are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Put")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task UpdateUser_Test()
        {
            var userToChange = new User(30, "TestName30", Enums.Sex.FEMALE, "12345");
            var userNewValues = new User(30, "TestName31", Enums.Sex.FEMALE, "12345");
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };

            AllureApi.Step("Create user");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(userToChange);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);

            AllureApi.Step("Send request");
            jsonContent = JsonContent.Create(updateUserBody);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(updateUserBody)), "json");
            response = await writeClient.PutAsync("http://localhost:49000/users", jsonContent);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code is not 200");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is updated");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var user = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(user.Any(u => u.Name.Equals(userNewValues.Name)), "User is not updated");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Put")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task UpdateUserWithIndalidZipCode_UserIsNotUpdated_Test()
        {
            var userToChange = new User(30, "TestName30", Enums.Sex.FEMALE, "12345");
            var userNewValues = new User(30, "TestName31", Enums.Sex.FEMALE, "123");
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };

            AllureApi.Step("Create user");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(userToChange);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);

            AllureApi.Step("Send request");
            jsonContent = JsonContent.Create(updateUserBody);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(updateUserBody)), "json");
            response = await writeClient.PutAsync("http://localhost:49000/users", jsonContent);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status Code is not 424");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is not updated");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var user = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(user.Any(u => u.Name.Equals(userNewValues.Name)), Is.False, "User is updated");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Put")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureIssue("Issue Status Code is not 409")]
        public async Task UpdateUserWithNotFilledReqiuredFields_UserIsNotUpdated_Test()
        {
            var userToChange = new User(30, "TestName31", Enums.Sex.FEMALE, "12333");
            var userNewValues = new User("TestName33", Enums.Sex.FEMALE);
            userNewValues.Name = null;
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };

            AllureApi.Step("Create user");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(userToChange);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);

            AllureApi.Step("Send request");
            jsonContent = JsonContent.Create(updateUserBody);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(updateUserBody)), "json");
            response = await writeClient.PutAsync("http://localhost:49000/users", jsonContent);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict), "Status Code is not 409");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is not updated");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var user = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(user.Any(u => u.Name.Equals(userNewValues.Name)), Is.False, "User is updated");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Delete")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task DeleteUser_Test()
        {
            var userToDelete = new User(20, "TestName20", Enums.Sex.MALE, "12345");
            var userNotDeleted = new User(30, "TestName30", Enums.Sex.MALE, "23456");
            var expectedZipCode = "12345";

            AllureApi.Step("Create users");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(userToDelete);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(userNotDeleted);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);

            AllureApi.Step("Delete user");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:49000/users");
            request.Content = JsonContent.Create(userToDelete);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(userToDelete)), "json");
            response = await writeClient.SendAsync(request); 
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Status code is not 204");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is deleted");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(users.Any(u => u.Name.Equals(userToDelete.Name)), Is.False, "User is not deleted");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that zipcode is available");
            readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/zip-codes");
            responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");
            var receivedZipcodes = ZipCode.ZipCodesToList(responseContent);
            try
            {
                Assert.That(receivedZipcodes.Any(z => z.Equals(expectedZipCode)), Is.True, "Zipcode not exist");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Delete")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task DeleteUserWithRequiredFields_UserIsDeleted_Test()
        {

            var userToDelete = new User("TestName40", Enums.Sex.FEMALE);
            var userNotDeleted = new User(30, "TestName30", Enums.Sex.MALE, "23456");

            AllureApi.Step("Create users");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(userToDelete);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(userNotDeleted);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);

            AllureApi.Step("Delete user");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:49000/users");
            request.Content = JsonContent.Create(userToDelete);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(userToDelete)), "json");
            response = await writeClient.SendAsync(request);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Status code is not 204");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is deleted");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(users.Any(u => u.Name.Equals(userToDelete.Name)), Is.False, "User is not deleted");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Delete")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task DeleteUserWithEmptyRequiredField_UserIsNotDeleted_Test()
        {
            var userToDelete = new User(20, "TestName40", Enums.Sex.FEMALE, "ABCDE");
            var userNotDeleted = new User(30, "TestName30", Enums.Sex.MALE, "23456");

            AllureApi.Step("Create users");
            var writeClient = await MyWriteHttpClient.getInstance();
            var jsonContent = JsonContent.Create(userToDelete);
            var response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            jsonContent = JsonContent.Create(userNotDeleted);
            response = await writeClient.PostAsync("http://localhost:49000/users", jsonContent);
            userToDelete.Name = null;

            AllureApi.Step("Delete user");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:49000/users");
            request.Content = JsonContent.Create(userToDelete);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(userToDelete)), "json");
            response = await writeClient.SendAsync(request);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict), "Status code is not 209");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is deleted");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(users.Any(u => u.Name.Equals("TestName40")), Is.True, "User is deleted");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task UploadUsers_Test() 
        {
            var expectedUsers = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };
            var filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\\MyJson.json";

            AllureApi.Step("Send request");
            var writeClient = await MyWriteHttpClient.getInstance();
            var formattedJson = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            formattedJson.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            var content = new MultipartFormDataContent
            {
                { formattedJson, "file", Path.GetFileName(filePath) }
            };
            AllureApi.AddAttachment("request", "application/json", filePath);
            var response = await writeClient.PostAsync("http://localhost:49000/users/upload", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that users are created");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(users.All(x => expectedUsers.Any(y => y.Name == x.Name)), "Users are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureIssue("Issue Status code is not 424, but 500")]
        public async Task UploadUsersWithIncorrectZipCodes_UsersAreNotUploadedTest()
        {
            var incorrectUsers = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };
            var filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\\MyIncorrectJson.json";

            AllureApi.Step("Send Request");
            var writeClient = await MyWriteHttpClient.getInstance();
            var formattedJson = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            formattedJson.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            var content = new MultipartFormDataContent
            {
                { formattedJson, "file", Path.GetFileName(filePath) }
            };
            AllureApi.AddAttachment("request", "application/json", filePath);
            var response = await writeClient.PostAsync("http://localhost:49000/users/upload", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status code is not 424");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that users are not uploaded");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(users.Any(x => incorrectUsers.Any(y => y.Name == x.Name)), Is.False, "Users are uploaded");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureIssue("Issue Status code is not 409, but was 500")]
        public async Task UploadUsersWithMissedRequiredFields_UsersAreNotUploadedTest()
        {
            var incorrectUsers = new List<User>()
            {
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };
            var filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\\MyMissedRequiredFieldsJson.json";

            AllureApi.Step("Send Request");
            var writeClient = await MyWriteHttpClient.getInstance();
            var formattedJson = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            formattedJson.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            var content = new MultipartFormDataContent
            {
                { formattedJson, "file", Path.GetFileName(filePath) }
            };
            AllureApi.AddAttachment("request", "application/json", filePath);
            var response = await writeClient.PostAsync("http://localhost:49000/users/upload", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict), "Status code is not 209");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that users are not uploaded");
            var readClient = await MyReadHttpClient.getInstance();
            response = await readClient.GetAsync("http://localhost:49000/users");
            responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info(responseContent, "Response");
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            try
            {
                Assert.That(users.Any(x => incorrectUsers.Any(y => y.Name == x.Name)), Is.False, "Users are uploaded");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Logger.Info("Test execution finished");
        }
    }
}