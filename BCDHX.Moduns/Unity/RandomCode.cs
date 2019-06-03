using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCDHX.Moduns.Unity
{
   public class RandomCode
    {
        public string RandomCodeGenral(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        


        private  string _numbers = "0123456789";
        public  string RandomNumber()
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder(6);
            string numberAsString = "";
            int numberAsNumber = 0;

            for (var i = 0; i < 4; i++)
            {
                builder.Append(_numbers[random.Next(1, _numbers.Length)]);
            }

            numberAsString = builder.ToString();
            numberAsNumber = int.Parse(numberAsString);
            return numberAsString;
        }

    }
}
