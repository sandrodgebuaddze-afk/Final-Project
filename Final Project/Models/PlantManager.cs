using Final_Project.Enums;
using Final_Project.Exceptions;
using Spectre.Console;

namespace Final_Project.Models
{
    public class PlantManager
    {
        private List<PlantComponent> _components = new List<PlantComponent>();

        private readonly string logPath = "log.txt";

        public string[] Notes { get; set; } = Array.Empty<string>();

        public bool IsPlantShutDown { get; private set; } = false;

        public string ShutdownReason { get; private set; } = string.Empty;

        public void AddComponent(PlantComponent component)
        {
            if (IsPlantShutDown)
            {
                throw new PlantShutdownException("Plant is shut down. Cannot add components.");
            }

            _components.Add(component);
            Console.WriteLine($"Added component {component.Name} as {component.Id}.");
            CheckSafetyStatus();
        }
        public void RemoveComponent(string id)
        {
            if (IsPlantShutDown)
            {
                throw new PlantShutdownException("Plant is shut down. Cannot remove components.");
            }

            PlantComponent comp = SearchComponentById(id);
            _components.Remove(comp);
            Console.WriteLine($"Removed component {comp.Name} ({comp.Id}).");
        }


        public void DisplayAllComponents()
        {
            if (_components.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No components in the system.[/]");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("ID")
                .AddColumn("Name")
                .AddColumn("Type")
                .AddColumn("Status")
                .AddColumn("Details");

            foreach (var comp in _components)
            {
                string statusText = comp.Status == ComponentStatus.Shutdown
                    ? "[red]Shutdown[/]"
                    : $"[green]{comp.Status}[/]";

                table.AddRow(
                    comp.Id,
                    comp.Name,
                    comp.GetType().Name,
                    statusText,
                    comp.GetDetails()
                );
            }

            AnsiConsole.Write(table);

            if (Notes.Length > 0)
            {
                AnsiConsole.MarkupLine("\n[bold]Notes:[/]");
                foreach (string note in Notes)
                {
                    AnsiConsole.MarkupLine($"• {note}");
                }
            }
        }


        public PlantComponent SearchComponentById(string id)
        {
            foreach (var comp in _components)
            {
                if (comp.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    return comp;
                }
            }

            throw new NotFoundException($"Component with ID '{id}' was not found.");
        }

        private void CheckSafetyStatus()
        {
            if (IsPlantShutDown) return;

            foreach (var comp in _components)
            {
                string issue = comp.GetSafetyProblem();
                if (issue != null)
                {
                    TriggerEmergencyShutdown(issue);
                    break;
                }
            }
        }
        private void TriggerEmergencyShutdown(string reason)
        {
            IsPlantShutDown = true;
            ShutdownReason = reason;

            foreach (var comp in _components)
            {
                comp.Status = ComponentStatus.Shutdown;
            }

            Console.WriteLine("EMERGENCY SHUTDOWN TRIGGERED.");
            Console.WriteLine($"Reason: {reason}");
        }
        public void CreateLogFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logPath, append: false))
                {
                    writer.WriteLine($"Plant Log Report : {DateTime.Now}");
                    writer.WriteLine($"Status: {(IsPlantShutDown ? "SHUT DOWN" : "OPERATIONAL")}");
                    if (IsPlantShutDown)
                    {
                        writer.WriteLine($"Shutdown Cause: {ShutdownReason}");
                    }
                    foreach (var comp in _components)
                    {
                        writer.WriteLine(comp.GetDetails());
                    }
                }
                Console.WriteLine("Log file saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing file: {ex.Message}");
            }
        }

        public void AppendToLogFile(string note)
        {
            if (!File.Exists(logPath))
            {
                Console.WriteLine("Cannot append. Save the log file first.");
                return;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(logPath, append: true))
                {
                    writer.WriteLine($"[{DateTime.Now}] Note: {note}");
                }
                Notes = Notes.Append(note).ToArray();
                Console.WriteLine("Note appended to log file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error appending file: {ex.Message}");
            }
        }
        public void DeleteLogFile()
        {
            try
            {
                if (File.Exists(logPath))
                {
                    File.Delete(logPath);
                    Console.WriteLine("Log file deleted successfully.");
                }
                else
                {
                    Console.WriteLine("No log file found to delete.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
            }
        }
    }
}