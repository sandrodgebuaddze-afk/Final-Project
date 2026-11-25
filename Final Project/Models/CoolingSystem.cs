using System;
using System.Collections.Generic;
using System.Text;

namespace Final_Project.Models
{

    public class CoolingSystem : PlantComponent
    {
        public double Pressure { get; set; }


        public CoolingSystem(string name, double pressure) : base(name)
        {
            Pressure = pressure;
        }

        // ematebs wnevas da usafrtkhoo zonas
        public override string GetDetails()
        {
            return $"{base.GetDetails()} | System Pressure: {Pressure} bar (Safe Zone: 140.0-170.0 bar)";
        }

        // tu wneva 140 ze nakleabia an 170 ze metia abrunebs problemas, sxva shemtkhvevashi null
        public override string GetSafetyProblem()
        {
            if (Pressure < 140.0 || Pressure > 170.0)
            {
                return $"Cooling pressure reached {Pressure} bar which is outside the safe 140.0 - 170.0 bar range.";
            }
            return null;
        }
    }
}