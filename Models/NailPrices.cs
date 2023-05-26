using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNails.Admin.Models
{
    public class NailPrices
    {
        public int ID { get; set; }
        public int Nail_ID { get; set; }
        public int Position { get; set; }
        public string URL { get; set; }
        public bool Status { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime DateTimeDelete { get; set; }
    }
}