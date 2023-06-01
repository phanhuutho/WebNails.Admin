using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using WebNails.Admin.Models;

namespace WebNails.Admin.Utilities
{
    public class Commons
    {
        public static string RandomCodeNumber(int length = 1)
        {
            Random generator = new Random();
            int maxValue = 10;
            for (int i = 1; i < length; i++)
            {
                maxValue = maxValue * 10;
            }
            string result = generator.Next(0, 1000000).ToString("D" + length);
            return result;
        }

        public static void GenerateDataWeb(JsonInfo jsonInfo, string txtBusinessHours, string txtAboutUs, string txtAboutUsHome, string domain)
        {
            //VirtualData
            string path = string.Format(ConfigurationManager.AppSettings["VirtualUpload"], domain);

            //Write Business Hours
            File.WriteAllText(path + "business-hours.txt", txtBusinessHours);

            //Write About Us
            File.WriteAllText(path + "about-us.txt", txtAboutUs);

            //Write About Us Home
            File.WriteAllText(path + "home-about-us.txt", txtAboutUsHome);

            //Write info.json
            string json = JsonConvert.SerializeObject(jsonInfo);
            File.WriteAllText(path + "info.json", json);
        }
    }
}