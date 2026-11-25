using Final_Project.Enums;

namespace Final_Project.Models
{
    public abstract class PlantComponent
    {
        // es ricxvi qmnis ID ebs, erti aris yvela komponentistvis, 100 dan iwyeba
        private static int _globalIdCounter = 1;
        private string _id;
        private string _name;

        public string Id => _id;
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public ComponentStatus Status { get; set; } = ComponentStatus.Operational;

        protected PlantComponent(string name)
        {
            _globalIdCounter++;
            _id = $"COMP-{_globalIdCounter}";
            Name = name;
        }

        public virtual string GetDetails()
        {
            return $"ID: {Id} | Name: {Name} | Status: {Status}";
        }
        public abstract string GetSafetyProblem();
    }
}