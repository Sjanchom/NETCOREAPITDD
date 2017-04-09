using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoMapper;
using HappyKids.Test.Helper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
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
                    dynamic maxProductId = _randomStudent.Last().Id;
                    dynamic nextProductId = Convert.ToInt32(maxProductId) + 1;
                    newStudent.Id = nextProductId.ToString();
                    _randomStudent.Add(newStudent);
                }));

            repository.Setup(x => x.RemoveStudent(It.IsAny<String>()))
              .Callback(new Action<String>(id =>
                {
                    _randomStudent.Single(x => x.Id == id).IsActived = 0;
                }));

            repository.Setup(x => x.UpdateStudent(It.IsAny<Student>()))
               .Callback(new Action<Student>(x =>
               {
                   var oldStudent = _randomStudent.Find(a => a.Id == x.Id);
                   oldStudent.BirthDate = x.BirthDate;
                   oldStudent.Name = x.Name;
                   oldStudent.IsActived = 1;
               }));

            repository.Setup(x => x.IsStudentExist(It.IsAny<String>()))
                .Returns(new Func<string, bool>(
                    id => _randomStudent.Any(x => x.Id == id)));

            return repository.Object;
        }

        private static List<Student> SetupStudents()
        {
            int counter = new int();
            List<Student> students = DataInitializer.GetAllProducts();

            foreach (Student student in students)
                student.Id = (++counter).ToString();

            return students;
        }


        [Fact]
        public void ShouldNotNull()
        {
            var controller = new StudentsController(_unitOfWork);
            var sut = controller.GetAllPost();

            Assert.NotNull(sut);
        }

        [Fact]
        public void ShouldReturnAllList()
        {
            var controller = new StudentsController(_unitOfWork);
            var sut = controller.GetAllPost();

            var okResult = Assert.IsType<OkObjectResult>(sut);
            var returnObject = Assert.IsType<List<StudentDTO>>(okResult.Value);
            var mappingAllListToDto = Mapper.Map<List<StudentDTO>>(_randomStudent);
            Assert.Equal(5, returnObject.Count);
            AssertObjects.ListAreEquals(returnObject, mappingAllListToDto);
        }

        [Fact]
        public void ShouldReturnCorrectId()
        {
            var controller = new StudentsController(_unitOfWork);
            var sut = controller.GetById("1");

            var okResult = Assert.IsType<OkObjectResult>(sut);
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
                Name = "Create Student",
                BirthDate = UtilHelper.PareDateTime("15/02/2015")
            };

            var sut = controller.CreateStudent(student);
            var maxProductIdBeforeAdd = _randomStudent.Max(a => Convert.ToInt32(a.Id));
            student.Id = (maxProductIdBeforeAdd + 1).ToString();


            var createAtRouteResult = Assert.IsType<CreatedAtRouteResult>(sut);
            var returnObject = Assert.IsType<StudentDTO>(createAtRouteResult.Value);

            Assert.Equal(student.Id, returnObject.Id);
            Assert.Equal(returnObject.Name,_randomStudent.Last().Name);
            Assert.Equal(student.BirthDate,returnObject.BirthDate);
        }

        [Fact]
        public void CreateShouldReturnNonFoundWhenReceiveNull()
        {
            var controller = new StudentsController(_unitOfWork);

            var sut = controller.CreateStudent(null);

            Assert.IsType<BadRequestResult>(sut);
        }

        [Fact]
        public void CreateShouldReturnBadRequestWhenNameIsNullOrEmpty()
        {
            var controller = new StudentsController(_unitOfWork);
            controller.ModelState.AddModelError("error", "some error");
            var emptyStudentDto = new StudentDTO {Name = null};

            var sut = controller.CreateStudent(emptyStudentDto);

            var badResult = Assert.IsType<BadRequestObjectResult>(sut);
            Assert.Equal(emptyStudentDto, badResult.Value);
        }


        [Fact]
        public void ShouldReturnNoContentWhenDeleteSuccess()
        {
            var controller = new StudentsController(_unitOfWork);

            var sut = controller.DeleteStudent("1");

            Assert.IsType<NoContentResult>(sut);

            Assert.Equal(0,_randomStudent.Single(x => x.Id == "1").IsActived);
        }

       
        //[Fact]
        //public void ShouldThrowExceptionWhenDeleteError()
        //{
        //    var controller = new StudentsController(_unitOfWork);

        //    var exception = Record.Exception(() => controller.DeleteStudent("sd1;;ll"));

        //    Assert.NotNull(exception);
        //    Assert.IsType<Exception>(exception);
        //    Assert.True(exception.Message.Contains("Cannot Delete Student ID:sd1;;ll"));

        //}

        [Fact]
        public void ShouldReturnNotFoundWhenStudentNotExist()
        {
            var controller = new StudentsController(_unitOfWork);

            var sut = controller.DeleteStudent("222");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(sut);
            Assert.Equal("222", notFoundResult.Value);
        }

        [Fact]
        public void ShouldReturnNoContentWhenUpdateSuccess()
        {
            var controller = new StudentsController(_unitOfWork);
            var student = new StudentForUpdateDTO();
            student.Name = "UpdateName";


            var sut = controller.UpdateStudent("1",student);
            var updateSudent = _randomStudent.Single(x => x.Id == "1");
            var mapUpdateStudentWithDto = Mapper.Map<StudentDTO>(updateSudent);

            Assert.IsType<NoContentResult>(sut);
            Assert.Equal(student.Name,mapUpdateStudentWithDto.Name);

        }

        [Fact]
        public void ShouldReturnNotFoundWhenUpdateStudentNotExist()
        {
            var controller = new StudentsController(_unitOfWork);
            var student = new StudentForUpdateDTO();
            student.Name = "UpdateName";

            var sut = controller.UpdateStudent("kjh1", student);
            Assert.IsType<NotFoundResult>(sut);

        }

        [Fact]
        public void OtherValueShouldNullWhenUseHttpPut()
        {
            var controller = new StudentsController(_unitOfWork);
            var student = new StudentForUpdateDTO();
            student.Name = "UpdateName";

            var sut = controller.UpdateStudent("1",student);
            var updateSudent = _randomStudent.Single(x => x.Id == "1");
            var mapUpdateStudentWithDto = Mapper.Map<StudentDTO>(updateSudent);

            Assert.IsType<NoContentResult>(sut);
            Assert.Null(mapUpdateStudentWithDto.BirthDate);
        }

        [Fact]
        public void ShouldReturnNoContentWhenSuccess()
        {
            var controller = new StudentsController(_unitOfWork);
            var patch = new JsonPatchDocument<StudentDTO>();
            patch.Operations.Add(new Operation<StudentDTO>("replace", "/Name", null, "PartialUpdate"));


            var studentInDb = _randomStudent.Single(x => x.Id == "2");
            var student = new Student();
            student.Name = "PartialUpdate";
            student.Id = studentInDb.Id;
            student.BirthDate = studentInDb.BirthDate;
            student.IsActived = studentInDb.IsActived;
            student.DairyReports = studentInDb.DairyReports;



            var sut = controller.PartialUpdateStudent("2",patch);


            Assert.IsType<NoContentResult>(sut);
            AssertObjects.PropertyValuesAreEquals(student, studentInDb);

        }

        [Fact]
        public void ShouldReturnNotFoundWhenIdNotExistInCollection()
        {
            var controller = new StudentsController(_unitOfWork);
            var patch = new JsonPatchDocument<StudentDTO>();
            patch.Operations.Add(new Operation<StudentDTO>("replace", "/Name", null, "PartialUpdate"));


            var sut = controller.PartialUpdateStudent("2sff", patch);


            Assert.IsType<NotFoundResult>(sut);
        }

    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class StudentDTO
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "You should fill out a Name.")]
        [MinLength(5, ErrorMessage = "The description shouldn't have more than 500 characters.")]
        public string Name { get; set; }

        [Required]
        public DateTime? BirthDate { get; set; }
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
        void RemoveStudent(string studentId);
        bool IsStudentExist(string studentId);
        void UpdateStudent(Student selectedStudent);
    }

    public class Student
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public ICollection<DairyReport> DairyReports { get; set; }
        public int IsActived { get; set; } = 1;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class StudentForUpdateDTO
    {
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
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
            var listOfDtos =  Mapper.Map<List<StudentDTO>>(listOfPost);
            return Ok(listOfDtos);
        }

        [HttpGet(Name = "GetBookById")]
        public IActionResult GetById(string id)
        {
            var selectedStudent = _unitOfWork.StudentRepository.GetStudentById(id);

            if (selectedStudent == null)
            {
                return NotFound();
            }

            var mapToDto = Mapper.Map<StudentDTO>(selectedStudent);

            return Ok(mapToDto);
        }

   


        [HttpPost]
        public IActionResult CreateStudent([FromBody]StudentDTO student)
        {

            if (student == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(student);
            }

            var studentMap = Mapper.Map<Student>(student);

            _unitOfWork.StudentRepository.CreateStudent(studentMap);

            var studentReturn = Mapper.Map<StudentDTO>(studentMap);

            return CreatedAtRoute("GetBookById", new {id = studentReturn.Id}, student);
        }

        [HttpDelete]
        public IActionResult DeleteStudent(string studentId)
        {
            if (!_unitOfWork.StudentRepository.IsStudentExist(studentId))
            {
                return NotFound(studentId);
            }
            try
            {
                _unitOfWork.StudentRepository.RemoveStudent(studentId);
            }
            catch (Exception)
            {
                throw new Exception($"Cannot Delete Student ID:{studentId}");
            }
            return NoContent();
        }

        public IActionResult UpdateStudent(string id, StudentForUpdateDTO student)
        {
            var selectedStudent = _unitOfWork.StudentRepository.GetStudentById(id);

            if (selectedStudent == null)
            {
                return NotFound();
            }

            Mapper.Map(student, selectedStudent);

            try
            {
                _unitOfWork.StudentRepository.UpdateStudent(selectedStudent);
            }
            catch (Exception)
            {
                throw new Exception($"Cannot Update Student ID:{id}");
            }


            return NoContent();
        }

        public IActionResult PartialUpdateStudent(string id,[FromBody] JsonPatchDocument<StudentDTO> patchDocStudentDto)
        {

            var selectedStudent = _unitOfWork.StudentRepository.GetStudentById(id);
            if (selectedStudent == null)
            {
                return NotFound();
            }
            var studentToPatch = Mapper.Map<StudentDTO>(selectedStudent);

            patchDocStudentDto.ApplyTo(studentToPatch,ModelState);
       
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            Mapper.Map(studentToPatch, selectedStudent);

            try
            {
                _unitOfWork.StudentRepository.UpdateStudent(selectedStudent);
            }
            catch (Exception)
            {
                throw new Exception($"Cannot Update Student ID:{selectedStudent.Id}");
            }

            return NoContent();
        }
    }

   
}