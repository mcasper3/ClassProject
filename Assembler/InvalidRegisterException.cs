using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Assembler
{
    class InvalidRegisterException : Exception, ISerializable
    {
        private string DEFAULT_MESSAGE = "A register in the assembly file is invalid.\r\n"
            + "Registers must be formatted as $ra with 0 <= a <= 15.";

        public new string Message { get; set; }
        public new Exception InnerException { get; set; }

        public InvalidRegisterException()
        {
            this.Message = DEFAULT_MESSAGE;
        }
        public InvalidRegisterException(string message)
        {
            this.Message = message;
        }

        public InvalidRegisterException(string message, Exception inner)
        {
            this.Message = message;
            this.InnerException = inner;
        }

        protected InvalidRegisterException(SerializationInfo info, StreamingContext context)
        {
            
        }
    }
}
