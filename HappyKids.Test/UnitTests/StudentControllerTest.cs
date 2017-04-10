using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HappyKids.Controllers;
using HappyKids.Test.Helper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using HappyKids.Cores;
using HappyKids.Models.DataTranferObjects;
using HappyKids.Models.Domain;

namespace HappyKids.Test.Controllers
{
    public class StudentControllerTest
    {
        private readonly List<Student> _randomStudent;
        private readonly IUnitOfWork _unitOfWork;
        private IUrlHelper _urlHelper;


        public StudentControllerTest()
        {
            MapperHelper.SetUpMapper();
            _randomStudent = SetupStudents();
            _unitOfWork = SetUpUnitOfWork();
            _urlHelper = SetupUrlHelper();
        }

        private IUrlHelper SetupUrlHelper()
        {
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<StudentResourceParameters>()))
                .Returns($"http:localhost/");

            return urlHelper.Object;
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

            repository.Setup(x => x.GetAllStudents(It.IsAny<StudentResourceParameters>()))
                .Returns(new Func<StudentResourceParameters, IEnumerable<Student>>(
                    studentResourceParameters => 
                        _randomStudent
                        .Where(x => (string.IsNullOrWhiteSpace(studentResourceParameters.Name) 
                        || x.Name.ToUpperInvariant().Contains(studentResourceParameters.Name.ToUpperInvariant())))
                        .Skip((studentResourceParameters.PageNumber - 1) *
                         studentResourceParameters.PageSize)
                        .Take(studentResourceParameters.PageSize)
                        .ToList()));

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
            var resource = new StudentResourceParameters();
            resource.PageSize = 20;

            var controller = new StudentsController(_unitOfWork);
            var sut = controller.GetAllPost(resource);

            Assert.NotNull(sut);
        }

        [Fact]
        public void ShouldReturnCorrectSize()
        {
            var resource = new StudentResourceParameters();
            resource.PageSize = 15;
            resource.PageNumber = 2;


            var controller = new StudentsController(_unitOfWork);
            var sut = controller.GetAllPost(resource);

            var okResult = Assert.IsType<OkObjectResult>(sut);
            var returnObject = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(StudentResourceParameters.maxPageSize, returnObject.Count);
            Assert.True(!returnObject.Contains(Mapper.Map<StudentDTO>(_randomStudent[1])));
            AssertObjects.PropertyValuesAreEquals(returnObject[0], Mapper.Map<StudentDTO>(_randomStudent[10]));
        }

        [Fact]
        public void ShouldReturnCorrectItem()
        {
            var resource = new StudentResourceParameters();
            resource.PageSize = 15;
            resource.PageNumber = 1;
            resource.Name = "N";

            var controller = new StudentsController(_unitOfWork);
            var sut = controller.GetAllPost(resource);

            var okResult = Assert.IsType<OkObjectResult>(sut);
            var returnObject = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.True(returnObject.All(x => x.Name.ToUpperInvariant().Contains(resource.Name.ToUpperInvariant())));
        }


        //[Fact]
        //public void ShouldReturnCorrectSequence()
        //{
        //    var resource = new StudentResourceParameters();
        //    resource.PageNumber = 1;
        //    resource.OrderBy = "BirthDate";

        //    var controller = new StudentsController(_unitOfWork);
        //    var sut = controller.GetAllPost(resource);

        //    var okResult = Assert.IsType<OkObjectResult>(sut);
        //    var returnObject = Assert.IsType<List<StudentDTO>>(okResult.Value);
        //    Assert.True(returnObject.All(x => x.Name.ToUpperInvariant().Contains(resource.Name.ToUpperInvariant())));
        //}

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

   
}