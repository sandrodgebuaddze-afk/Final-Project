namespace Final_Project.Models
{
    public class ReactorCore : PlantComponent
    {
        private static Random _random = new Random();

        public double Temperature { get; set; }

        public int PowerLevel { get; set; }

        public double PowerOutput { get; set; }

        public int ScramChance { get; set; }
        public bool Scrammed { get; set; }
        public ReactorCore(string name, double temperature, int powerLevel) : base(name)
        {
            Temperature = temperature;
            PowerLevel = powerLevel;

            int minPower = powerLevel * 100 - 50;
            int maxPower = powerLevel * 100;

            PowerOutput = Math.Round(minPower + _random.NextDouble() * (maxPower - minPower), 1);

            ScramChance = powerLevel * 5;

            Scrammed = _random.Next(1, 101) <= ScramChance;
        }

        public override string GetDetails()
        {
            return $"{base.GetDetails()} | Temperature: {Temperature}°C (Max: 350.0°C) | Power Level: {PowerLevel} | Output: {PowerOutput} MW | Scram Risk: {ScramChance}%";
        }

        public override string GetSafetyProblem()
        {
            if (Temperature > 350.0)
            {
                return $"Core temperature reached {Temperature}°C which exceeds the 350.0°C safe threshold.";
            }

            if (Scrammed)
            {
                return $"Reactor scrammed at power level {PowerLevel} ({ScramChance}% scram risk).";
            }
            return null;
        }
    }
}