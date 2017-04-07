using System;
using System.Collections.Generic;
using System.Linq;
using HappyKids.Test.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using Moq;
using Xunit;

namespace HappyKids.Test.Controllers
{
    public class PostControllerTest
    {
        private readonly List<Student> _randomStudent;
        private readonly IUnitOfWork _unitOfWork;


        public PostControllerTest()
        {
            MapperHelper.SetUpMapper();
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
            repository.Setup(p => p.GetStudentById(It.IsAny<string>()))
                .Returns(new Func<string, Student>(
                    id => _randomStudent.Find(p => p.Id.Equals(id))));

            repository.Setup(x => x.CreateStudent(It.IsAny<Student>()))
                .Callback(new Action<Student>(newStudent =>
                {
                    dynamic maxProductID = _randomStudent.Last().Id;
                    dynamic nextProductID = Convert.ToInt32(maxProductID) + 1;
                    newStudent.Id = nextProductID.ToString();
                    _randomStudent.Add(newStudent);
                }));

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

        [Fact]
        public void ShouldReturnCorrectId()
        {
            var controller = new StudentsController(_unitOfWork);
            var result = controller.GetById("1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnObject = Assert.IsType<StudentDTO>(okResult.Value);

            Assert.Equal("1", returnObject.Id);
            Assert.Equal(_randomStudent.Single(x => x.Id == "1").Name, returnObject.Name);
        }

        [Fact]
        public void ShouldreturnNonFoundWhenInCorrectId()
        {
            var controller = new StudentsController(_unitOfWork);
            var result = controller.GetById("1--");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ShouldCanPost()
        {
            var controller = new StudentsController(_unitOfWork);

            var student = new StudentDTO()
            {
                Name = "Create Student"
            };

            var result = controller.CreateStudent(student);
            var maxProductIDBeforeAdd = _randomStudent.Max(a => Convert.ToInt32(a.Id));
            student.Id = (maxProductIDBeforeAdd + 1).ToString();


            var createAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            var returnObject = Assert.IsType<StudentDTO>(createAtRouteResult.Value);

            Assert.Equal(student.Id, returnObject.Id);
            Assert.Equal(returnObject.Name,_randomStudent.Last().Name);
        }

        [Fact]
        public void ShouldHaveUpdateMethod()
        {
            var controller = new StudentsController(_unitOfWork);
            var student = new StudentDTO();
            student.Id = "1";
            student.Name = "UpdateName";

            var result = controller.UpdateStudent(student);
        }

        [Fact]
        public void WhenIdNotExistinCollectionShouldreturnNotFound()
        {
        }
    }

    public class StudentDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public interface IUnitOfWork
    {
        IStudentRepository StudentRepository { get; }
    }

    public interface IStudentRepository
    {
        IEnumerable<Student> GetAllStudents();
        Student GetStudentById(string id);
        void CreateStudent(Student student);
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


        public IActionResult GetById(string id)
        {
            var selectedStudent = _unitOfWork.StudentRepository.GetStudentById(id);

            if (selectedStudent == null)
            {
                return NotFound();
            }

            var mapToDto = AutoMapper.Mapper.Map<StudentDTO>(selectedStudent);

            return Ok(mapToDto);
        }

        public object UpdateStudent(StudentDTO student)
        {
            return null;
        }

        public IActionResult CreateStudent(StudentDTO student)
        {
            var studentMap = AutoMapper.Mapper.Map<Student>(student);

            _unitOfWork.StudentRepository.CreateStudent(studentMap);

            var studentReturn = AutoMapper.Mapper.Map<StudentDTO>(studentMap);

            return CreatedAtRoute("GetBookById", new {id = studentReturn.Id}, student);
        }
    }
}