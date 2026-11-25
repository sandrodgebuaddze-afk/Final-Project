using System;
using System.Collections.Generic;
using System.Text;

namespace Final_Project.Exceptions
{
    // es shecdoma vagdebt roca sadguri ukve gamortulia da momkhmarebeli tsdilobs komponentis damatebas an washlas
    public class PlantShutdownException : Exception
    {
        public PlantShutdownException(string message) : base(message) { }
    }
}