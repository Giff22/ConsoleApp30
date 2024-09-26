using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class Activity
{
    public string Name { get; set; }
    public decimal Cost { get; set; }

    public Activity(string name, decimal cost)
    {
        Name = name;
        Cost = cost;
    }
}

public class TravelItinerary
{
    private string destination;
    private DateTime departureDate;
    private DateTime returnDate;
    private List<Activity> activities;

    public string Destination
    {
        get { return destination; }
        set { destination = value; }
    }

    public DateTime DepartureDate
    {
        get { return departureDate; }
    }

    public DateTime ReturnDate
    {
        get { return returnDate; }
    }

    public List<Activity> Activities
    {
        get { return activities; }
        set { activities = value; }
    }

    public TravelItinerary()
    {
        destination = string.Empty;
        departureDate = DateTime.MinValue;
        returnDate = DateTime.MinValue;
        activities = new List<Activity>();
    }

    public TravelItinerary(string destination, DateTime departureDate, DateTime returnDate)
    {
        this.destination = destination;
        SetDates(departureDate, returnDate);
        activities = new List<Activity>();
    }

    public void AddActivity(string name, decimal cost)
    {
        activities.Add(new Activity(name, cost));
    }

    public void RemoveActivity(string name)
    {
        var activityToRemove = activities.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (activityToRemove != null)
        {
            activities.Remove(activityToRemove);
        }
        else
        {
            Console.WriteLine("Активність не знайдена.");
        }
    }

    public decimal CalculateTotalCost()
    {
        return activities.Sum(a => a.Cost);
    }

    public void PrintItinerary()
    {
        Console.WriteLine($"Місце призначення: {destination}");
        Console.WriteLine($"Дата відправлення: {departureDate.ToShortDateString()}");
        Console.WriteLine($"Дата повернення: {returnDate.ToShortDateString()}");
        Console.WriteLine("Заплановані активності:");
        foreach (var activity in activities)
        {
            Console.WriteLine($" - {activity.Name}: {activity.Cost} грн");
        }
        Console.WriteLine($"Загальна вартість активностей: {CalculateTotalCost()} грн");
    }

    public void SetDates(DateTime departureDate, DateTime returnDate)
    {
        if (returnDate < departureDate)
        {
            throw new ArgumentException("Дата повернення не може бути раніше дати відправлення.");
        }
        this.departureDate = departureDate;
        this.returnDate = returnDate;
    }

    public void SaveItineraryToFile(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(destination);
            writer.WriteLine(departureDate);
            writer.WriteLine(returnDate);
            writer.WriteLine(string.Join(",", activities.Select(a => $"{a.Name}:{a.Cost}")));
        }
    }

    public static TravelItinerary LoadItineraryFromFile(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            string destination = reader.ReadLine();
            DateTime departureDate = DateTime.Parse(reader.ReadLine());
            DateTime returnDate = DateTime.Parse(reader.ReadLine());
            List<string> activitiesData = reader.ReadLine().Split(',').ToList();
            List<Activity> activities = activitiesData.Select(a =>
            {
                var parts = a.Split(':');
                return new Activity(parts[0], decimal.Parse(parts[1]));
            }).ToList();

            TravelItinerary itinerary = new TravelItinerary(destination, departureDate, returnDate);
            itinerary.Activities = activities;
            return itinerary;
        }
    }
}

class Program
{
    static void Main()
    {
        TravelItinerary itinerary = new TravelItinerary();

        while (true)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Ввести інформацію про подорож");
            Console.WriteLine("2. Додати активність");
            Console.WriteLine("3. Видалити активність");
            Console.WriteLine("4. Вивести інформацію про подорож");
            Console.WriteLine("5. Зберегти маршруту у файл");
            Console.WriteLine("6. Завантажити маршрут з файлу");
            Console.WriteLine("0. Вихід");
            Console.Write("Оберіть опцію: ");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.Write("Введіть місце призначення: ");
                    itinerary.Destination = Console.ReadLine();

                    // Введення дати відправлення
                    while (true)
                    {
                        Console.Write("Введіть дату відправлення (yyyy-mm-dd): ");
                        string departureInput = Console.ReadLine();
                        if (DateTime.TryParse(departureInput, out DateTime departureDate))
                        {
                            // Введення дати повернення
                            while (true)
                            {
                                Console.Write("Введіть дату повернення (yyyy-mm-dd): ");
                                string returnInput = Console.ReadLine();
                                if (DateTime.TryParse(returnInput, out DateTime returnDate))
                                {
                                    try
                                    {
                                        itinerary.SetDates(departureDate, returnDate);
                                        break; // Вихід з циклу дати повернення
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Помилка: Невірний формат дати, спробуйте ще раз.");
                                }
                            }
                            break; // Вихід з циклу дати відправлення
                        }
                        else
                        {
                            Console.WriteLine("Помилка: Невірний формат дати, спробуйте ще раз.");
                        }
                    }
                    break;

                case "2":
                    Console.Write("Введіть активність: ");
                    string activityName = Console.ReadLine();
                    Console.Write("Введіть вартість активності: ");
                    decimal activityCost;
                    while (!decimal.TryParse(Console.ReadLine(), out activityCost) || activityCost < 0)
                    {
                        Console.WriteLine("Помилка: Введіть дійсну позитивну вартість.");
                    }
                    itinerary.AddActivity(activityName, activityCost);
                    break;

                case "3":
                    Console.Write("Введіть назву активності для видалення: ");
                    string activityToRemove = Console.ReadLine();
                    itinerary.RemoveActivity(activityToRemove);
                    break;

                case "4":
                    itinerary.PrintItinerary();
                    break;

                case "5":
                    string folderPath = @"C:\Users\1\1lab2";
                    string filePath = Path.Combine(folderPath, "itinerary.txt");

                    itinerary.SaveItineraryToFile(filePath);
                    Console.WriteLine($"Маршрут збережено в {filePath}.");
                    break;

                case "6":
                    string folderPathToLoad = @"C:\Users\1\1lab2";
                    string filePathToLoad = Path.Combine(folderPathToLoad, "itinerary.txt");

                    if (File.Exists(filePathToLoad))
                    {
                        itinerary = TravelItinerary.LoadItineraryFromFile(filePathToLoad);
                        Console.WriteLine($"Маршрут завантажено з {filePathToLoad}.");
                    }
                    else
                    {
                        Console.WriteLine($"Файл не знайдено за шляхом: {filePathToLoad}.");
                    }
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Невірний вибір, спробуйте ще раз.");
                    break;
            }
        }
    }
}
