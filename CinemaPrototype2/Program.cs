﻿using Spectre.Console;
using System.Globalization;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("С началом работы! Вы находитесь в режиме администратора для ввода данных.");
        StartAdministratorInterface();
    }


    static void StartAdministratorInterface()
    {
        Console.WriteLine("Введите количество фильмов:"); // получаем информацию по фильмам
        int filmNum = GetPositiveInt();
        for (int i = 0; i < filmNum; i++)
        {
            Console.WriteLine($"Введите данные для зала {i + 1}.");
            AddNewFilm();
        }

        Console.WriteLine("Введите число залов:"); // получаем информацию о залах
        int hallsNum = GetPositiveInt();
        for (int i = 0; i < hallsNum; i++)
        {
            Console.WriteLine($"Введите данные для зала {i + 1}.");
            AddNewHall();
        }

        foreach (Film film in Film.all)
        {
            string answer;
            do
            {
                AddNewScreening(film);
                AnsiConsole.Write(new Markup($"Хотите добавить еще сеанс для фильма {film.name}?\n"));
                answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                            .AddChoice("да")
                                            .AddChoice("нет")
                                            .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.\n"));
            } while (answer == "да");
        } // добавляем сеансы для каждого фильма

    }

    static int GetPositiveInt()
    {
        while (true)
        {
            string inputNum = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            int num; bool successfullyParsed = int.TryParse(inputNum, out num);
            if (successfullyParsed && num > 0)
                return num;
            else
                Console.WriteLine("Неверное значение. Повторите попытку.");
        }
    }
    static void AddNewFilm()
    {
        Film newFilm;
        newFilm = new Film();
        newFilm.SetName();
        newFilm.SetAgeRestriction();
        newFilm.SetLanguage();
        Film.all.Add(newFilm);
    }
    static void AddNewHall()
    {
        Hall newHall;
        newHall = new Hall();
        newHall.SetName();
        Console.WriteLine("Введите число рядов в зале:");
        int rows = GetPositiveInt(); newHall.rowsNum = rows;
        Console.WriteLine("Введите число мест в одном ряду:");
        int seats = GetPositiveInt(); newHall.seatsInRowNum = seats;
        newHall.SetType();
        Hall.all.Add(newHall);
    }
    static void AddNewScreening(Film currFilm)
    {
        Console.WriteLine($"Выберите зал для показа фильма {currFilm.name}");

        bool validChoice = false;
        string chosenHallName = "";
        while (!validChoice)
        {
            TextPrompt<string> hallChoicePrompt = new TextPrompt<string>("");
            foreach (Hall hall in Hall.all)
                hallChoicePrompt.AddChoice(hall.name);
            string chHall = AnsiConsole.Prompt(hallChoicePrompt);

            bool isOkayToChoose = true;
            foreach (Film film in Film.all)
            {
                foreach (Hall hall in film.hallsData)
                {
                    if (hall.name == chHall)
                    {
                        isOkayToChoose = false;
                        break;
                    }
                }
            }

            if (isOkayToChoose)
            {
                chosenHallName = chHall;
                validChoice = true;
            }
            else
                Console.WriteLine("Выберите другой зал");
        }
        Hall chosenHall = Hall.GetHallByName(chosenHallName);

        Console.WriteLine($"Выберите время для показа фильма {currFilm.name} в зале {chosenHallName}.");
        DateTime showDate = GetDate();

        Screening newScreening = new Screening { film = currFilm, hall = chosenHall, time = showDate };
        newScreening.SetInitialAvailability();
        newScreening.SetInitialPrices();
    }

    static DateTime GetDate()
    {
        while (true)
        {
            Console.WriteLine("Введите дату в формате ДД/ММ/ГГГ ЧЧ:ММ");
            string dateString = Console.ReadLine();
            string format = "dd/MM/yyyy HH:mm";
            try
            {
                DateTime result = DateTime.ParseExact(dateString, format, CultureInfo.CurrentCulture);

                if (result > DateTime.Now)
                    return result;
                else
                    Console.WriteLine("Это время уже прошло. Повторите ввод.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Некорректный формат. Повторите ввод.");
            }
        }
    }

    class Administrator
    {
        private string hardcodedPassword = "12345";
    }


    class User
    {

    }


    class Hall
    {
        public static List<Hall> all = new List<Hall>();

        public string name = "";
        public int rowsNum;
        public int seatsInRowNum;
        public string type = "";

        public void SetName()
        {
            bool succeeded = false;
            Console.WriteLine($"Введите название зала:");
            while (!succeeded)
            {
                string inputName = AnsiConsole.Prompt(new TextPrompt<string>("> "));

                bool alreadyExists = false;
                foreach (Hall hall in Hall.all)
                {
                    if (inputName == hall.name)
                    {
                        Console.WriteLine("Данное название уже есть в базе.");
                        alreadyExists = true;
                        break;
                    }
                }

                if (inputName.Length >= 1 && !alreadyExists)
                {
                    name = inputName;
                    succeeded = true;
                }
                else
                    Console.WriteLine("Неверное значение для названия фильма. Повторите попытку.");
            }
        }
        public void SetType()
        {
            Console.WriteLine($"Введите тип зала {name}:");
            string typeCode = AnsiConsole.Prompt(new TextPrompt<string>("1 - стандартный, 2 - VIP")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .InvalidChoiceMessage("[red1]Введен неверный вариант. Пожалуйста, попробуйте еще раз.[/]"));
            Console.WriteLine();
            if (typeCode == "1")
                type = "стандартный";
            if (typeCode == "2")
                type = "VIP";
        }

        public static Hall GetHallByName(string someName)
        {
            foreach (Hall hall in Hall.all)
            {
                if (hall.name == someName)
                    return hall;
            }
            return new Hall(); // dummyHall
        }
    }

    class Film
    {
        public static List<Film> all = new List<Film>();

        public string name = "";
        public string ageRestriction = "";
        public string language = "";
        public List<Hall> hallsData = new List<Hall>();
        public List<Screening> screenings = new List<Screening>();

        public void SetName()
        {
            bool succeeded = false;
            Console.WriteLine($"Введите название фильма:");
            while (!succeeded)
            {
                string inputName = AnsiConsole.Prompt(new TextPrompt<string>("> "));

                bool alreadyExists = false;
                foreach (Film film in Film.all)
                {
                    if (inputName == film.name)
                    {
                        Console.WriteLine("Данный фильм уже есть в базе. Повторите попытку.");
                        alreadyExists = true;
                        break;
                    }

                }

                if (inputName.Length >= 1 && !alreadyExists)
                {
                    name = inputName;
                    succeeded = true;
                }
                else
                    Console.WriteLine("Неверное значение для названия фильма. Повторите попытку.");
            }
        }
        public void SetAgeRestriction()
        {
            Console.WriteLine($"Введите возрастное ограничение для фильма {name}:");
            string ageRest = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                            .AddChoice("0+")
                                                            .AddChoice("6+")
                                                            .AddChoice("12+")
                                                            .AddChoice("16+")
                                                            .AddChoice("18+")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            ageRestriction = ageRest;
        }
        public void SetLanguage()
        {
            Console.WriteLine($"Введите язык ддя фильма {name}:");
            string langCode = AnsiConsole.Prompt(new TextPrompt<string>("1 - русский, 2 - английский")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            Console.WriteLine();
            if (langCode == "1")
                language = "русский";
            if (langCode == "2")
                language = "английский";
        }


    }

    class Screening
    {
        public Film film;
        public Hall hall;
        public DateTime time;

        public List<List<char>> seatsAvailability = new List<List<char>>();
        public List<List<int>> priceData = new List<List<int>>(); // матрица с ценами на места

        public void SetInitialAvailability()
        {
            for (int i = 0; i < hall.rowsNum; i++)
            {
                List<char> row = new List<char>();
                for (int j = 0; j < hall.seatsInRowNum; j++)
                    row.Add('0');
                seatsAvailability.Add(row);
            }
        }
        public void SetInitialPrices()
        {
            AnsiConsole.Write(new Markup("\nВведите стоимость билетов в зале через пробел:[/]\n"));

            for (int i = 0; i < hall.rowsNum; i++)
            {
                List<int> rowPrices = new List<int>();
                bool arePricesValid = false;
                string[] validPrices = new string[hall.seatsInRowNum];

                do
                {
                    string[] rawRowPrices = AnsiConsole.Prompt(new TextPrompt<string>("Ряд " + Convert.ToString(i + 1) + "> ")).Split(' ');
                    try
                    {
                        if (rawRowPrices.Length != hall.seatsInRowNum)
                        {
                            AnsiConsole.Write(new Markup("Неверные значения для цен. Повторите попытку.[/]\n"));
                            continue;
                        }
                        foreach (var strPrice in rawRowPrices)
                        {
                            int intPrice = int.Parse(strPrice);
                            if (intPrice < 0)
                            {
                                AnsiConsole.Write(new Markup("Неверные значения для цен. Повторите попытку.[/]\n"));
                                continue;
                            }
                        }
                        validPrices = rawRowPrices;
                        arePricesValid = true;
                    }
                    catch (Exception)
                    {
                        AnsiConsole.Write(new Markup("[red1]Неверные значения для цен. Повторите попытку.[/]\n"));
                    }
                } while (arePricesValid == false);

                // сохраняю цены в матрицу
                foreach (var item in validPrices)
                    rowPrices.Add(int.Parse(item));
                priceData.Add(rowPrices);
            }
        }
    }
}