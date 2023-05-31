using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNails.Admin.Models
{
    public class Social
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string ClassIcon { get; set; }
        public string BackgroundColor { get; set; }

        //
        public string URL { get; set; }
        public int Position { get; set; }
    }
}