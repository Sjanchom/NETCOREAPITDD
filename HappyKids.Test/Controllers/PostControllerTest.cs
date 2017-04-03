using System;
using System.Collections.Generic;
using HappyKids.Test.Helper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using Moq;
using Xunit;

namespace HappyKids.Test.Controllers
{
    public class PostControllerTest
    {
        private List<Student> _randomStudent;
        IUnitOfWork _unitOfWork;


        public PostControllerTest()
        {
            _randomStudent = SetupStudents();
            _unitOfWork = SetUpUnitOfWork();

        }

        private IUnitOfWork SetUpUnitOfWork()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(x => x.StudentRepository)
                .Returns(SetUpStudentRepository());

            return unitOfWork.Object;
        }

        private IStudentRepository SetUpStudentRepository()
        {
            var repository = new Mock<IStudentRepository>();
            repository.Setup(x => x.GetAllStudents()).Returns(_randomStudent);

            return repository.Object;
        }

        private static List<Student> SetupStudents()
        {
            int _counter = new int();
            List<Student> _students = DataInitializer.GetAllProducts();

            foreach (Student _student in _students)
                _student.Id = (++_counter).ToString();

            return _students;
        }

//        actionResult = valuesController.Get(12);
//OkNegotiatedContentResult<string> conNegResult = Assert.IsType<OkNegotiatedContentResult<string>>(actionResult);
//        Assert.Equal("data: 12", conNegResult.Content);

        [Fact]
        public void ShouldNotNull()
        {
            var controller = new StudentsController(_unitOfWork);
            var result = controller.GetAllPost();

            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldReturnAllList()
        {
            var controller = new StudentsController(_unitOfWork);
            var result = controller.GetAllPost();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnObject = Assert.IsType<List<Student>>(okResult.Value);

            Assert.Equal(5, returnObject.Count);

        }


    }

    public interface IUnitOfWork
    {
        IStudentRepository StudentRepository { get; }
    }

    public interface IStudentRepository
    {
        IEnumerable<Student> GetAllStudents();
    }

    public class Student
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public ICollection<DairyReport> DairyReports { get; set; }
     
    }

    public class DairyReport

    {
        [BsonId]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreateBy { get; set; }

    }

    public class StudentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAllPost()
        {
            var listOfPost = _unitOfWork.StudentRepository.GetAllStudents(); 
            return Ok(listOfPost);
        }
    }
}
