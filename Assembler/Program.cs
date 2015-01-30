using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.ConvertFile();
        }

        public void ConvertFile()
        {
            IOInteraction io = new IOInteraction();
            io.RunMainMenu();
        }
    }
}
