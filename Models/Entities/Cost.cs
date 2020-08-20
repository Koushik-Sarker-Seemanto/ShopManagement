using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    public class Cost
    {
        [BsonId]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
       
        public string Reason { get; set; }
        [Required]
        public  int Amount { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
