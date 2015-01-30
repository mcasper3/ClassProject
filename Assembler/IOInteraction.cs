using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assembler
{
    class IOInteraction
    {

        public void ConvertFile()
        {
            bool done = false;
            do
            {
                try
                {
                    CreateMIFFromAssembly();
                    Console.WriteLine("The file has been converted.\r\n");
                }
                catch (InvalidInstructionException e)
                {
                    Console.WriteLine("An error occured while attempting to convert the file:");
                    Console.WriteLine(e.Message + "\r\n");
                }
                catch (InvalidLabelException e)
                {
                    Console.WriteLine("An error occured while attempting to convert the file:");
                    Console.WriteLine(e.Message + "\r\n");
                }
                catch (InvalidRegisterException e)
                {
                    Console.WriteLine("An error occured while attempting to convert the file:");
                    Console.WriteLine(e.Message + "\r\n");
                }
                catch (Exception)
                {
                    
                    Console.WriteLine("An error occured while attempting to convert the file:");
                    Console.WriteLine("The file contained an invalid instruction.\r\n");
                }

                done = RequestIfDone();
                Console.WriteLine();
            } while (!done);

            Console.WriteLine("Thank you for using Group 2's Assembler and remember to always pick number three m'lord");
            Console.Write("\r\nPress enter to end program...");
            Console.ReadLine();
        }

        private void CreateMIFFromAssembly()
        {
            string inputFileName = RequestFileName();
            string[] assemblyInstructions = ReadFile(inputFileName);

            if (assemblyInstructions != null)
            {
                CreateMIFFile(assemblyInstructions);
            }
        }

        private void CreateMIFFile(string[] assemblyInstructions)
        {
            InstructionTransformer instructTransf = new InstructionTransformer();
            Instruction[] binaryInstructions = instructTransf.ConvertInstructions(assemblyInstructions);
            int memoryAddress = 1;
            string nextLine = "";

            System.Console.WriteLine("\r\nPlease enter the desired name of the .mif to be created.");
            string filename = System.Console.ReadLine();
            Console.WriteLine("\r\nProcessing file...\r\n");

            if (!filename.EndsWith(".mif"))
            {
                filename += ".mif";
            }

            string mifText = "WIDTH=24;\r\nDEPTH=1024;\r\n\r\nADDRESS_RADIX=UNS;\r\nDATA_RADIX=HEX;\r\n\r\nCONTENT BEGIN\r\n\t0\t\t\t:\t000000;";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
            {
                file.WriteLine(mifText);

                foreach (Instruction i in binaryInstructions)
                {
                    nextLine = "\t" + memoryAddress + "\t\t\t:\t" + i.ToString() + ";";
                    memoryAddress++;
                    file.WriteLine(nextLine);
                }

                if (instructTransf.IsDataSectionLocated())
                {
                    nextLine = instructTransf.ConvertDataSectionToString();
                    memoryAddress += instructTransf.GetSizeOfDataSection();
                    file.Write(nextLine);
                }

                nextLine = "\t[" + memoryAddress + "..1023]\t:\t000000;\r\nEND;";
                file.WriteLine(nextLine);
            }
        }

        private string RequestFileName()
        {
            bool isValidFile = true;
            string filename = "";
            Console.Clear();
            System.Console.WriteLine("Please enter the name of the assembly file (.asm) you wish to use");
            do
            {
                filename = System.Console.ReadLine();
                if (!filename.EndsWith(".asm") && !filename.EndsWith(".s"))
                {
                    filename += ".asm";
                }

                isValidFile = ValidateFile(filename);

                if (!isValidFile)
                {
                    System.Console.WriteLine("Please enter the name of a valid assembly file");
                }
            } while (!isValidFile);
            return filename;
        }

        private string[] ReadFile(string filename)
        {
            string[] assemblyInstructions = null;
            try
            {
                assemblyInstructions = File.ReadAllLines(filename);
            }
            catch (Exception)
            {
                System.Console.WriteLine("Could not read the file + \"" + filename + "\"");
            }
            return assemblyInstructions;
        }

        private bool ValidateFile(string filename)
        {
            return File.Exists(filename);
        }

        public void PrintWelcomeMessage()
        {
            Console.WriteLine("*                                                                                       *"
                            + "\r\n*                         Welcome to Group 2's (Pick Number 3 M'lord)                   *"
                            + "\r\n*                                       Assembler                                       *"
                            + "\r\n*                                                                                       *"
                            + "\r\n*****************************************************************************************\r\n");
        }

        private bool RequestIfDone()
        {
            Console.WriteLine("Would you like to convert another file? (y/n)");
            bool notValid = false;
            bool isDone = true;
            do
            {
                string choice = Console.ReadLine();
                if (choice.ToLower().Equals("y"))
                {
                    isDone = false;
                    notValid = false;
                }
                else if (choice.ToLower().Equals("n"))
                {
                    isDone = true;
                    notValid = false;
                }
                else
                {
                    Console.WriteLine("Please enter either y or n for your selection.");
                    notValid = true;
                }
            } while (notValid);

            return isDone;
        }

        public void RunMainMenu()
        {
            PrintWelcomeMessage();
            PrintMainMenu();
        }

        private void PrintMainMenu()
        {
            bool exit = false;
            do
            {
                Console.WriteLine("Please select one of the following options by entering the corresponding number:");
                Console.WriteLine("1: View a sample assembly file detailing correct syntax");
                Console.WriteLine("2: Pick number three m'lord!");
                Console.WriteLine("3: Convert an .asm file to a .mif file");
                Console.WriteLine("4: Exit\r\n");
                int choice = DetermineMenuChoice();
                switch (choice)
                {
                    case 1: PrintSampleFile();
                        break;
                    case 2: PrintPickThree();
                        break;
                    case 3: ConvertFile();
                        exit = true;
                        break;
                    case 4: exit = true;
                        break;
                }
            } while (!exit);
        }

        private int DetermineMenuChoice()
        {
            int choice = 0;
            bool validChoice = true;
            do
            {
                string num = Console.ReadLine();
                try
                {
                    choice = int.Parse(num);
                    if (choice != 1 && choice != 2 && choice != 3 && choice != 4)
                    {
                        throw new FormatException();
                    }
                    validChoice = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("\r\nThe entered choice was not a valid option.");
                    Console.WriteLine("Please enter either 1, 2, 3, or 4.\r\n");
                    validChoice = false;
                }
            } while (!validChoice);
            return choice;
        }

        private void PrintPickThree()
        {
            Console.Clear();
            string[] message = File.ReadAllLines("Needed Files\\PickThree.txt");
            foreach (string s in message)
            {
                Console.WriteLine(s);
            }
        }

        private void PrintSampleFile()
        {
            Console.Clear();
            string[] message = File.ReadAllLines("Needed Files\\ExampleFile.txt");
            foreach (string s in message)
            {
                Console.WriteLine(s);
            }
            Console.Write("Press enter to return to the main menu...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
