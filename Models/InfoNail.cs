using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNails.Admin.Models
{
    public class InfoNail
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string TextTell { get; set; }
        public string HyperLinkTell { get; set; }
        public string LinkBookingAppointment { get; set; }
        public string GooglePlus { get; set; }
        public string LinkGoogleMapAddress { get; set; }
        public string LinkIFrameGoogleMap { get; set; }
        public string BusinessHours { get; set; }
        public string HomeAboutUs { get; set; }
        public string AboutUs { get; set; }
        public bool ShowCoupon { get; set; }
        public JsonSocial Telegram { get; set; }
        public JsonSocial Facebook { get; set; }
        public JsonSocial Instagram { get; set; }
        public JsonSocial Twitter { get; set; }
        public JsonSocial Youtube { get; set; }
    }
}