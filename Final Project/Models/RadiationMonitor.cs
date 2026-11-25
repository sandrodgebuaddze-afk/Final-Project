using Final_Project.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Final_Project
{

    public class RadiationMonitor : PlantComponent
    {

        public double SievertsPerHour { get; set; }

        public RadiationMonitor(string name, double sieverts) : base(name)
        {
            SievertsPerHour = sieverts;
        }

        public override string GetDetails()
        {
            return $"{base.GetDetails()} | Radiation Level: {SievertsPerHour} mSv/h (Max: 0.1 mSv/h)";
        }

        public override string GetSafetyProblem()
        {
            if (SievertsPerHour > 0.1)
            {
                return $"Radiation dose rate reached {SievertsPerHour} mSv/h which exceeds the 0.1 mSv/h safe threshold.";
            }
            return null;
        }
    }
}