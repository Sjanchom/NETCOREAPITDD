using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HappyKids.Models.DataTranferObjects { 
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class StudentDTO
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "You should fill out a Name.")]
        [MinLength(5, ErrorMessage = "The description shouldn't have more than 500 characters.")]
        public string Name { get; set; }

        [Required]
        public string BirthDate { get; set; }
    }

   
}