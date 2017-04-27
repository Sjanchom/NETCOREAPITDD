using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HappyKids.Configurations;
using HappyKids.Helper;
using HappyKids.Models.DataTranferObjects;
using HappyKids.Models.Domain;
using HappyKids.Test.Helper;
using HappyKids.TestMock;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Xunit;

namespace HappyKids.Test.IntegrationTests
{
    public class StudentControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;
        private List<Student> _randomStudent;

        public StudentControllerTests(TestFixture<Startup> fixture)
        {
            MapperHelper.SetUpMapper();
            _client = fixture.Client;
            _randomStudent = DataInitializer.Instance.GetStudents();
        }


        [Fact]
        public async Task GetAllReturnCorrect()
        {
            var response = await _client.GetAsync("/api/students?page=1&pagesize=8&Name=N");

            response.EnsureSuccessStatusCode();
            var returnedSession = await response.Content.ReadAsJsonAsync<List<StudentDTO>>();
            var xPage = ((IList<string>)response.Headers.GetValues("X-Pagination"))[0];
            var page = JsonConvert.DeserializeObject<PaginationHeader>(xPage);
            Assert.NotNull(page);
            Assert.IsType<PaginationHeader>(page);
            Assert.Equal(8,page.PageSize);
            Assert.Equal(1,page.CurrentPage);
            Assert.Equal(8, returnedSession.Count);
            Assert.True(returnedSession.All(i => i.Name.ToUpperInvariant().Contains("N")));
        }

        [Fact]
        public async Task GetByIdReturnCorrect()
        {
            var response = await _client.GetAsync("/api/students/1");

            response.EnsureSuccessStatusCode();
            var returnSession = await response.Content.ReadAsJsonAsync<StudentDTO>();
            AssertObjects.PropertyValuesAreEquals
                (returnSession,AutoMapper.Mapper.Map<StudentDTO>(_randomStudent.Single(x => x.Id == "1")));
        }

        [Fact]
        public async Task GetByIdReturnNotFoundWhenIdNotCorrect()
        {
            var response = await _client.GetAsync("/api/students/1sdfsdf");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostReturnCorrectLinkWhenSuccess()
        {
            var student = new StudentForCreateDTO();
            student.Name = "Something Create New";
            student.BirthDate = "22/05/2014";

            var response = await _client.PostAsJsonAsync("/api/students", student);

            var last =_randomStudent.Last();
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(response.Headers.Location, new Uri($"http://localhost/api/students/{last.Id}"));
        }


        [Fact]
        public async Task PostReturnCorrectLinkWhenFailure()
        {
            var student = new StudentForCreateDTO();
            student.BirthDate = "22/05/2014";

            var response = await _client.PostAsJsonAsync("/api/students", student);

            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);

        }

        [Fact]
        public async Task DeleteReturnNoContentWhenSuccess()
        {
            var response = await _client.DeleteAsync("/api/students/1");

            Assert.True(_randomStudent.Single(x => x.Id == "1").IsActived == 0);
            Assert.Equal(HttpStatusCode.NoContent,response.StatusCode);
        }

        [Fact]
        public async Task DeleteReturnNotFoundWhenFail()
        {
            var response = await _client.DeleteAsync("/api/students/1sss");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task FullUpdateShouldReturnNoContentWhenSuccess()
        {
            var student = new StudentForUpdateDTO();
            student.Name = "Update Name";

            var response = await _client.PutAsJsonAsync("/api/students/5", student);

            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
            //var returnedSession = await response.Content.ReadAsJsonAsync<List<object>>();
            //Assert.True(_randomStudent.Single(x => x.Id == "5").Name.Equals("Update Name"));
            //Assert.Null(_randomStudent.Single(x => x.Id == "5").BirthDate);
        }

        [Fact]
        public async Task FullUpdateShouldReturnNotFoundWhenIdNotExist()
        {
            var student = new StudentForUpdateDTO();
            student.Name = "Update Name";
            student.BirthDate = "22/12/2016";
            var response = await _client.PutAsJsonAsync("/api/students/5ssss", student);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
          
        }

        [Fact]
        public async Task PartialUpdateShouldReturnNoContentWhenSuccess()
        {
            JsonPatchDocument<StudentDTO> patchDoc = new JsonPatchDocument<StudentDTO>();
            patchDoc.Replace(e => e.Name, "IntegratePartialUpdate");
            var response = await _client.PatchAsJsonAsync("/api/students/5", patchDoc);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.True(_randomStudent.Single(x => x.Id == "5").Name.Equals("IntegratePartialUpdate"));
            Assert.NotNull(_randomStudent.Single(x => x.Id == "5").BirthDate);
        }

        [Fact]
        public async Task PartialUpdateShouldReturnNotFoundWhenIdNotExist()
        {
            var student = new StudentForUpdateDTO();
            student.Name = "Update Name";
            student.BirthDate = "22/12/2016";
            var response = await _client.PatchAsJsonAsync("/api/students/5ssss", student);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }
    }


}



