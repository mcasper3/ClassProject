using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Assembler
{
    class InvalidInstructionException : Exception, ISerializable
    {
        public new string Message { get; set; }
        public new Exception InnerException { get; set; }
        private string DEFAULT_MESSAGE = "An instruction in the assembly file is invalid.";


        public InvalidInstructionException()
        {
            this.Message = DEFAULT_MESSAGE;
        }
        public InvalidInstructionException(string message)
        {
            this.Message = message;
        }
        public InvalidInstructionException(string message, Exception inner)
        {
            this.Message = message;
            this.InnerException = inner;
        }

        protected InvalidInstructionException(SerializationInfo info, StreamingContext context)
        {

        }
    }
}
