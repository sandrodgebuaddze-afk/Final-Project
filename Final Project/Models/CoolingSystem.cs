namespace Final_Project.Models
{

    public class CoolingSystem : PlantComponent
    {
        public double Pressure { get; set; }


        public CoolingSystem(string name, double pressure) : base(name)
        {
            Pressure = pressure;
        }
        public override string GetDetails()
        {
            return $"{base.GetDetails()} | System Pressure: {Pressure} bar (Safe Zone: 140.0-170.0 bar)";
        }
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