using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HappyKids.Models.DataTranferObjects
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class StudentForManipulationDTO
    {
        [Required(ErrorMessage = "Name Cannot be null.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Birthdate Cannot be null")]
        public string BirthDate { get; set; }
    }
}
