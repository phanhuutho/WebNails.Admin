﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNails.Admin.Models
{
    public class Nail
    {
        public int ID { get; set; }
        public Guid GUID { get; set; }
        public string Domain { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string HyperLinkTell { get; set; }
        public string TextTell { get; set; }
        public string GooglePlus { get; set; }
        public string LinkGoogleMapAddress { get; set; }
        public string LinkIFrameGoogleMap { get; set; }
        public string LinkBookingAppointment { get; set; }
        public bool Coupons { get; set; }
        [DataType(DataType.MultilineText)]
        public string AboutUs { get; set; }
        [DataType(DataType.MultilineText)]
        public string AboutUsHome { get; set; }
        [DataType(DataType.MultilineText)]
        public string BusinessHours { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public int? NailApi_ID { get; set; }
        public List<SelectListItem> NailApis { get; set; }
        public int SalesOff { get; set; }
        public int FeePaypal { get; set; }
        public bool IsBuyerFeePaypal { get; set; }
        public int AmountMinimum { get; set; }
        public bool AutoSaleOffCode { get; set; }
    }
}