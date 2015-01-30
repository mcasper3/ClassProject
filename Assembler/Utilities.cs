using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Utilities
    {
        public Utilities()
        {

        }

        public string ExtendBinaryNumber(int number, int neededLength)
        {
            string currentBinary = "";
            int numToExtend;
            currentBinary = Convert.ToString(number, 2);
            if (currentBinary.Length < neededLength && number >= 0)
            {
                numToExtend = neededLength - currentBinary.Length;
                for (int j = 0; j < numToExtend; j++)
                {
                    currentBinary = 0 + currentBinary;
                }
            }
            else if (currentBinary.Length < neededLength && number < 0)
            {
                numToExtend = neededLength - currentBinary.Length;
                for (int j = 0; j < numToExtend; j++)
                {
                    currentBinary = 1 + currentBinary;
                }
            }
            else if (currentBinary.Length > neededLength)
            {
                currentBinary = currentBinary.Substring(currentBinary.Length - neededLength);
            }
            return currentBinary;
        }

        public string ExtendHexNumber(string number, int neededLength)
        {
            int numZeros;
            if (number.Length < neededLength)
            {
                numZeros = neededLength - number.Length;
                for (int j = 0; j < numZeros; j++)
                {
                    number = 0 + number;
                }
            }
            else if (number.Length > neededLength)
            {
                number = number.Substring(number.Length - neededLength);
            }
            return number;
        }
    }
}
