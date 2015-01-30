using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class JTypeInstruction : Instruction
    {
        public int destinationRegister { get; set; }
        public int immediateValue { get; set; }
        public InstructionType instructionType { get; set; }
        public InstructionSubType instructionSubType { get; set; }
        private Utilities utilities;

        public JTypeInstruction()
        {
            utilities = new Utilities();
            instructionType = InstructionType.JType;
            destinationRegister = -1;
        }

        public new string ToString()
        {
            string s = "";

            if (destinationRegister == -1)
            {
                s += utilities.ExtendBinaryNumber(immediateValue, 20);
            }
            else
            {
                s += utilities.ExtendBinaryNumber(destinationRegister, 4);
                s += utilities.ExtendBinaryNumber(immediateValue, 16);
            }
            s += utilities.ExtendBinaryNumber((int)instructionSubType, 4);
            string hex = String.Format("{0:X2}", Convert.ToUInt64(s, 2));
            hex = utilities.ExtendHexNumber(hex, 6);

            return hex;
        }
    }
}
