using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNails.Admin.Models
{
    public class NailCodeSale
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public int Sale { get; set; }
        public bool Status { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime DateTimeDelete { get; set; }
        public DateTime ExpireDateFrom { get; set; }
        public DateTime ExpireDateTo { get; set; }
        public int Nail_ID { get; set; }
        public int MinAmountSaleOff { get; set; }

        //
        public string Domain { get; set; }
        public List<SelectListItem> Nails { get; set; }
    }
}