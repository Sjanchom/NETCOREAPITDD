using System.Collections.Generic;
using HappyKids.Models.DataTranferObjects;
using HappyKids.Models.Domain;

namespace HappyKids.Cores
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAllStudents(StudentResourceParameters studentResourceParameters);
        Student GetStudentById(string id);
        void CreateStudent(Student student);
        void RemoveStudent(string studentId);
        bool IsStudentExist(string studentId);
        void UpdateStudent(Student selectedStudent);
    }
}
