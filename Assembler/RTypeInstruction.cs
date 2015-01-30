using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class RTypeInstruction : Instruction
    {
        private List<int> sourceRegisters;
        public int destinationRegister { get; set; }
        public Opx OpX { get; set; }
        public RType rType { get; set; }
        public Conditional cond { get; set; }
        public int sBit{ get; set; }
        public InstructionType instructionType { get; set; }
        public InstructionSubType instructionSubType { get; set; }
        private Utilities utilities;

        public RTypeInstruction()
        {
            utilities = new Utilities();
            this.sourceRegisters = new List<int>();
            this.instructionType = InstructionType.RType;
            this.cond = Conditional.AL;
            this.destinationRegister = 0;
            this.sBit = 0;
        }

        public void AddSourceRegister(int register)
        {
            this.sourceRegisters.Add(register);
        }

        public new string ToString()
        {
            string s = "";

            foreach (int i in sourceRegisters)
            {
                s += utilities.ExtendBinaryNumber(i, 4);
            }

            s += utilities.ExtendBinaryNumber(destinationRegister, 4);
            s += utilities.ExtendBinaryNumber((int)OpX, 3);
            s += sBit;
            s += utilities.ExtendBinaryNumber((int)cond, 4);
            s += utilities.ExtendBinaryNumber((int)instructionSubType, 4);
            string hex = String.Format("{0:X2}", Convert.ToUInt64(s, 2));
            hex = utilities.ExtendHexNumber(hex, 6);

            return hex;
        }

        public enum RType
        {
            srl = 0x03,
            cmp = 0x02,
            jr = 0x01,
            basic = 0x00,
        }

        public enum Opx
        {
            add = 0x03,
            sub = 0x04,
            and = 0x00,
            or = 0x01,
            xor = 0x02,
            srl = 0x00,
            cmp = 0x00,
            jr = 0x00
        }
    }
}
