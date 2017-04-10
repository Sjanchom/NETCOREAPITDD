using System;
using System.Diagnostics.CodeAnalysis;

namespace HappyKids.Models.DataTranferObjects
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class StudentForUpdateDTO
    {
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
    }

   
}