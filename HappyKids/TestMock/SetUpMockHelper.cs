using System;
using System.Linq;
using System.Collections.Generic;
using HappyKids.Cores;
using HappyKids.Models.DataTranferObjects;
using HappyKids.Models.Domain;
using Moq;

namespace HappyKids.TestMock
{
    public class SetUpMockHelper
    {
        public static List<Student> SetupStudents() => DataInitializer.Instance.GetStudents();

        public static IUnitOfWork SetUpUnitOfWork()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(x => x.StudentRepository)
                .Returns(SetUpStudentRepository());

            return unitOfWork.Object;
        }

        public static IStudentRepository SetUpStudentRepository()
        {
            var randomStudent = SetupStudents();

            var repository = new Mock<IStudentRepository>();

            repository.Setup(x => x.GetAllStudents(It.IsAny<StudentResourceParameters>()))
                .Returns(new Func<StudentResourceParameters, IEnumerable<Student>>(
                    studentResourceParameters =>
                        randomStudent
                        .Where(x => (string.IsNullOrWhiteSpace(studentResourceParameters.Name)
                        || x.Name.ToUpperInvariant().Contains(studentResourceParameters.Name.ToUpperInvariant())))
                        .Skip((studentResourceParameters.PageNumber - 1) *
                         studentResourceParameters.PageSize)
                        .Take(studentResourceParameters.PageSize)
                        .ToList()));

            repository.Setup(p => p.GetStudentById(It.IsAny<string>()))
                .Returns(new Func<string, Student>(
                    id => randomStudent.Find(p => p.Id.Equals(id))));

            repository.Setup(x => x.CreateStudent(It.IsAny<Student>()))
                .Callback(new Action<Student>(newStudent =>
                {
                    dynamic maxProductId = randomStudent.Last().Id;
                    dynamic nextProductId = Convert.ToInt32(maxProductId) + 1;
                    newStudent.Id = nextProductId.ToString();
                    randomStudent.Add(newStudent);
                }));

            repository.Setup(x => x.RemoveStudent(It.IsAny<String>()))
              .Callback(new Action<String>(id =>
              {
                  randomStudent.Single(x => x.Id == id).IsActived = 0;
              }));

            repository.Setup(x => x.UpdateStudent(It.IsAny<Student>()))
               .Callback(new Action<Student>(x =>
               {
                   var oldStudent = randomStudent.Find(a => a.Id == x.Id);
                   oldStudent.BirthDate = x.BirthDate;
                   oldStudent.Name = x.Name;
                   oldStudent.IsActived = 1;
               }));

            repository.Setup(x => x.IsStudentExist(It.IsAny<String>()))
                .Returns(new Func<string, bool>(
                    id => randomStudent.Any(x => x.Id == id)));

            return repository.Object;
        }

  
    }
}
