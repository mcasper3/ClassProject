using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class DTypeInstruction : Instruction
    {
        public int sourceRegister { get; set; }
        public int destinationRegister { get; set; }
        public Conditional cond { get; set; }
        public int sBit { get; set; }
        public int immediateValue { get; set; }
        public InstructionType instructionType { get; set; }
        public InstructionSubType instructionSubType { get; set; }
        private Utilities utilities;

        public DTypeInstruction()
        {
            utilities = new Utilities();
            instructionType = InstructionType.DType;
            cond = Conditional.AL;
            sBit = 0;
        }

        public new string ToString()
        {
            string s = "";
            s += utilities.ExtendBinaryNumber(destinationRegister, 4);
            s += utilities.ExtendBinaryNumber(sourceRegister, 4);
            s += utilities.ExtendBinaryNumber(immediateValue, 7);
            s += sBit;
            s += utilities.ExtendBinaryNumber((int)cond, 4);
            s += utilities.ExtendBinaryNumber((int)instructionSubType, 4);
            string hex = String.Format("{0:X2}", Convert.ToUInt64(s, 2));
            hex = utilities.ExtendHexNumber(hex, 6);

            return hex;
        }
    }
}
