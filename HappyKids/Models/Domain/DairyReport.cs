using System;
using MongoDB.Bson.Serialization.Attributes;

namespace HappyKids.Models.Domain
{
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

   
}