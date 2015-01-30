using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class BTypeInstruction : Instruction
    {
        public int immediateValue { get; set; }
        public Conditional cond { get; set; }
        public InstructionType instructionType { get; set; }
        public InstructionSubType instructionSubType { get; set; }
        private Utilities utilities;

        public BTypeInstruction()
        {
            utilities = new Utilities();
            instructionType = InstructionType.BType;
            cond = Conditional.AL;
        }

        public new string ToString()
        {
            string s = "";
            s += utilities.ExtendBinaryNumber(immediateValue, 16);
            s += utilities.ExtendBinaryNumber((int)cond, 4);
            s += utilities.ExtendBinaryNumber((int)instructionSubType, 4);
            string hex = String.Format("{0:X2}", Convert.ToUInt64(s, 2));
            hex = utilities.ExtendHexNumber(hex, 6);

            return hex;
        }
    }
}
