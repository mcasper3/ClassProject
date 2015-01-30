using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class DataSection
    {
        private List<int[]> dataInMemory;
        private Utilities utilities;

        public DataSection()
        {
            dataInMemory = new List<int[]>();
            utilities = new Utilities();
        }

        public void AddDataToMemory(int data, int memoryLocation)
        {
            int[] dataToAdd = new int[2];
            dataToAdd[0] = data;
            dataToAdd[1] = memoryLocation;
            dataInMemory.Add(dataToAdd);
        }

        public new string ToString()
        {
            string result = "";
            string hex;
            foreach (int[] i in dataInMemory)
            {
                hex = String.Format("{0:X2}", i[0]);
                hex = utilities.ExtendHexNumber(hex, 6);
                result += "\t" + i[1] + "\t\t\t:\t" + hex + ";\r\n";
            }
            return result;
        }

        public int GetSize()
        {
            return dataInMemory.Count;
        }
    }
}
