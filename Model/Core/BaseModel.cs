using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Core
{
    public class BaseModel
    {
        
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; } //primary key of User collection 

        public DateTime UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; } //primary key of User collection

        public BaseModel()
        {
        }
    }
}
