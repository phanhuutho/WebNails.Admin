using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}