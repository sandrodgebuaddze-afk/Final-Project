using System;
using Final_Project.Exceptions;
using Final_Project.Models;

namespace Final_Project
{
    class Program
    {
        static void Main()
        {
            string usersPath = "users.txt";

            List<User> defaultUsers = new List<User>()
            {
                new User(1, "Giorgi Beridze", "giorgi@gmail.com", "Giorgi123"),
                new User(2, "Nika Kapanadze", "nika@gmail.com", "Nika12345"),
                new User(3, "Ana Maisuradze", "ana@gmail.com", "Ana12345")
            };
            using (StreamWriter writer = new StreamWriter(usersPath, append: false))
            {
                foreach (User user in defaultUsers)
                {
                    writer.WriteLine(user);
                }
            }

            List<User> users = new List<User>();

            using (StreamReader reader = new StreamReader(usersPath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split("|");
                    int.TryParse(parts[0], out int id);
                    users.Add(new User(id, parts[1], parts[2], parts[3]));
                }
            }
            User currentUser = null;
            bool wantToContinue = true;

            do
            {
                Console.Write("Email: ");
                string email = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();

                currentUser = users.FirstOrDefault(user => user.Email == email && user.Password == password);

                if (currentUser != null)
                {
                    wantToContinue = false;
                }
                else
                {
                    Console.WriteLine("Wrong email or password.");
                    Console.WriteLine("Try again? (yes/no)");
                    string userAnswer = Console.ReadLine();

                    if (string.Equals(userAnswer, "yes", StringComparison.OrdinalIgnoreCase))
                    {
                        wantToContinue = true;
                    }
                    else
                    {
                        wantToContinue = false;
                    }
                }
            } while (wantToContinue);

            if (currentUser == null)
            {
                Console.WriteLine("Access denied.");
                return;
            }

            Console.WriteLine($"Welcome, {currentUser.Fullname}!");

            PlantManager manager = new PlantManager();
            bool running = true;

            while (running)
            {
                Console.WriteLine("1. Add component");
                Console.WriteLine("2. View all");
                Console.WriteLine("3. Search ID");
                Console.WriteLine("4. Remove component");
                Console.WriteLine("5. Save file");
                Console.WriteLine("6. Add note");
                Console.WriteLine("7. Delete file");
                Console.WriteLine("8. Exit");
                Console.Write("Option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        try
                        {
                            AddComponent(manager);
                        }
                        catch (PlantShutdownException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "2":
                        manager.DisplayAllComponents();
                        break;
                    case "3":
                        try
                        {
                            SearchComponent(manager);
                        }
                        catch (NotFoundException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "4":
                        try
                        {
                            RemoveComponent(manager);
                        }
                        catch (PlantShutdownException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (NotFoundException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "5":
                        manager.CreateLogFile();
                        break;
                    case "6":
                        Console.Write("Note: ");
                        string note = Console.ReadLine();
                        manager.AppendToLogFile(note);
                        break;
                    case "7":
                        manager.DeleteLogFile();
                        break;
                    case "8":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        // amocmebs momkhmareblis sheyvanil monacemebs temperatura, radiacia, wneva da amatebs komponents tu sadguri mushaobs.
        private static void AddComponent(PlantManager manager)
        {
            if (manager.IsPlantShutDown)
            {
                throw new PlantShutdownException("Plant is shut down. Cannot add components.");
            }

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.WriteLine("1. Reactor Core (Safe limit: max 350.0 C)");
            Console.WriteLine("2. Radiation Monitor (Safe limit: max 0.1 mSv/h)");
            Console.WriteLine("3. Cooling System (Safe limit: 140.0 - 170.0 bar)");
            Console.Write("Type: ");
            string type = Console.ReadLine();

            if (type == "1")
            {
                Console.Write("Temperature (Safe limit: max 350.0 C): ");
                if (double.TryParse(Console.ReadLine(), out double temp) && temp >= 0)
                {
                    Console.Write("Power level 1-10: ");
                    if (int.TryParse(Console.ReadLine(), out int level) && level >= 1 && level <= 10)
                    {
                        manager.AddComponent(new ReactorCore(name, temp, level));
                    }
                    else
                    {
                        Console.WriteLine("Invalid power level.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid temperature.");
                }
            }
            else if (type == "2")
            {
                Console.Write("Radiation level (Safe limit: max 0.1 mSv/h): ");
                if (double.TryParse(Console.ReadLine(), out double rads) && rads >= 0)
                {
                    manager.AddComponent(new RadiationMonitor(name, rads));
                }
                else
                {
                    Console.WriteLine("Invalid radiation level.");
                }
            }
            else if (type == "3")
            {
                Console.Write("Pressure (Safe limit: 140.0 - 170.0 bar): ");
                if (double.TryParse(Console.ReadLine(), out double pressure) && pressure >= 0)
                {
                    manager.AddComponent(new CoolingSystem(name, pressure));
                }
                else
                {
                    Console.WriteLine("Invalid pressure.");
                }
            }
            else
            {
                Console.WriteLine("Invalid type choice.");
            }
        }
        private static void SearchComponent(PlantManager manager)
        {
            Console.Write("Component ID (in order of creation, starting from COMP-1): ");
            string id = Console.ReadLine();

            PlantComponent item = manager.SearchComponentById(id);
            Console.WriteLine(item.GetDetails());
        }
        private static void RemoveComponent(PlantManager manager)
        {
            Console.Write("Component ID to remove (in order of creation, starting from COMP-1): ");
            string id = Console.ReadLine();

            manager.RemoveComponent(id);
        }
    }
}
//The note from the notable noters note