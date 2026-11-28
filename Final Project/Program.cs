using Final_Project.Exceptions;
using Final_Project.Models;
using Spectre.Console;

namespace Final_Project
{
    class Program
    {
        static void Main()
        {
            string usersPath = "users.txt";

            AnsiConsole.Write(
                new FigletText("NUCLEAR FISSION POWER PLANT")
                    .Centered()
                    .Color(Color.Green));

            if (!File.Exists(usersPath))
            {
                List<User> defaultUsers = new List<User>()
                {
                    new User(1, "Giorgi Beridze", "giorgi@gmail.com", "Giorgi123"),
                    new User(2, "Nika Kapanadze", "nika@gmail.com", "Nika12345"),
                    new User(3, "Ana Maisuradze", "ana@gmail.com", "Ana12345")
                };
                SaveUsers(usersPath, defaultUsers);
            }

            List<User> users = LoadUsers(usersPath);

            User currentUser = null;
            bool wantToContinue = true;

            do
            {
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Renew Password");
                Console.WriteLine("4. Exit");
                Console.Write("Option: ");
                string authChoice = Console.ReadLine();

                switch (authChoice)
                {
                    case "1":
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
                            Console.WriteLine("Try again? (y/n)");
                            string userAnswer = Console.ReadLine();

                            if (string.Equals(userAnswer, "y"))
                            {
                                wantToContinue = true;
                            }
                            else
                            {
                                wantToContinue = false;
                            }
                        }
                        break;

                    case "2":
                        RegisterUser(usersPath, users);
                        break;

                    case "3":
                        RenewPassword(usersPath, users);
                        break;

                    case "4":
                        wantToContinue = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
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
                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("\n[bold yellow]Select an option:[/]")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "Add component",
                            "View all",
                            "Search ID",
                            "Remove component",
                            "Save file",
                            "Add note",
                            "Delete file",
                            "Exit"
                        }));

                switch (choice)
                {
                    case "Add component":
                        try { AddComponent(manager); }
                        catch (PlantShutdownException ex) { Console.WriteLine(ex.Message); }
                        break;
                    case "View all":
                        manager.DisplayAllComponents();
                        break;
                    case "Search ID":
                        try { SearchComponent(manager); }
                        catch (NotFoundException ex) { Console.WriteLine(ex.Message); }
                        break;
                    case "Remove component":
                        try { RemoveComponent(manager); }
                        catch (PlantShutdownException ex) { Console.WriteLine(ex.Message); }
                        catch (NotFoundException ex) { Console.WriteLine(ex.Message); }
                        break;
                    case "Save file":
                        manager.CreateLogFile();
                        break;
                    case "Add note":
                        string note = AnsiConsole.Ask<string>("Note:");
                        manager.AppendToLogFile(note);
                        break;
                    case "Delete file":
                        manager.DeleteLogFile();
                        break;
                    case "Exit":
                        running = false;
                        break;
                }
            }
        }

        private static void RegisterUser(string usersPath, List<User> users)
        {
            Console.Write("Full Name: ");
            string fullName = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("All fields are required.");
                return;
            }

            if (password.Length < 8)
            {
                Console.WriteLine("Password must be at least 8 characters long.");
                return;
            }

            if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("A user with this email already exists.");
                return;
            }

            int nextId = 1;
            if (users.Count > 0)
            {
                nextId = users.Max(u => u.Id) + 1;
            }

            User newUser = new User(nextId, fullName, email, password);
            users.Add(newUser);
            SaveUsers(usersPath, users);

            Console.WriteLine("Registration successful! You can now log in.");
        }

        private static void RenewPassword(string usersPath, List<User> users)
        {
            Console.Write("Enter your Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter current Password: ");
            string currentPassword = Console.ReadLine();

            User user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Password == currentPassword);

            if (user == null)
            {
                Console.WriteLine("Invalid email or current password.");
                return;
            }

            Console.Write("Enter new Password: ");
            string newPassword = Console.ReadLine();

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
            {
                Console.WriteLine("New password must be at least 8 characters long.");
                return;
            }

            if (newPassword == currentPassword)
            {
                Console.WriteLine("New password must be different from the old password.");
                return;
            }

            user.Password = newPassword;
            SaveUsers(usersPath, users);

            Console.WriteLine("Password renewed successfully!");
        }

        private static List<User> LoadUsers(string path)
        {
            List<User> users = new List<User>();
            using (StreamReader reader = new StreamReader(path))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split("|");
                    int.TryParse(parts[0], out int id);
                    users.Add(new User(id, parts[1], parts[2], parts[3]));
                }
            }
            return users;
        }

        private static void SaveUsers(string path, List<User> users)
        {
            using (StreamWriter writer = new StreamWriter(path, append: false))
            {
                foreach (User user in users)
                {
                    writer.WriteLine(user);
                }
            }
        }

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