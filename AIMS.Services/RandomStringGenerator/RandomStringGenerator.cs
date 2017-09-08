using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services.RandomStringGenerator
{
    public class RandomStringGenerator
    {
        public string GenerateString()
        {
            string allowedChars = string.Empty;
            
            allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789#+@&$ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string generatedString = string.Empty;

            Random random = new Random();

            for (int i = 0; i <= 8; i++)
            {
                int iRand = random.Next(0, allowedChars.Length - 1);
                generatedString += allowedChars.Substring(iRand, 1);
            }
            return generatedString;
        }
    }
}
