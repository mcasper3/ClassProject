using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class InstructionTransformer
    {
        private Dictionary<string, InstructionSubType> RInstructions = new Dictionary<string, InstructionSubType>();
        private Dictionary<string, InstructionSubType> BInstructions = new Dictionary<string, InstructionSubType>();
        private Dictionary<string, InstructionSubType> DInstructions = new Dictionary<string, InstructionSubType>();
        private Dictionary<string, InstructionSubType> JInstructions = new Dictionary<string, InstructionSubType>();
        private Dictionary<string, Conditional> conditionals = new Dictionary<string, Conditional>();
        private Dictionary<string, int> labels = new Dictionary<string, int>();
        private DataSection dataSection = new DataSection();
        private int startOfDataSection;
        private string startLabel;
        private int startOfInstructions;
        private bool dataSectionLocated;

        /// <summary>
        /// Creates an instance of InstructionTransformer and initializes
        /// private dictionaries containing r, j, b and d type instructions,
        /// and conditionals.
        /// </summary>
        public InstructionTransformer()
        {
            // adds all of the instruction types to dictionaries
            RInstructions.Add("sub", InstructionSubType.arithmetic);
            RInstructions.Add("add", InstructionSubType.arithmetic);
            RInstructions.Add("and", InstructionSubType.arithmetic);
            RInstructions.Add("xor", InstructionSubType.arithmetic);
            RInstructions.Add("or", InstructionSubType.arithmetic);
            RInstructions.Add("srl", InstructionSubType.srl);
            RInstructions.Add("cmp", InstructionSubType.cmp);
            RInstructions.Add("jr", InstructionSubType.jr);
            DInstructions.Add("lw", InstructionSubType.lw);
            DInstructions.Add("sw", InstructionSubType.sw);
            DInstructions.Add("addi", InstructionSubType.addi);
            BInstructions.Add("b", InstructionSubType.b);
            BInstructions.Add("bal", InstructionSubType.bal);
            JInstructions.Add("j", InstructionSubType.j);
            JInstructions.Add("jal", InstructionSubType.jal);
            JInstructions.Add("li", InstructionSubType.li);
            conditionals.Add("al", Conditional.AL);
            conditionals.Add("nv", Conditional.NV);
            conditionals.Add("eq", Conditional.EQ);
            conditionals.Add("ne", Conditional.NE);
            conditionals.Add("vs", Conditional.VS);
            conditionals.Add("vc", Conditional.VC);
            conditionals.Add("mi", Conditional.MI);
            conditionals.Add("pl", Conditional.PL);
            conditionals.Add("cs", Conditional.CS);
            conditionals.Add("cc", Conditional.CC);
            conditionals.Add("hi", Conditional.HI);
            conditionals.Add("ls", Conditional.LS);
            conditionals.Add("gt", Conditional.GT);
            conditionals.Add("lt", Conditional.LT);
            conditionals.Add("ge", Conditional.GE);
            conditionals.Add("le", Conditional.LE);
            dataSectionLocated = false;
            startOfInstructions = -1;
        }

        // Fills the labels dictionary with the labels used in the assembly file
        private void PerformFirstPass(string[] assemblyInstructions)
        {
            int lineNum = 0;
            string instruction = "";
            int numOfInstructions = 0;
            //InstructionSubType subType;
            //Conditional cond;
            foreach (string line in assemblyInstructions)
            {
                lineNum++;
                // If the line in the assembly file is not a comment or is not blank
                if (CheckIfLineContainsInstruction(line))
                {
                    numOfInstructions++;
                    // Split the instruction on the semi colon in case there is a comment on the line
                    string[] linePieces = line.ToLower().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    instruction = linePieces[0];

                    string[] instructionPieces = instruction.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    string potentialInstruction = instructionPieces[0];
                    // if the first item on a line ends with a colon or starts with a period it is a label, .global, or .data
                    if (potentialInstruction.EndsWith(":") || potentialInstruction.StartsWith("."))
                    {
                        if (instructionPieces.Length == 1)
                        {
                            AddLabel(potentialInstruction, instructionPieces, lineNum, numOfInstructions--);
                        }
                        else
                        {
                            if (potentialInstruction.Equals(".global"))
                            {
                                startLabel = instructionPieces[1];
                                startOfInstructions = lineNum;
                                numOfInstructions--;
                            }
                            numOfInstructions += AddLabel(potentialInstruction, instructionPieces, lineNum, numOfInstructions);
                        }
                    }
                }
            }
        }

        public bool IsDataSectionLocated()
        {
            return dataSectionLocated;
        }

        public string ConvertDataSectionToString()
        {
            return dataSection.ToString();
        }

        public int GetSizeOfDataSection()
        {
            return dataSection.GetSize();
        }

        private void CreateDataSection(string[] assemblyInstructions)
        {
            string line = "";
            int dataSectionStartLine = 0;
            labels.TryGetValue(".data", out dataSectionStartLine);
            int memoryLocation = dataSectionStartLine - 1;
            int endIndex;
            if (startOfInstructions > startOfDataSection)
            {
                endIndex = startOfInstructions - 1;
            }
            else
            {
                endIndex = assemblyInstructions.Length;
            }

            for (int i = startOfDataSection; i < endIndex; i++)
            {
                line = assemblyInstructions[i];
                if (CheckIfLineContainsInstruction(line))
                {
                    string[] temp = line.Split(';');
                    string[] linePieces = temp[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    switch (linePieces[1])
                    {
                        case ".ascii":
                            memoryLocation = AddAsciiToMemory(linePieces, memoryLocation);
                            break;
                        case ".asciiz":
                            memoryLocation = AddAsciizToMemory(linePieces, memoryLocation);
                            break;
                        case ".byte":
                            memoryLocation = AddByteToMemory(linePieces, memoryLocation);
                            break;
                        case ".word":
                            memoryLocation = AddWordToMemory(linePieces, memoryLocation);
                            break;
                        case ".halfword":
                            memoryLocation = AddHalfwordToMemory(linePieces, memoryLocation);
                            break;
                        case ".space":
                            memoryLocation = AddSpaceToMemory(linePieces, memoryLocation);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts a string array of instructions in assembly code into
        /// an array of binary instructions
        /// </summary>
        /// <param name="assemblyInstructions">The list of assembly instructions to be converted</param>
        /// <returns>A list of binary instructions</returns>
        public Instruction[] ConvertInstructions(string[] assemblyInstructions)
        {
            string instruction = "";
            List<Instruction> instructions = new List<Instruction>();
            int lineNum = 0;
            int numOfInstructions = 0;
            PerformFirstPass(assemblyInstructions);
            // If there is no .global _start, throw an error stating syntax is wrong
            if (startOfInstructions == -1)
            {
                throw new InvalidInstructionException("The file must contain a starting label declared by \".global _start\" where \"_start\" is arbitrary");
            }

            // If there is a data section, add the pieces to memory
            if (dataSectionLocated)
            {
                CreateDataSection(assemblyInstructions);
            }

            int endIndex;
            if (startOfInstructions > startOfDataSection){
                endIndex = assemblyInstructions.Length;
            } else {
                endIndex = startOfDataSection - 1;
            }

            for (int j = startOfInstructions; j < endIndex; j++)
            {
                lineNum++;
                bool lineContainsInstruction = CheckIfLineContainsInstruction(assemblyInstructions[j]);
                if (lineContainsInstruction)
                {
                    numOfInstructions++;
                    string[] linePieces = assemblyInstructions[j].ToLower().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    instruction = linePieces[0];
                    Instruction i = CreateInstruction(instruction, lineNum);
                    // If an instruction could be created determine what registers are needed and add it to the list of instructions
                    if (i != null)
                    {
                        DetermineNeededInformation(instruction, i, instructions.Count + 1);
                        instructions.Add(i);
                    }
                }
            }
            return instructions.ToArray<Instruction>();
        }

        private Instruction CreateInstruction(string instruction, int lineNum) {
            string[] instructionPieces = instruction.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            Instruction i = null;
            bool containsLabel = 
                DetermineIfContainsLabel(instructionPieces.Length, instructionPieces[0].Substring(0, instructionPieces[0].Length - 1));
            // If there is a label on the line, skip the first piece
            if (containsLabel)
            {
                i = DetermineInstruction(instructionPieces[1], lineNum);
            }
            // Otherwise, don't
            else
            {
                i = DetermineInstruction(instructionPieces[0], lineNum);
            }

            return i;
        }

        private bool DetermineIfContainsLabel(int numPieces, string instructionPiece)
        {
            int temp = 0;
            bool containsLabel = false;
            // If there is only one piece or if the dictionary of label contains it, then the line contains a label
            if(numPieces == 1 || labels.TryGetValue(instructionPiece, out temp)){
                containsLabel = true;
            }
            return containsLabel;
        }

        private void DetermineNeededInformation(string instruction, Instruction i, int numOfInstructions)
        {
            string[] instructionPieces = instruction.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string[] instructionPiecesToDecode;
            bool containsLabel = 
                DetermineIfContainsLabel(instructionPieces.Length, instructionPieces[0].Substring(0, instructionPieces[0].Length - 1));

            if (containsLabel)
            {
                // Take out the first two entries before decoding instructionPieces
                instructionPiecesToDecode = new string[instructionPieces.Length - 2];
                Array.Copy(instructionPieces, 2, instructionPiecesToDecode, 0, instructionPiecesToDecode.Length);
            }
            else
            {
                // Decode instructionPieces without first entry
                instructionPiecesToDecode = new string[instructionPieces.Length - 1];
                Array.Copy(instructionPieces, 1, instructionPiecesToDecode, 0, instructionPiecesToDecode.Length);
            }

            switch (i.instructionType)
            {
                case InstructionType.RType:
                    DecodeRTypeInstruction(i, instructionPiecesToDecode);
                    break;
                case InstructionType.BType:
                    DecodeBTypeInstruction(i, instructionPiecesToDecode, numOfInstructions);
                    break;
                case InstructionType.DType:
                    DecodeDTypeInstruction(i, instructionPiecesToDecode);
                    break;
                case InstructionType.JType:
                    DecodeJTypeInstruction(i, instructionPiecesToDecode);
                    break;
            }
        }

        private bool CheckIfLineContainsInstruction(string line)
        {
            bool containsLine = true;
            string[] tempLine = line.Split(';');
            string[] linePieces = tempLine[0].ToLower().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            int temp;
            if (line.IndexOf(';') == 0 || line.Equals("") || (linePieces.Length < 2 && labels.TryGetValue(linePieces[0].Substring(0, linePieces[0].Length - 1), out temp)))
            {
                containsLine = false;
            }
            return containsLine;
        }

        private Instruction DetermineInstruction(string instructionType, int lineNum)
        {
            string instruction = "";
            Conditional cond = Conditional.AL;
            Instruction i = null;
            bool hasCond = false;
            bool setSBit = CheckForSetBit(instructionType);

            // Has a conditional if the last two letters are in the conditionals library and is not "bal"
            if (instructionType.Length > 2 && conditionals.TryGetValue(instructionType.Substring(instructionType.Length - 2), out cond) 
                && !instructionType.Equals("bal"))
            {
                // Sets instruction to the instructionType without the conditional letters
                instruction = instructionType.Substring(0, instructionType.Length - 2);
                hasCond = true;
            }
            // Also has a conditional if the last letter is s and the previous two are in the conditionals library
            else if (instructionType.Length > 2 && instructionType.EndsWith("s"))
            {
                if (conditionals.TryGetValue(instructionType.Substring(instructionType.Length - 3, 2), out cond))
                {
                    instruction = instructionType.Substring(0, instructionType.Length - 3);
                    hasCond = true;
                }
                else
                {
                    instruction = instructionType.Substring(0, instructionType.Length - 1);
                }
            }
            else
            {
                instruction = instructionType;
            }

            i = CreateCorrectTypeOfInstruction(instruction, lineNum);

            if (hasCond)
            {
                SetConditional(i, cond);
            }

            if (setSBit)
            {
                i = SetSBit(i);
            }

            return i;
        }

        private void DecodeRTypeInstruction(Instruction i, string[] registers)
        {
            RTypeInstruction r = (RTypeInstruction)i;
            int[] registersUsed = DetermineRegistersUsed(registers, r.instructionSubType);

            switch (r.instructionSubType)
            {
                case InstructionSubType.arithmetic:
                    r.destinationRegister = registersUsed[0];
                    r.AddSourceRegister(registersUsed[2]);
                    r.AddSourceRegister(registersUsed[1]);
                    break;
                case InstructionSubType.jr:
                    r.AddSourceRegister(registersUsed[0]);
                    break;
                case InstructionSubType.cmp:
                    r.AddSourceRegister(registersUsed[1]);
                    r.AddSourceRegister(registersUsed[0]);
                    break;
                case InstructionSubType.srl:
                    r.destinationRegister = registersUsed[0];
                    r.AddSourceRegister(registersUsed[2]);
                    r.AddSourceRegister(registersUsed[1]);
                    break;
            }
        }

        private void DecodeDTypeInstruction(Instruction i, string[] registers)
        {
            DTypeInstruction d = (DTypeInstruction)i;
            int[] registersUsed = DetermineRegistersUsed(registers, d.instructionSubType);

            switch (d.instructionSubType)
            {
                case InstructionSubType.lw:
                    d.destinationRegister = registersUsed[0];
                    d.immediateValue = registersUsed[1];
                    d.sourceRegister = registersUsed[2];
                    break;
                case InstructionSubType.sw:
                    d.destinationRegister = registersUsed[0];
                    d.immediateValue = registersUsed[1];
                    d.sourceRegister = registersUsed[2];
                    break;
                case InstructionSubType.addi:
                    d.destinationRegister = registersUsed[0];
                    d.sourceRegister = registersUsed[1];
                    d.immediateValue = registersUsed[2];
                    break;
                //case InstructionSubType.si:
                //    break;
            }
        }

        private void DecodeBTypeInstruction(Instruction i, string[] immediate, int currentInstructionNumber)
        {
            BTypeInstruction b = (BTypeInstruction)i;
            bool containsLabel = DetermineIfContainsLabel(0, immediate[0]);
            if (containsLabel)
            {
                int immed = 0;
                labels.TryGetValue(immediate[0], out immed);
                b.immediateValue = immed - currentInstructionNumber - 1;
            }
            else
            {
                b.immediateValue = DetermineRegistersUsed(immediate, b.instructionSubType)[0] - 1;
            }
        }

        private void DecodeJTypeInstruction(Instruction i, string[] regAndImmed)
        {
            JTypeInstruction j = (JTypeInstruction)i;
            int[] regAndImmedValue = DetermineRegistersUsed(regAndImmed, j.instructionSubType);

            if (j.instructionSubType == InstructionSubType.li)
            {
                j.destinationRegister = regAndImmedValue[0];
                j.immediateValue = regAndImmedValue[1];
            }
            else
            {
                j.immediateValue = regAndImmedValue[0];
            }
        }

        // Determines which type of instruction to create based on if instruction is in a specific dictionary
        // and creates a new instruction of that type. Instructions are in the form of lw, sw, add, bal, etc.
        private Instruction CreateCorrectTypeOfInstruction(string instruction, int lineNum)
        {
            InstructionSubType subType;
            Instruction i = null;
            if (JInstructions.TryGetValue(instruction, out subType))
            {
                JTypeInstruction j = new JTypeInstruction();
                j.instructionSubType = subType;
                i = j;
            }
            else if (BInstructions.TryGetValue(instruction, out subType))
            {
                BTypeInstruction b = new BTypeInstruction();
                b.instructionSubType = subType;
                i = b;
            }
            else if (DInstructions.TryGetValue(instruction, out subType))
            {
                DTypeInstruction d = new DTypeInstruction();
                d.instructionSubType = subType;
                i = d;
            }
            else if (RInstructions.TryGetValue(instruction, out subType))
            {
                RTypeInstruction r = new RTypeInstruction();
                r.instructionSubType = subType;
                // Determines what the OpX bits should be based on the instruction given in the form add, sub, xor, etc.
                r.OpX = (RTypeInstruction.Opx) Enum.Parse(typeof(RTypeInstruction.Opx), instruction);
                i = r;
            }
            else
            {
                throw new InvalidInstructionException(String.Format("The instruction on line {0} is an invalid instruction.", lineNum));
            }

            return i;
        }

        // Sets the conditional bits
        private void SetConditional(Instruction i, Conditional cond)
        {
            switch (i.instructionType)
            {
                case InstructionType.RType:
                    RTypeInstruction r = (RTypeInstruction)i;
                    r.cond = cond;
                    break;
                case InstructionType.BType:
                    BTypeInstruction b = (BTypeInstruction)i;
                    b.cond = cond;
                    break;
                case InstructionType.DType:
                    DTypeInstruction d = (DTypeInstruction)i;
                    d.cond = cond;
                    break;
            }
        }

        private int AddLabel(string potentialInstruction, string[] linePieces, int lineNumInFile, int instructionNum)
        {
            // If the first piece of the line contains .data it is the data section
            int offset = 0;
            if (potentialInstruction.Equals(".data") && !dataSectionLocated)
            {
                startOfDataSection = lineNumInFile;
                dataSectionLocated = true;
                labels.Add(potentialInstruction, instructionNum);
            }
            else if (potentialInstruction.Equals(".data"))
            {
                throw new InvalidLabelException("Multiple .data sections were detected at lines "
                    + startOfDataSection + " and " + lineNumInFile);
            }
            // Otherwise it is a label
            else// if (!dataSectionLocated)
            {
                try
                {
                    labels.Add(potentialInstruction.Substring(0, potentialInstruction.Length - 1), instructionNum);
                    if (dataSectionLocated)
                    {
                        offset = linePieces.Length - 3;
                    }
                }
                catch (ArgumentException)
                {
                    throw new InvalidLabelException(String.Format("The label {0} on line {1} was "
                    + "already used as a label.", potentialInstruction, lineNumInFile));
                }
            }
            return offset;
        }

        /*
         * Checks which registers and immediates are being used
         */
        private int[] DetermineRegistersUsed(string[] registers, InstructionSubType type)
        {
            int[] registersUsed = new int[registers.Length + 1];
            int potentialReg = 0;
            int immed = 0;
            int count = 0;

            foreach (string s in registers)
            {
                // If the register used is not the stack pointer or link register
                if (!s.Equals("$sp") && !s.Equals("$lr") 
                    && (type != InstructionSubType.j && type != InstructionSubType.jal && type != InstructionSubType.li))
                {
                    potentialReg = int.Parse(s.Substring(s.IndexOf('r') + 1));
                }
                else if (s.Equals("$sp"))
                {
                    potentialReg = 14;
                }
                else if (s.Equals("$lr"))
                {
                    potentialReg = 15;
                }
                // If it is a load or store inustruction (lw $r2 (30)$r3) The immediate is the
                // value to be determined when count is 1
                if ((type == InstructionSubType.lw || type == InstructionSubType.sw) && count == 1)
                {
                    // The immediate value must be between -64 and 63
                    immed = int.Parse(s.Substring(s.IndexOf('(') + 1, s.IndexOf(')') - 1));
                    if (immed > 63 || immed < -64)
                    {
                        throw new InvalidRegisterException("Immediate values for lw, and sw cannot exceed 63 or be smaller than -64.");
                    }
                    else
                    {
                        registersUsed[count] = immed;
                        count++;
                    }
                }

                // If the register being looked at is the last entry for addi, b, bal, j, jal, or li, then it is an immediate
                // value and must be between -64 and 63 for addi and between -2^16 and 2^16 - 1
                if ((type == InstructionSubType.addi || type == InstructionSubType.b || type == InstructionSubType.bal)
                    && count == registers.Length - 1)
                {
                    immed = int.Parse(s);
                    if ((immed > 63 || immed < -64) && type == InstructionSubType.addi)
                    {
                        throw new InvalidRegisterException("Immediate values for addi cannot exceed 63 or be smaller than -64.");
                    }
                    else if (immed > 32767 || immed < -32768)
                    {
                        throw new InvalidRegisterException("Immediate values for b and bal instructions must not exceed 32,767 or be smaller than -32,768.");
                    }
                    else
                    {
                        registersUsed[count] = immed;
                        count++;
                    }
                }
                // If the instruction is a j, jal, or li, the immedate must be between -2^20 and 2^20 - 1
                else if (type == InstructionSubType.j || type == InstructionSubType.jal || (type == InstructionSubType.li && count == 1))
                {
                    bool containsLabel = DetermineIfContainsLabel(0, s);

                    if (containsLabel)
                    {
                        labels.TryGetValue(s, out immed);
                    }
                    else
                    {
                        immed = int.Parse(s);
                    }

                    if (immed > 1048575 || immed < -1048576)
                    {
                        throw new InvalidRegisterException("Immediate values for JType instructions cannot exceed (2^20) - 1 or be smaller than -(2^20)");
                    }
                    else
                    {
                        registersUsed[count] = immed;
                        count++;
                    }
                }
                // Otherwise, if the register is between 0 and 15, it is a valid register
                else if (potentialReg >= 0 && potentialReg <= 15)
                {
                    if (type == InstructionSubType.li)
                    {
                    potentialReg = int.Parse(s.Substring(s.IndexOf('r') + 1));
                    }
                    registersUsed[count] = potentialReg;
                    count++;
                }
                else
                {
                    throw new InvalidRegisterException();
                }
            }
            return registersUsed;
        }

        private bool CheckForSetBit(string instruction)
        {
            Conditional temp;
            InstructionSubType temp1;
            bool setSBit = false;
            /*
             * Checks if the instruction is a compare instruction or if it ends with an s and does not end with a conditional that ends with s
             * (for example vs) and if it is a valid instruction without the last letter, or without the last three letters
             * (for example addgts is add without the last three letters a.k.a. without the conditional and s bit) 
             */
            if (instruction.StartsWith("cmp"))
            {
                setSBit = true;
            }
            else if (instruction.EndsWith("s") && !conditionals.TryGetValue(instruction.Substring(instruction.Length - 2), out temp)
                && (RInstructions.TryGetValue(instruction.Substring(0, instruction.Length - 1), out temp1) 
                || RInstructions.TryGetValue(instruction.Substring(0, instruction.Length - 3), out temp1) 
                || DInstructions.TryGetValue(instruction.Substring(0, instruction.Length - 3), out temp1)))
            {
                if (instruction.StartsWith("jr"))
                {
                    throw new InvalidInstructionException("jr instructions cannot enable the set bit");
                }
                else if (JInstructions.TryGetValue(instruction.Substring(0, instruction.Length - 1), out temp1)
                    || JInstructions.TryGetValue(instruction.Substring(0, instruction.Length - 3), out temp1)
                    || DInstructions.TryGetValue(instruction.Substring(0, instruction.Length - 1), out temp1)
                    || DInstructions.TryGetValue(instruction.Substring(0, instruction.Length - 3), out temp1))
                {
                    throw new InvalidInstructionException("J and B type instructions cannot set the s bit.");
                }
                else
                {
                    setSBit = true;
                }
            }

            return setSBit;
        }

        private Instruction SetSBit(Instruction i)
        {
            if (i.instructionType == InstructionType.RType)
            {
                RTypeInstruction r = (RTypeInstruction)i;
                r.sBit = 1;
                return r;
            }
            else
            {
                DTypeInstruction d = (DTypeInstruction)i;
                d.sBit = 1;
                return d;
            }
        }

        // Adds each character of a string of data to the data section object
        private int AddAsciiToMemory(string[] data, int memoryLocation)
        {
            string stringToStore = data[2].Substring(1, data[2].Length - 2);
            char[] characters = stringToStore.ToCharArray();

            for (int i = 0; i < characters.Length; i++)
            {
                dataSection.AddDataToMemory((int)characters[i], ++memoryLocation);
            }
            return memoryLocation;
        }

        // Adds each character of a string of data to the data section object and appends a null terminator to the end
        private int AddAsciizToMemory(string[] data, int memoryLocation)
        {
            string stringToStore = data[2].Substring(1, data[2].Length - 2);
            char[] characters = stringToStore.ToCharArray();

            for (int i = 0; i < characters.Length; i++)
            {
                dataSection.AddDataToMemory((int)characters[i], ++memoryLocation);
            }

            dataSection.AddDataToMemory(0, memoryLocation++);
            return memoryLocation;
        }

        // Adds each word to the data section object
        private int AddWordToMemory(string[] data, int memoryLocation)
        {
            string neededPartOfData;
            for (int i = 2; i < data.Length; i++)
            {
                if (data[i].EndsWith(","))
                {
                    neededPartOfData = data[i].Substring(0, data[i].Length - 1);
                }
                else
                {
                    neededPartOfData = data[i];
                }
                dataSection.AddDataToMemory(Int32.Parse(neededPartOfData), ++memoryLocation);
            }
            return memoryLocation;
        }

        // Adds each halfword to the data section object
        private int AddHalfwordToMemory(string[] data, int memoryLocation)
        {
            string neededPartOfData;
            for (int i = 2; i < data.Length; i++)
            {
                if (data[i].EndsWith(","))
                {
                    neededPartOfData = data[i].Substring(0, data[i].Length - 1);
                }
                else
                {
                    neededPartOfData = data[i];
                }
                dataSection.AddDataToMemory(Int16.Parse(neededPartOfData), ++memoryLocation);
            }
            return memoryLocation;
        }

        // Adds each byte of data to the data section object
        private int AddByteToMemory(string[] data, int memoryLocation)
        {
            string neededPartOfData;
            for (int i = 2; i < data.Length; i++)
            {
                if (data[i].EndsWith(","))
                {
                    neededPartOfData = data[i].Substring(0, data[i].Length - 1);
                }
                else
                {
                    neededPartOfData = data[i];
                }
                dataSection.AddDataToMemory(Int32.Parse(neededPartOfData), ++memoryLocation);
            }
            return memoryLocation;
        }

        // Allocates a designated amount of space in memory to be set aside
        private int AddSpaceToMemory(string[] data, int memoryLocation)
        {
            int amountToAdd = Int32.Parse(data[2]);
            for (int i = 0; i < amountToAdd; i++)
            {
                dataSection.AddDataToMemory(0, ++memoryLocation);
            }
            return memoryLocation;
        }
    }
}
