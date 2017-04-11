using System;
using System.Collections.Generic;
using AutoMapper;
using HappyKids.Cores;
using HappyKids.Models.DataTranferObjects;
using HappyKids.Models.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HappyKids.Controllers
{
    [Route("api/students")]
    public class StudentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAllPost(StudentResourceParameters studentResourceParameters)
        {
            var listOfPost = _unitOfWork.StudentRepository.GetAllStudents(studentResourceParameters);
            var listOfDtos = Mapper.Map<List<StudentDTO>>(listOfPost);
            return Ok(listOfDtos);
        }

        [HttpGet("{id}", Name = "GetBookById")]
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
        public IActionResult CreateStudent([FromBody]StudentForCreateDTO student)
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

            return CreatedAtRoute("GetBookById", new {id = studentReturn.Id}, studentReturn);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(string id)
        {
            if (!_unitOfWork.StudentRepository.IsStudentExist(id))
            {
                return NotFound(id);
            }
            try
            {
                _unitOfWork.StudentRepository.RemoveStudent(id);
            }
            catch (Exception)
            {
                throw new Exception($"Cannot Delete Student ID:{id}");
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(string id,[FromBody] StudentForUpdateDTO student)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(student);
            }

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