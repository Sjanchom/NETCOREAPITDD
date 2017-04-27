using System;
using System.Collections.Generic;
using AutoMapper;
using HappyKids.Cores;
using HappyKids.Helper;
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
        private readonly IUrlHelper _urlHelper;

        public StudentsController(IUnitOfWork unitOfWork, IUrlHelper urlHelper)
        {
            _unitOfWork = unitOfWork;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetBooks")]
        public IActionResult GetAllPost(StudentResourceParameters studentResourceParameters)
        {
            var listOfPost = _unitOfWork.StudentRepository.GetAllStudents(studentResourceParameters);
            var listOfDtos = Mapper.Map<List<StudentDTO>>(listOfPost);

            var previousPageLink = listOfPost.HasPrevious
                ? CreateStudentResourceUri(studentResourceParameters,
                    ResourceUriType.PreviousPage)
                : null;

            var nextPageLink = listOfPost.HasNext
                ? CreateStudentResourceUri(studentResourceParameters,
                    ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new PaginationHeader
            {
                PreviousPageLink = previousPageLink,
                NextPageLink = nextPageLink,
                TotalCount = listOfPost.TotalCount,
                PageSize = listOfPost.PageSize,
                CurrentPage = listOfPost.CurrentPage,
                TotalPages = listOfPost.TotalPages
            };
            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
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
        public IActionResult CreateStudent([FromBody] StudentForCreateDTO student)
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
        public IActionResult UpdateStudent(string id, [FromBody] StudentForUpdateDTO student)
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

        [HttpPatch("{id}")]
        public IActionResult PartialUpdateStudent(string id, [FromBody] JsonPatchDocument<StudentDTO> patchDocStudentDto)
        {

            if (patchDocStudentDto == null)
            {
                return NotFound();
            }
            var selectedStudent = _unitOfWork.StudentRepository.GetStudentById(id);
            if (selectedStudent == null)
            {
                return NotFound();
            }
            var studentToPatch = Mapper.Map<StudentDTO>(selectedStudent);

            patchDocStudentDto.ApplyTo(studentToPatch, ModelState);

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

        private string CreateStudentResourceUri(
            StudentResourceParameters authorsResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetBooks",
                        new
                        {
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetBooks",
                        new
                        {
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize
                        });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetBooks",
                        new
                        {
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize
                        });
            }
        }
    }
}