using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace HappyKids.Models.Domain
{
    public class Student
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public ICollection<DairyReport> DairyReports { get; set; }
        public int IsActived { get; set; } = 1;
    }

   
}