//namespace TestingControllersSample.Tests.IntegrationTests
//{
//    public class ApiIdeasControllerTests : IClassFixture<TestFixture<TestingControllersSample.Startup>>
//    {
//        internal class NewIdeaDto
//        {
//            public NewIdeaDto(string name, string description, int sessionId)
//            {
//                Name = name;
//                Description = description;
//                SessionId = sessionId;
//            }

//            public string Name { get; set; }
//            public string Description { get; set; }
//            public int SessionId { get; set; }
//        }

//        private readonly HttpClient _client;

//        public ApiIdeasControllerTests(TestFixture<TestingControllersSample.Startup> fixture)
//        {
//            _client = fixture.Client;
//        }

//        [Fact]
//        public async Task CreatePostReturnsBadRequestForMissingNameValue()
//        {
//            // Arrange
//            var newIdea = new NewIdeaDto("", "Description", 1);

//            // Act
//            var response = await _client.PostAsJsonAsync("/api/ideas/create", newIdea);

//            // Assert
//            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//        }

//        [Fact]
//        public async Task CreatePostReturnsBadRequestForMissingDescriptionValue()
//        {
//            // Arrange
//            var newIdea = new NewIdeaDto("Name", "", 1);

//            // Act
//            var response = await _client.PostAsJsonAsync("/api/ideas/create", newIdea);

//            // Assert
//            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//        }

//        [Fact]
//        public async Task CreatePostReturnsBadRequestForSessionIdValueTooSmall()
//        {
//            // Arrange
//            var newIdea = new NewIdeaDto("Name", "Description", 0);

//            // Act
//            var response = await _client.PostAsJsonAsync("/api/ideas/create", newIdea);

//            // Assert
//            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//        }

//        [Fact]
//        public async Task CreatePostReturnsBadRequestForSessionIdValueTooLarge()
//        {
//            // Arrange
//            var newIdea = new NewIdeaDto("Name", "Description", 1000001);

//            // Act
//            var response = await _client.PostAsJsonAsync("/api/ideas/create", newIdea);

//            // Assert
//            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//        }

//        [Fact]
//        public async Task CreatePostReturnsNotFoundForInvalidSession()
//        {
//            // Arrange
//            var newIdea = new NewIdeaDto("Name", "Description", 123);

//            // Act
//            var response = await _client.PostAsJsonAsync("/api/ideas/create", newIdea);

//            // Assert
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task CreatePostReturnsCreatedIdeaWithCorrectInputs()
//        {
//            // Arrange
//            var testIdeaName = Guid.NewGuid().ToString();
//            var newIdea = new NewIdeaDto(testIdeaName, "Description", 1);

//            // Act
//            var response = await _client.PostAsJsonAsync("/api/ideas/create", newIdea);

//            // Assert
//            response.EnsureSuccessStatusCode();
//            var returnedSession = await response.Content.ReadAsJsonAsync<BrainstormSession>();
//            Assert.Equal(2, returnedSession.Ideas.Count);
//            Assert.True(returnedSession.Ideas.Any(i => i.Name == testIdeaName));
//        }

//        [Fact]
//        public async Task ForSessionReturnsNotFoundForBadSessionId()
//        {
//            // Arrange & Act
//            var response = await _client.GetAsync("/api/ideas/forsession/500");

//            // Assert
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task ForSessionReturnsIdeasForValidSessionId()
//        {
//            // Arrange
//            var testSession = Startup.GetTestSession();

//            // Act
//            var response = await _client.GetAsync("/api/ideas/forsession/1");

//            // Assert
//            response.EnsureSuccessStatusCode();
//            var ideaList = JsonConvert.DeserializeObject<List<IdeaDTO>>(
//                await response.Content.ReadAsStringAsync());
//            var firstIdea = ideaList.First();
//            Assert.Equal(testSession.Ideas.First().Name, firstIdea.Name);
//        }
//    }
//}
