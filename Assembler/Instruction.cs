using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    interface Instruction
    {
        InstructionType instructionType { get; set; }
        InstructionSubType instructionSubType { get; set; }

        string ToString();
    }
    public enum InstructionType
    {
        RType = 0x03,
        DType = 0x02,
        BType = 0x01,
        JType = 0x00,
        InvalidType = 0x09
    }

    public enum InstructionSubType
    {
        arithmetic = 0x0C,
        srl = 0x0F,
        cmp = 0x0E,
        jr = 0x0D,
        lw = 0x08,
        sw = 0x09,
        addi = 0x0A,
        si = 0x0B,
        b = 0x04,
        bal = 0x05,
        j = 0x00,
        jal = 0x01,
        li = 0x02
    }
}
