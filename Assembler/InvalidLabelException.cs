using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Assembler
{
    class InvalidLabelException : Exception, ISerializable
    {
        private string DEFAULT_MESSAGE = "A label in the assembly file is invalid.";

        public new string Message { get; set; }
        public new Exception InnerException { get; set; }

        public InvalidLabelException()
        {
            this.Message = DEFAULT_MESSAGE;
        }
        public InvalidLabelException(string message)
        {
            this.Message = message;
        }

        public InvalidLabelException(string message, Exception inner)
        {
            this.Message = message;
            this.InnerException = inner;
        }

        protected InvalidLabelException(SerializationInfo info, StreamingContext context)
        {
            
        }
    }
}
