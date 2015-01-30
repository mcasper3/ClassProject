using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public enum Conditional
    {
        NV = 0x01,
        EQ = 0x02,
        NE = 0x03,
        VS = 0x04,
        VC = 0x05,
        MI = 0x06,
        PL = 0x07,
        CS = 0x08,
        CC = 0x09,
        HI = 0x0A,
        LS = 0x0B,
        GT = 0x0C,
        LT = 0x0D,
        GE = 0x0E,
        LE = 0x0F,
        AL = 0x00
    }
}
