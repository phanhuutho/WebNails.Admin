using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNails.Admin.Models
{
    public class ActionDetail
    {
        public Guid ID { get; set; }
        public string Table { get; set; } 
        public int UserID { get; set; }
        public string DataJson { get; set; }
        public string Description { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public string Field { get; set; }
        public int FieldValue { get; set; }

        //
        public string Username { get; set; }
    }
}