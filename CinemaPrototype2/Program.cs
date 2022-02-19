using Spectre.Console;
using System.Collections;
using System.Globalization;
class Program
{
    public static void Main(string[] args)
    {
        Administrator admin = new Administrator();
        Console.WriteLine("С началом работы! Вы находитесь в режиме администратора для ввода данных.");
        StartAdministratorInterface();

        while (true)
        {
            // выбираем интерфейс для работы
            AnsiConsole.Write(new Markup("\nВы находитесь в меню выбора интерфейса\n"));
            AnsiConsole.Write(new Markup("Чтобы перейти к интерфейсу администратора, введите '1'\nЧтобы перейти к интерфейсу пользователя, введите '2'.\nЧтобы завершить работу, введите '3'.\n"));
            string command = AnsiConsole.Prompt(new TextPrompt<string>("")
                                            .AddChoice("1")
                                            .AddChoice("2")
                                            .AddChoice("3")
                                            .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.\n"));

            if (command == "1")
            {
                bool needsToReturn = admin.Check_Password();
                Console.WriteLine();

                if (needsToReturn)
                    continue;

                ExtraAdministratorInterface();
            }

            else if (command == "2")
                UserInterface();

            else if (command == "3")
            {
                Console.WriteLine("Завершаю работу...");
                break;
            }
        }
        Console.ReadKey();
    }


    static void StartAdministratorInterface()
    {
        Console.WriteLine("Введите количество фильмов:"); // получаем информацию по фильмам
        int filmNum = GetPositiveInt();
        for (int i = 0; i < filmNum; i++)
        {
            Console.WriteLine($"\nВведите данные для фильма {i + 1}.");
            Administrator.AddNewFilm();
        }

        Console.WriteLine("\nВведите число залов:"); // получаем информацию о залах
        int hallsNum;
        while (true)
        {
            hallsNum = GetPositiveInt();
            if (hallsNum < filmNum)
                Console.WriteLine("Залов должно быть не меньше, чем фильмов. Повторите попытку.");
            else
                break;
        }

        for (int i = 0; i < hallsNum; i++)
        {
            Console.WriteLine($"\nВведите данные для зала {i + 1}.");
            Administrator.AddNewHall();
        }

        foreach (Film film in Film.all)
        {
            string answer;
            do
            {
                bool success = Administrator.AddNewScreening(film);
                AnsiConsole.Write(new Markup($"\nХотите добавить еще сеанс для фильма {film.name}?\n"));
                answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                            .AddChoice("да")
                                            .AddChoice("нет")
                                            .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
            } while (answer == "да");
        } // добавляем сеансы для каждого фильма

    }
    static void UserInterface() 
    {

        User currUser = new User();
        currUser.SetUsername();
        Console.WriteLine("Введите начальный баланс.");
        int initBalance = GetPositiveInt();
        currUser.balance = initBalance;

        while (true)
        {
            Console.WriteLine("\nВы находитесь в меню пользователя");
            Console.WriteLine("1 - приобрести билеты,");
            Console.WriteLine("2 - вернуть билеты");
            Console.WriteLine("3 - показать все купленные билеты");
            Console.WriteLine("4 - выйти из меню пользователя и вернуться к меню выбора интерфейса.");
            string command = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .AddChoice("3")
                                                            .AddChoice("4")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            Console.WriteLine();

            if (command == "1")
                currUser.MakeOrder();

            else if (command == "2")
            {
                //ReturnTickets();
            }

            else if (command == "3")
                currUser.PrintAllTickets();

            else if (command == "4")
                return;
        }

    } // ВОЗВРАТ БИЛЕТОВ НЕ РЕАЛИЗОВАН
    static void ExtraAdministratorInterface()
    {
        while (true)
        {
            Console.WriteLine("\nВы находитесь в меню администратора\n1 - посмотреть аналитику по продажам;\n2 - посмотреть загруженность залов;\n3 - посмотреть клиентскую аналитику;\n4 - изменить данные о кинотеатре;\n5 - выйти из аккаунта администратора и вернуться к меню выбора интерфейса.");;
            string command = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .AddChoice("3")
                                                            .AddChoice("4")
                                                            .AddChoice("5")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (command == "1") // аналитика по продажам
                Administrator.GetSalesStatistics();
            else if (command == "2")
                Administrator.TrackHallLoad();

            else if (command == "3") // клиентская аналитика
                Administrator.GetClientAnalytics();

            else if (command == "4") // изменение данных
                Administrator.EditCinemaData();

            else if (command == "5")
                return;
        }
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
    static DateTime GetDateAndTime()
    {
        while (true)
        {
            Console.WriteLine("Введите дату в формате ДД.ММ.ГГГГ ЧЧ:ММ");
            string dateString = Console.ReadLine();
            string format = "dd.MM.yyyy HH:mm";
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
    static DateOnly GetDate()
    {
        while (true)
        {
            Console.WriteLine("Введите дату в формате ДД.ММ.ГГ");
            Console.Write("> ");
            string inputTime = Console.ReadLine();
            string format = "dd.MM.yyyy";
            CultureInfo invariant = CultureInfo.InvariantCulture;
            DateTime dt;
            if (DateTime.TryParseExact(inputTime, format, invariant, DateTimeStyles.None, out dt))
                return DateOnly.FromDateTime(dt);
            else
                Console.WriteLine("Неверный формат. Повторите попытку.");
        }
    }
    static TimeOnly GetTime()
    {
        while (true)
        {
            Console.WriteLine("Введите время в формате ЧЧ:ММ");
            Console.Write("> ");
            string inputTime = Console.ReadLine();
            string format = "HH:mm";
            CultureInfo invariant = CultureInfo.InvariantCulture;
            DateTime dt;
            if (DateTime.TryParseExact(inputTime, format, invariant, DateTimeStyles.None, out dt))
                return TimeOnly.FromDateTime(dt);

            else
                Console.WriteLine("Неверный формат. Повторите попытку.");
        }
    }
    static DateOnly GetRandomDate(DateOnly start, DateOnly end)
    {
        DateTime startDT = start.ToDateTime(TimeOnly.Parse("00:00:00"));
        DateTime endDT = end.ToDateTime(TimeOnly.Parse("23:59:59"));
        Random gen = new Random();
        int range = (endDT - startDT).Days;
        return DateOnly.FromDateTime(startDT.AddDays(gen.Next(range)));
    }


    class Administrator
    {
        private string hardcodedPassword = "12345";

        public static void AddNewFilm()
        {
            Film newFilm;
            newFilm = new Film();
            newFilm.SetName();
            newFilm.SetAgeRestriction();
            newFilm.SetLanguage();
            Film.all.Add(newFilm);
        }
        public static void AddNewHall()
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
        public static bool AddNewScreening(Film currFilm)
        {
            bool isSuccessful = false;
            
            Console.WriteLine($"\nВыберите зал для показа фильма {currFilm.name}");

            bool validChoice = false;
            Hall chosenHall = new Hall();
            
            while (!validChoice)
            {
                Hall currHall = Hall.ChooseHall();

                bool isOkayToChoose = true;
                foreach (Film film in Film.all)
                {
                    foreach (Hall hall in film.halls)
                    {
                        if (hall.name == currHall.name && film.name != currFilm.name)
                        {
                            isOkayToChoose = false;
                            break;
                        }
                    }
                }

                if (isOkayToChoose)
                {
                    chosenHall = currHall;
                    validChoice = true;
                }
                else
                {
                    Console.WriteLine("Выберите другой зал, этот уже зарезервирован для другого фильма.");
                    AnsiConsole.Write(new Markup("Хотите продолжить выбор зала?\n"));
                    string answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    Console.WriteLine();
                    if (answer == "нет")
                        return isSuccessful;
                }
            }
            currFilm.halls.Add(chosenHall);

            Console.WriteLine($"\nВыберите время для показа фильма {currFilm.name} в зале {chosenHall.name}.");

            DateTime showDate = GetDateAndTime();
            foreach (Screening screening in currFilm.screenings)
            {
                if (screening.hall == chosenHall && screening.time == showDate)
                {
                    Console.WriteLine("Данный сеанс уже есть в базе. В добавлении отказано.");
                    isSuccessful = true;
                    return isSuccessful;
                }
            }
            Screening newScreening = new Screening { film = currFilm, hall = chosenHall, time = showDate };
            newScreening.SetInitialAvailability();
            newScreening.SetInitialPrices();
            currFilm.screenings.Add(newScreening);
            isSuccessful = true;
            return isSuccessful;
        }
        public bool Check_Password() // проверка пароля для входа в интерфейс администратора
        {
            bool isAllowed = false;
            bool isReturnNeeded = false;
            AnsiConsole.Write(new Markup("\nВведите пароль. Чтобы вернуться к меню выбора интерфейса, введите 'и'\n"));

            do
            {
                string pwd = AnsiConsole.Prompt(new TextPrompt<string>("> ").Secret());
                if (pwd == hardcodedPassword)
                    isAllowed = true;
                else if (pwd == "и")
                {
                    isReturnNeeded = true;
                    break;
                }
                else
                    AnsiConsole.Write(new Markup("Ошибка: неверный пароль. Повторите ввод.\n"));
            } while (!isAllowed);

            return isReturnNeeded;
        }
        public static void EditCinemaData()
        {
            while (true)
            {
                Console.WriteLine("\nВы находитесь в режиме изменения данных о кинотеатре.");
                Console.WriteLine("1 - добавить данные;\n2 - изменить данные;\n3 - удалить данные;\n4 - выйти из режима изменения данных и вернуться к меню администратора.");
                string command = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3").AddChoice("4")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                if (command == "1")
                {
                    string answer = "да";
                    do
                    {
                        Console.WriteLine("\nВы находитесь режиме добавления данных о кинотеатре.");
                        Console.WriteLine("1 - добавить фильм;\n2 - добавить зал;\n3 - добавить сеанс;\n4-выйти из режима добавления данных и вернуться к режиму изменения данных.");
                        string editCommand = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3")
                                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        if (editCommand == "1")
                            AddNewFilm();

                        if (editCommand == "2")
                            AddNewHall();

                        if (editCommand == "3")
                        {
                            if (Film.all.Count == 0)
                            {
                                Console.WriteLine("В базе нет фильмов. Добавить сеанс к конкретному фильму невозможно.");
                                continue;
                            }
                            
                            Film filmAddScreeningFor = Film.ChooseFilm("screening count not important");
                            AddNewScreening(filmAddScreeningFor);
                        }

                        if (editCommand == "4")
                            break;

                        AnsiConsole.Write(new Markup("\nХотите продолжить добавлять данные?\n"));
                        answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        Console.WriteLine();

                    } while (answer == "да");

                } // добавление данных

                else if (command == "2")
                {
                    string answer = "да";
                    do
                    {
                        Console.WriteLine("\nВы находитесь режиме изменения данных о кинотеатре.");
                        Console.WriteLine("1 - изменить информацию о фильме;\n2 - изменить информацию о зале;\n3 - изменить информацию о сеансе;\n4 - выйти из режима изменения данных и вернуться к меню.");
                        string editCommand = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3").AddChoice("4")
                                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        if (editCommand == "1")
                        {
                            if (Film.all.Count == 0)
                            {
                                Console.WriteLine("В базе нет фильмов. Изменить данные о конкретном фильме невозможно.");
                                continue;
                            }
                            
                            Film filmToEdit = Film.ChooseFilm("screening count not important");
                            bool canBeEdited = filmToEdit.CanBeEdited();
                            if (!canBeEdited)
                            {
                                Console.WriteLine("На этот фильм уже были куплены билеты. В изменении данных отказано.");
                                continue;
                            }
                            
                            Console.WriteLine("\nВыберите действие.\n1 - изменить название;\n2 - изменить возрастное ограничение;\n3 - изменить язык.");
                            string action = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3")
                                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));

                            if (action == "1")
                                filmToEdit.SetName();

                            if (action == "2")
                                filmToEdit.SetAgeRestriction();

                            if (action == "3")
                                filmToEdit.SetLanguage();

                        } // изменить информацию о фильме
                        if (editCommand == "2")
                        {
                            if (Hall.all.Count == 0)
                            {
                                Console.WriteLine("В базе нет залов. Изменить данные о конкретном зале невозможно.");
                                continue;
                            }

                            Hall chosenHall = Hall.ChooseHall();
                            bool canBeEdited = chosenHall.CanBeEdited();
                            if (!canBeEdited)
                            {
                                Console.WriteLine("На сеансы в этом зале уже были куплены билеты. В изменении данных отказано.");
                                continue;
                            }

                            Console.WriteLine("\nВыберите действие.\n1 - изменить название;\n2 - изменить тип;\n");
                            string action = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2")
                                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                            if (action == "1")
                                chosenHall.SetName();
                            if (action == "2")
                                chosenHall.SetType();

                        } // изменить информацию о зале
                        if (editCommand == "3")
                        {
                            int totalScreeningCount = 0;
                            foreach (Film film in Film.all)
                                totalScreeningCount = totalScreeningCount + film.screenings.Count;

                            if (totalScreeningCount == 0)
                            {
                                Console.WriteLine("Доступных сеансов нет.");
                                continue;
                            }

                            Console.WriteLine("\nВыберите фильм.");
                            Film chosenFilm = Film.ChooseFilm("screening count important");
                            Screening chosenScreening = chosenFilm.ChooseScreening();

                            Console.WriteLine("\nВыберите действие.\n1 - пометить места как занятые (стоимость покупки приравнивается к нулю);\n2 - изменить цены;\n3 - изменить время");
                            string action = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3")
                                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                            if (action == "1")
                                chosenScreening.MarkSeatsAsTaken();

                            if (action == "2")
                            {
                                string reply = "да";
                                do
                                {
                                    bool isReturnNeeded = chosenScreening.ChangePrices();

                                    if (isReturnNeeded)
                                    {
                                        Console.WriteLine();
                                        break;
                                    }

                                    AnsiConsole.Write(new Markup("Хотите продолжить изменение цен?\n"));
                                    reply = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                                .AddChoice("да")
                                                                .AddChoice("нет")
                                                                .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.\n"));

                                } while (reply == "да");
                                Console.WriteLine();
                            }

                            if (action == "3")
                            {
                                bool canBeEdited = chosenScreening.CanTimeBeEdited();
                                if (!canBeEdited)
                                {
                                    Console.WriteLine("На этот сеанс уже были куплены билеты. В изменении данных отказано.");
                                    continue;
                                }
                                chosenScreening.time = GetDateAndTime();
                            }

                        } // изменить информацию о сеансе
                        if (editCommand == "4")
                            break;

                        AnsiConsole.Write(new Markup("\nХотите продолжить изменять данные?\n"));
                        answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        Console.WriteLine();

                    } while (answer == "да");

                } // изменение данных

                else if (command == "3")
                {
                    string answer = "да";

                    do
                    {
                        Console.WriteLine("\nВы находитесь режиме удаления данных.");
                        Console.WriteLine("1 - удалить фильм;\n2 - удалить зал;\n3 - удалить сеанс;\n4 - выйти из режима удаления данных и вернуться к режиму изменения данных.");
                        string deleteCommand = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3").AddChoice("4")
                                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));

                        if (deleteCommand == "1")
                        {
                            if (Film.all.Count == 0)
                            {
                                Console.WriteLine("В базе и так нет фильмов.");
                                continue;
                            }
                            
                            Film filmToDelete = Film.ChooseFilm("screening count not important");
                            bool canBeDeleted = filmToDelete.CanBeEdited();
                            if (!canBeDeleted)
                            {
                                Console.WriteLine("На этот фильм уже были куплены билеты. В удалении отказано.");
                                continue;
                            }

                            Film.all.Remove(filmToDelete);
                        } // удалить фильм

                        if (deleteCommand == "2")
                        {
                            if (Hall.all.Count == 0)
                            {
                                Console.WriteLine("В базе и так нет залов.");
                                continue;
                            }

                            Hall chosenHall = Hall.ChooseHall();
                            bool canBeDeleted = chosenHall.CanBeEdited();
                            if (!canBeDeleted)
                            {
                                Console.WriteLine("На сеансы в этом зале уже были куплены билеты. В удалении отказано.");
                                continue;
                            }

                            Hall.all.Remove(chosenHall);

                        } //удалить зал

                        if (deleteCommand == "3")
                        {
                            int totalScreeningCount = 0;
                            foreach (Film film in Film.all)
                                totalScreeningCount = totalScreeningCount + film.screenings.Count;

                            if (totalScreeningCount == 0)
                            {
                                Console.WriteLine("В базе и так нет сеансов.");
                                continue;
                            }

                            Console.WriteLine("\nВыберите фильм");
                            Film chosenFilm = Film.ChooseFilm("screening count important");
                            Screening chosenScreening = chosenFilm.ChooseScreening();

                            bool canBeDeleted = chosenScreening.CanTimeBeEdited();
                            if (!canBeDeleted)
                            {
                                Console.WriteLine("На этот сеанс уже были куплены билеты. В удалении отказано.");
                                continue;
                            }

                            chosenFilm.screenings.Remove(chosenScreening);

                        } // удалить сеанс

                        if (deleteCommand == "4")
                            break;

                        AnsiConsole.Write(new Markup("\nХотите продолжить удаление данных?\n"));
                        answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        Console.WriteLine();

                    } while (answer == "да");
                } // удаление данных

                else if (command == "4")
                    return;
            }
        }
        public static void GetClientAnalytics()
        {
            while (true)
            {
                Console.WriteLine("\nВы находитесь в режиме просмотра клиентской аналитики.");
                Console.WriteLine("1 - вывести информацию о клиентах, купивших наибольшее количество билетов;");
                Console.WriteLine("2 - вывести информацию о клиентах, купивших билеты на наибольшее количество разных сеансов;");
                Console.WriteLine("3 - вывести информацию о клиентах, потративших наибольшую сумму денег;");
                Console.WriteLine("4 - выйти из режима просмотра клиентской аналитики и вернуться к меню администратора.");
                string command = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3").AddChoice("4")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                if (command == "1")
                {
                    Console.WriteLine("Введите, сколько клиентов вас интересует (топ-1, топ-3 или топ-5)");
                    string topNum = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("3").AddChoice("5")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    User.GreatestTicketNum(int.Parse(topNum));
                } // клиенты, купившие наибольшее количество билетов

                else if (command == "2")
                {
                    Console.WriteLine("Введите, сколько клиентов вас интересует (топ-1, топ-3 или топ-5)");
                    string topNum = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("3").AddChoice("5")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    User.GreatestDistinctScreeningTicketNum(int.Parse(topNum));
                } // клиенты, купившие билеты на наибольшее количество разных сеансов

                else if (command == "3")
                {
                    Console.WriteLine("Введите, сколько клиентов вас интересует (топ-1, топ-3 или топ-5)");
                    string topNum = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("3").AddChoice("5")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    User.GreatestMoneyAmountSpent(int.Parse(topNum));
                } // клиенты, потратившие наибольшую сумму денег

                else if (command == "4")
                    return;
            }
        }
        public static void GetSalesStatistics()
        {
            while (true)
            {
                Console.WriteLine("\nВы находитесь в режиме просмотра аналитики по продажам.");
                Console.WriteLine("1 - посмотреть аналитику по одному конкретному фильму;");
                Console.WriteLine("2 - посмотреть аналитику по всем фильмам;");
                Console.WriteLine("3 - выйти из режима просмотра аналитики по продажам и вернуться к меню администратора.");
                string command = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                if (command == "1")
                {
                    Console.WriteLine("\nВыберите фильм.");
                    Film filmToAnalyse = Film.ChooseFilm("screening count not important");
                    if (filmToAnalyse.screenings.Count == 0)
                    {
                        Console.WriteLine("Данному фильму не назначен ни один сеанс. Информации нет.");
                        continue;
                    } // нет ни одного сеанса - статистики тоже

                    bool certainScreeningChosen = false;
                    Screening screeningToAnalyse = new Screening(); // потом происходит присваивание реального сеанса

                    bool timeChosen = false;
                    DateOnly startDay;
                    DateOnly endDay;

                    bool timeOfTheDayChosen = false;
                    TimeOnly startTimeOfTheDay;
                    TimeOnly endTimeOfTheDay;

                    Console.WriteLine("Хотите ли вы выбрать один конкретный сеанс?");
                    string certainScreeningNeeded = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                    if (certainScreeningNeeded == "да")
                    {
                        screeningToAnalyse = filmToAnalyse.ChooseScreening();
                        certainScreeningChosen = true;
                    }

                    Console.WriteLine("Хотите выбрать определенный промежуток времени?");
                    string isTimeIntervalNeeded = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                    if (isTimeIntervalNeeded == "да")
                    {
                        while (true)
                        {
                            Console.WriteLine("Введите начальную дату.");
                            startDay = GetDate();
                            Console.WriteLine("Введите конечную дату.");
                            endDay = GetDate();
                            if (startDay > endDay)
                                Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                            else
                            {
                                timeChosen = true;
                                break;
                            }
                        } // получаем даты

                        Console.WriteLine("Хотите выбрать определенный период дня?");
                        string isTimeOfTheDayNeeded = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                        while (true)
                        {
                            Console.WriteLine("Введите начальное время.");
                            startTimeOfTheDay = GetTime();
                            Console.WriteLine("Введите конечное время.");
                            endTimeOfTheDay = GetTime();
                            if (startTimeOfTheDay > endTimeOfTheDay)
                                Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                            else
                            {
                                timeOfTheDayChosen = true;
                                break;
                            }
                        } // получаем время
                    }

                    int seatsSold = 0;
                    int currRevenue = 0;

                    if (certainScreeningChosen == true)
                    {
                        if (timeChosen)
                        {
                            DateTime start;
                            DateTime end;

                            if (timeOfTheDayChosen)
                            {
                                DateOnly randomDate = GetRandomDate(startDay, endDay); // случайный день из заданного администратором промежутка
                                start = randomDate.ToDateTime(startTimeOfTheDay);
                                end = randomDate.ToDateTime(endTimeOfTheDay);
                            } // выбраны только дни
                            else
                            {
                                start = startDay.ToDateTime(TimeOnly.Parse("00:00:00"));
                                end = endDay.ToDateTime(TimeOnly.Parse("23:59:59"));
                            } // выбраны дни и период дня

                            List<Screening> allScreenings = new List<Screening>();

                            foreach (User user in User.all)
                            {
                                List<Ticket> relevantTickets = user.orders.FindAll(order => order.screening == screeningToAnalyse && order.timeBought >= start && order.timeBought <= end);
                                foreach (Ticket ticket in relevantTickets)
                                {
                                    seatsSold++;
                                    currRevenue = currRevenue + ticket.price;
                                    allScreenings.Add(ticket.screening);
                                }
                            }
                        } // выбраны дни
                        else
                        {
                            List<Screening> allScreenings = new List<Screening>();

                            foreach (User user in User.all)
                            {
                                List<Ticket> relevantTickets = user.orders.FindAll(order => order.screening == screeningToAnalyse);
                                foreach (Ticket ticket in relevantTickets)
                                {
                                    seatsSold++;
                                    currRevenue = currRevenue + ticket.price;
                                    allScreenings.Add(ticket.screening);
                                }
                            }
                        } // не выбраны дни
                    } // по одному сеансу
                    else if (timeChosen)
                    {
                        DateTime start;
                        DateTime end;

                        if (timeOfTheDayChosen)
                        {
                            DateOnly randomDate = GetRandomDate(startDay, endDay); // случайный день из заданного администратором промежутка
                            start = randomDate.ToDateTime(startTimeOfTheDay);
                            end = randomDate.ToDateTime(endTimeOfTheDay);
                        } // выбраны только дни
                        else
                        {
                            start = startDay.ToDateTime(TimeOnly.Parse("00:00:00"));
                            end = endDay.ToDateTime(TimeOnly.Parse("23:59:59"));
                        } // выбраны дни и период дня
                        
                        List<Screening> allScreenings = new List<Screening>();

                        foreach (User user in User.all)
                        {
                            List<Ticket> relevantTickets = user.orders.FindAll(order => order.screening.film == filmToAnalyse && order.timeBought >= start && order.timeBought <= end);
                            foreach (Ticket ticket in relevantTickets)
                            {
                                seatsSold++;
                                currRevenue = currRevenue + ticket.price;
                                allScreenings.Add(ticket.screening);
                            }
                        }

                    } // по одному фильму по выбранным дням
                    else
                    {
                        List<Screening> allScreenings = new List<Screening>();

                        foreach (User user in User.all)
                        {
                            List<Ticket> relevantTickets = user.orders.FindAll(order => order.screening.film == filmToAnalyse);
                            foreach (Ticket ticket in relevantTickets)
                            {
                                seatsSold++;
                                currRevenue = currRevenue + ticket.price;
                                allScreenings.Add(ticket.screening);
                            }
                        }
                    } // по одному фильму без фильтров

                    Console.WriteLine($"\nПродано {seatsSold} мест.");
                    Console.WriteLine($"Текущая выручка составляет {currRevenue} рублей");
                } // по одному конкретному фильму

                else if (command == "2")
                {
                    bool ageRestChosen = false;
                    string chosenAgeRest = ""; // потом происходит присваивание реального ограничения

                    bool timeChosen = false;
                    DateOnly startDay;
                    DateOnly endDay;

                    bool timeOfTheDayChosen = false;
                    TimeOnly startTimeOfTheDay;
                    TimeOnly endTimeOfTheDay;

                    Console.WriteLine("Хотите ли вы выбрать конкретный возрастной рейтинг?");
                    string isAgeRestNeeded = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                    if (isAgeRestNeeded == "да")
                    {
                        chosenAgeRest = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                        .AddChoice("0+")
                                                        .AddChoice("6+")
                                                        .AddChoice("12+")
                                                        .AddChoice("16+")
                                                        .AddChoice("18+")
                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        ageRestChosen = true;
                    }

                    Console.WriteLine("Хотите выбрать определенный промежуток времени?");
                    string isTimeIntervalNeeded = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                    if (isTimeIntervalNeeded == "да")
                    {
                        while (true)
                        {
                            Console.WriteLine("Введите начальную дату.");
                            startDay = GetDate();
                            Console.WriteLine("Введите конечную дату.");
                            endDay = GetDate();
                            if (startDay > endDay)
                                Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                            else
                            {
                                timeChosen = true;
                                break;
                            }
                        } // получаем даты

                        Console.WriteLine("Хотите выбрать определенный период дня?");
                        string isTimeOfTheDayNeeded = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                        while (true)
                        {
                            Console.WriteLine("Введите начальное время.");
                            startTimeOfTheDay = GetTime();
                            Console.WriteLine("Введите конечное время.");
                            endTimeOfTheDay = GetTime();
                            if (startTimeOfTheDay > endTimeOfTheDay)
                                Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                            else
                            {
                                timeOfTheDayChosen = true;
                                break;
                            }
                        } // получаем время
                    }

                    int seatsSold = 0;
                    int currRevenue = 0;

                    if (ageRestChosen)
                    {
                        if (timeChosen)
                        {
                            DateTime start;
                            DateTime end;

                            if (timeOfTheDayChosen)
                            {
                                DateOnly randomDate = GetRandomDate(startDay, endDay); // случайный день из заданного администратором промежутка
                                start = randomDate.ToDateTime(startTimeOfTheDay);
                                end = randomDate.ToDateTime(endTimeOfTheDay);
                            } // выбраны только дни
                            else
                            {
                                start = startDay.ToDateTime(TimeOnly.Parse("00:00:00"));
                                end = endDay.ToDateTime(TimeOnly.Parse("23:59:59"));
                            } // выбраны дни и период дня

                            List<Screening> allScreenings = new List<Screening>();

                            foreach (User user in User.all)
                            {
                                List<Ticket> relevantTickets = user.orders.FindAll(order => order.screening.film.ageRestriction == chosenAgeRest && order.timeBought >= start && order.timeBought <= end);
                                foreach (Ticket ticket in relevantTickets)
                                {
                                    seatsSold++;
                                    currRevenue = currRevenue + ticket.price;
                                    allScreenings.Add(ticket.screening);
                                }
                            }
                        } // выбраны дни
                        else
                        {
                            List<Screening> allScreenings = new List<Screening>();

                            foreach (User user in User.all)
                            {
                                List<Ticket> relevantTickets = user.orders.FindAll(order => order.screening.film.ageRestriction == chosenAgeRest);
                                foreach (Ticket ticket in relevantTickets)
                                {
                                    seatsSold++;
                                    currRevenue = currRevenue + ticket.price;
                                    allScreenings.Add(ticket.screening);
                                }
                            }
                        } // по всем дням
                    }  // все фильмы с определенным возрастным ограничением
                    else
                    {
                        if (timeChosen)
                        {
                            DateTime start;
                            DateTime end;

                            if (timeOfTheDayChosen)
                            {
                                DateOnly randomDate = GetRandomDate(startDay, endDay); // случайный день из заданного администратором промежутка
                                start = randomDate.ToDateTime(startTimeOfTheDay);
                                end = randomDate.ToDateTime(endTimeOfTheDay);
                            } // выбраны только дни
                            else
                            {
                                start = startDay.ToDateTime(TimeOnly.Parse("00:00:00"));
                                end = endDay.ToDateTime(TimeOnly.Parse("23:59:59"));
                            } // выбраны дни и период дня

                            List<Screening> allScreenings = new List<Screening>();

                            foreach (User user in User.all)
                            {
                                List<Ticket> relevantTickets = user.orders.FindAll(order => order.timeBought >= start && order.timeBought <= end);
                                foreach (Ticket ticket in relevantTickets)
                                {
                                    seatsSold++;
                                    currRevenue = currRevenue + ticket.price;
                                    allScreenings.Add(ticket.screening);
                                }
                            }

                        } // выбраны дни
                        else
                        {
                            List<Screening> allScreenings = new List<Screening>();

                            foreach (User user in User.all)
                            {
                                List<Ticket> relevantTickets = user.orders;
                                foreach (Ticket ticket in relevantTickets)
                                {
                                    seatsSold++;
                                    currRevenue = currRevenue + ticket.price;
                                    allScreenings.Add(ticket.screening);
                                }
                            }
                        } // по всем дням
                    } // все фильмы

                    Console.WriteLine($"\nПродано {seatsSold} мест.");
                    Console.WriteLine($"Текущая выручка составляет {currRevenue} рублей");

                } // по всем фильмам

                else if (command == "3")
                    return;
            }
        }
        public static void TrackHallLoad()
        {
            while (true)
            {
                bool certainFilm = false;
                Film filmChosen = new Film(); //  потом присваивается настоящий фильм

                bool certainScreeningChosen = false;
                Screening screeningChosen = new Screening(); // потом присваивается настоящий сеанс

                bool ageRestChosen = false;
                string chosenAgeRest = ""; // потом присваивается настоящее возрастное ограничение

                bool certainHallChosen = false;
                Hall hallChosen = new Hall(); // потом присваивается настоящий зал

                bool timeChosen = false;
                DateOnly startDay;
                DateOnly endDay;

                bool timeOfTheDayChosen = false;
                TimeOnly startTimeOfTheDay;
                TimeOnly endTimeOfTheDay;

                Console.WriteLine("\nВы находитесь в режиме просмотра загруженности залов.");

                string certainFilmHalls = AnsiConsole.Prompt(new TextPrompt<string>("Вас интересут залы, где идет в прокате один конкретный фильм?")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                if (certainFilmHalls == "да")
                {
                    filmChosen = Film.ChooseFilm("screening count not important");
                    if (filmChosen.halls.Count == 0)
                    {
                        Console.WriteLine("Данный фильм не идет в прокате ни в одном зале.");
                        continue;
                    }
                    certainFilm = true;
                }

                else
                {
                    Console.WriteLine("Хотите ли вы выбрать конкретный возрастной рейтинг?");
                    string isAgeRestNeeded = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                    if (isAgeRestNeeded == "да")
                    {
                        chosenAgeRest = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                        .AddChoice("0+")
                                                        .AddChoice("6+")
                                                        .AddChoice("12+")
                                                        .AddChoice("16+")
                                                        .AddChoice("18+")
                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        ageRestChosen = true;
                    }

                }

                string certainHall = AnsiConsole.Prompt(new TextPrompt<string>("Вас интересует конкретный зал?")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                if (certainHall == "да")
                {
                    if (certainFilm)
                    {
                        TextPrompt<string> hallChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введен неверный вариант. Повторите попытку.");
                        foreach (Hall hall in filmChosen.halls)
                            hallChoicePrompt.AddChoice(hall.name);
                        string chHallName = AnsiConsole.Prompt(hallChoicePrompt);

                        foreach (Hall hall in filmChosen.halls)
                        {
                            if (hall.name == chHallName)
                            {
                                hallChosen = hall;
                                break;
                            }
                        }

                        string certainScreening = AnsiConsole.Prompt(new TextPrompt<string>("Вас интересует конкретный сеанс?")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                        if (certainScreening == "да")
                        {
                            List<Screening> relevantScreenings = filmChosen.screenings.FindAll(screening => screening.hall.name == hallChosen.name);

                            if (relevantScreenings.Count == 0)
                            {
                                Console.WriteLine("У выбранного ранее фильма в выбранном зале нет сеансов.");
                                continue;
                            }

                            TextPrompt<string> scrChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.");

                            for (int i = 0; i < relevantScreenings.Count; i++)
                            {
                                Console.WriteLine($"{i + 1,4}: {relevantScreenings[i].hall.name,15} {relevantScreenings[i].time.ToString("dd/MM/yyyy HH:mm"),16}");
                                scrChoicePrompt.AddChoice(Convert.ToString(i + 1));
                            }

                            Console.WriteLine("\nВведите номер одного выбранного сеанса:");
                            int scrNum = int.Parse(AnsiConsole.Prompt(scrChoicePrompt)) - 1;

                            screeningChosen = relevantScreenings[scrNum];
                            certainScreeningChosen = true;
                        }
                    }
                    else
                    {
                        hallChosen = Hall.ChooseHall();
                    }
                }

                if (!certainScreeningChosen)
                {
                    Console.WriteLine("Хотите выбрать определенный промежуток времени?");
                    string isTimeIntervalNeeded = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                    if (isTimeIntervalNeeded == "да")
                    {
                        while (true)
                        {
                            Console.WriteLine("Введите начальную дату.");
                            startDay = GetDate();
                            Console.WriteLine("Введите конечную дату.");
                            endDay = GetDate();
                            if (startDay > endDay)
                                Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                            else
                            {
                                timeChosen = true;
                                break;
                            }
                        } // получаем даты

                        Console.WriteLine("Хотите выбрать определенный период дня?");
                        string isTimeOfTheDayNeeded = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                        while (true)
                        {
                            Console.WriteLine("Введите начальное время.");
                            startTimeOfTheDay = GetTime();
                            Console.WriteLine("Введите конечное время.");
                            endTimeOfTheDay = GetTime();
                            if (startTimeOfTheDay > endTimeOfTheDay)
                                Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                            else
                            {
                                timeOfTheDayChosen = true;
                                break;
                            }
                        } // получаем время
                    }
                }

                

            }
        }
    }
    class User
    {
        public static List<User> all = new List<User>();

        public int balance;
        public string username;
        public List<Ticket> orders = new List<Ticket>();

        public void SetUsername()
        {
            bool success = false;
            Console.WriteLine("\nВведите свое уникальное имя пользователя:");
            while (!success)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input.Length >= 1)
                {
                    bool alreadyTaken = false;
                    foreach (User existingUser in User.all)
                    {
                        if (input == existingUser.username)
                        {
                            alreadyTaken = true;
                            break;
                        }
                    }

                    if (alreadyTaken)
                        Console.WriteLine("Данное имя уже занято.");

                    else
                    {
                        username = input;
                        success = true;
                    }
                }
                else
                    Console.WriteLine("Повторите ввод.");
            }
        }
        public void UpdateBalance()
        {
            Console.WriteLine("Введите сумму, на которую хотите пополнить баланс.");
            int toAdd = GetPositiveInt();
            balance = balance + toAdd;
            Console.WriteLine("Пополнение прошло успешно!");
        }
        public Dictionary<Screening, List<List<int>>> ReadOneScreeningOrder(List<Ticket> alreadyReserved)
        {
            Dictionary<Screening, List<List<int>>> currOrder = new Dictionary<Screening, List<List<int>>>();
            string answer = "да";
            
            Console.WriteLine("\nВыберите фильм.");
            
            Film chosenFilm = Film.ChooseFilm("choose screening after");
            Console.WriteLine();
          

            Screening chosenScreening = chosenFilm.ChooseScreening();
            AnsiConsole.Write(new Markup("Доступные места [green](0 - место доступно;[/] [red] x - место выкуплено)[/]\n"));
            chosenScreening.PrintHallData("availability");
            AnsiConsole.Write(new Markup("Цены на билеты\n"));
            chosenScreening.PrintHallData("prices");

            // в currOrder записываются списки вида {<ряд>, <место>} c ключом Screening
            do
            {
                try
                {
                    Console.WriteLine("\nПожалуйста, выберите места, которые вы хотите выкупить.");
                    Console.WriteLine("Введите один номер места в формате '<номер ряда> <номер места>'.");

                    string[] seatData = AnsiConsole.Prompt(new TextPrompt<string>("> ")).Split(' ');

                    List<int> ticket = new List<int> { int.Parse(seatData[0]) - 1, int.Parse(seatData[1]) - 1 };
                    int areRowSeatValid = chosenScreening.priceData[ticket[0]][ticket[1]]; // ловим IndexOutOfRangeException до добавления к заказу

                    // проверка, было ли это же место ранее добавлено в текущий заказ пользователя
                    bool alreadyInOrder = false;

                    foreach (Ticket existingTicket in alreadyReserved)
                    {
                        if (existingTicket.screening == chosenScreening && existingTicket.seat[0] == ticket[0] && existingTicket.seat[0] == ticket[1])
                        {
                            alreadyInOrder = true;
                            break;
                        }
                    }

                    if (currOrder.ContainsKey(chosenScreening))
                    {
                        foreach (var existingTicket in currOrder[chosenScreening])
                        {
                            if (ticket[0] == existingTicket[0] && ticket[1] == existingTicket[1])
                            {
                                alreadyInOrder = true;
                                break;
                            }
                        }
                    }

                    if (alreadyInOrder)
                        AnsiConsole.Write(new Markup("Вы уже выбрали это место.\n"));

                    // проверка, свободно ли место
                    else if (chosenScreening.seatsAvailability[ticket[0]][ticket[1]] == '0')
                    {
                        // добавляем в заказ
                        if (currOrder.ContainsKey(chosenScreening))
                            currOrder[chosenScreening].Add(ticket);
                        else
                            currOrder.Add(chosenScreening, new List<List<int>> { ticket });
                    }
                    else
                        AnsiConsole.Write(new Markup("К сожалению, данное место уже куплено.\n"));

                    AnsiConsole.Write(new Markup("Хотите продолжить покупку билетов на этот сеанс?\n"));
                    answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                        .AddChoice("да")
                                                        .AddChoice("нет")
                                                        .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                }
                catch (Exception)
                {
                    AnsiConsole.Write(new Markup("Неверное значение для ряда и/или места. Повторите ввод.\n"));
                }
            } while (answer == "да");

            return currOrder;
        }
        public bool Check_Balance(List<Ticket> reserved) // проверка, достаточно ли у пользователя средств
        {
            Console.WriteLine("\nВыполняется проверка...\n");
            int ticketPriceSum = 0;

            foreach (Ticket ticket in reserved)
            {
                int row = ticket.seat[0]; int seat = ticket.seat[1];
                ticket.SetPrice(ticket.screening.priceData[row][seat]);
                ticketPriceSum = ticketPriceSum + ticket.screening.priceData[row][seat];
            }
            bool verificationStatus = ticketPriceSum <= balance;
            return verificationStatus;
        }
        public void MakeOrder()
        {
            List<Ticket> reservedTickets = new List<Ticket>();

            int totalScreeningCount = 0;
            foreach (Film film in Film.all)
                totalScreeningCount = totalScreeningCount + film.screenings.Count;
            
            if (totalScreeningCount == 0)
            {
                Console.WriteLine("Доступных сеансов нет.");
                return;
            }

            string answer = "да"; // резервируем билеты
            do
            {
                Dictionary<Screening, List<List<int>>> ticketsToReserve = ReadOneScreeningOrder(reservedTickets);
                foreach (KeyValuePair<Screening, List<List<int>>> kvp in ticketsToReserve)
                {
                    foreach (List<int> seatsData in kvp.Value)
                    {
                        Ticket currTicket = new Ticket(username, kvp.Key, seatsData);
                        reservedTickets.Add(currTicket);
                    }
                } // добавили в бронирование

                AnsiConsole.Write(new Markup("\nХотите продолжить ввод данных для покупки билетов?\n"));
                answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
            } while (answer == "да");

            if (reservedTickets.Count == 0)
            {
                Console.WriteLine("Сожалеем, что вы не приобрели ни одного билета. Пожалуйста, приходите к нам ещё!\n");
                return;
            }

            bool isOkayToBuy = Check_Balance(reservedTickets);
            while (!isOkayToBuy)
            {
                Console.WriteLine("Ошибка: недостаточно средств для покупки. Хотите пополнить баланс?");
                string yn = AnsiConsole.Prompt(new TextPrompt<string>("")
                                            .AddChoice("да")
                                            .AddChoice("нет")
                                            .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.\n"));
                if (yn == "да")
                {
                    UpdateBalance();
                    isOkayToBuy = Check_Balance(reservedTickets);
                }
                else
                {
                    Console.WriteLine($"Ваша бронь билетов аннулирована. На вашем счету остается {balance} рублей.");
                    return;
                }

            } // пополняем баланс или снимаем бронь и выходим

            foreach (Ticket ticket in reservedTickets)
            {
                ticket.screening.UpdateSeats(ticket.seat);
                ticket.SetTimeBougth();
                this.orders.Add(ticket);
            } // вносим данные о покупке в системы

            Console.WriteLine("Покупка прошла успешно! Ваши билеты:"); // печатаем купленные билеты для пользователя
            foreach (Ticket ticket in reservedTickets)
                ticket.Print();

        }
        public void PrintAllTickets()
        {
            if (orders.Count == 0)
                Console.Write("Билетов нет.\n");
            else
            {
                orders.Sort((x, y) => x.screening.time.CompareTo(y.screening.time));
                foreach (Ticket ticket in orders)
                    ticket.Print();
            }
        }
        public static void GreatestTicketNum(int topNum)
        {
            List<User>sortedByTicketNum = all.OrderByDescending(o => o.orders.Count).ToList();

            if (sortedByTicketNum.Count < topNum)
            {
                Console.WriteLine("Недостаточное число клиентов в базе.");
                return;
            }

            for (int i = 0; i < topNum - 1; i++)
                Console.WriteLine($"{i+1}. {sortedByTicketNum[i].username}");
        }
        public static void GreatestDistinctScreeningTicketNum(int topNum)
        {
            List<ArrayList> userData = new List<ArrayList>();
            foreach (User user in User.all)
            {
                List<Ticket> distinctScreeningTickets = user.orders
                    .GroupBy(p => new { p.screening.film, p.screening.hall, p.screening.time })
                    .Select(g => g.First())
                    .ToList();
                ArrayList userStats = new ArrayList { user.username, distinctScreeningTickets.Count };
                userData.Add(userStats);
            }
            List<ArrayList> sorted = userData.OrderByDescending(o => o[1]).ToList();

            if (sorted.Count < topNum)
            {
                Console.WriteLine("Недостаточное число клиентов в базе.");
                return;
            }

            for (int i = 0; i < topNum - 1; i++)
                Console.WriteLine($"{i + 1}. {sorted[i]}");
        }
        public static void GreatestMoneyAmountSpent(int topNum)
        {
            List<ArrayList> userData = new List<ArrayList>();
            foreach (User user in User.all)
            {
                int total = user.orders.Sum(ticket => ticket.price);
                ArrayList userStats = new ArrayList { user.username, total };
                userData.Add(userStats);
            }
            List<ArrayList> sorted = userData.OrderByDescending(o => o[1]).ToList();

            if (sorted.Count < topNum)
            {
                Console.WriteLine("Недостаточное число клиентов в базе.");
                return;
            }

            for (int i = 0; i < topNum - 1; i++)
                Console.WriteLine($"{i + 1}. {sorted[i]}");
        }
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
                        Console.Write("Данное название уже есть в базе. ");
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
                    Console.WriteLine("Неверное значение для названия зала. Повторите попытку.");
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
        public bool CanBeEdited()
        {
            bool canBeEdited = true;

            foreach (Film film in Film.all)
            {
                foreach (Screening screening in film.screenings)
                {
                    if (screening.hall.name != name)
                        continue;

                    for (int i = 0; i < screening.hall.rowsNum; i++)
                    {
                        for (int j = 0; j < screening.hall.seatsInRowNum; j++)
                        {
                            if (screening.seatsAvailability[i][j] == 'x')
                            {
                                canBeEdited = false;
                                break;
                            }
                        }
                    }
                }
            }
            return canBeEdited;
        }
        public static Hall ChooseHall()
        {
            TextPrompt<string> hallChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введен неверный вариант. Повторите попытку.");
            foreach (Hall hall in Hall.all)
                hallChoicePrompt.AddChoice(hall.name);
            string chHallName = AnsiConsole.Prompt(hallChoicePrompt);

            foreach (Hall hall in Hall.all)
            {
                if (hall.name == chHallName)
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
        public List<Hall> halls = new List<Hall>();
        public List<Screening> screenings = new List<Screening>();

        public void SetName()
        {
            bool succeeded = false;
            Console.WriteLine($"\nВведите название фильма:");
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
            Console.WriteLine($"\nВведите возрастное ограничение для фильма {name}:");
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
            Console.WriteLine($"\nВведите язык для фильма {name}:");
            string langCode = AnsiConsole.Prompt(new TextPrompt<string>("1 - русский, 2 - английский")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (langCode == "1")
                language = "русский";
            if (langCode == "2")
                language = "английский";
        }
        public bool CanBeEdited()
        {
            bool canBeEdited = true;
            foreach (Screening screening in screenings)
            {
                for (int i = 0; i < screening.hall.rowsNum; i++)
                {
                    for (int j = 0; j < screening.hall.seatsInRowNum; j++)
                    {
                        if (screening.seatsAvailability[i][j] == 'x')
                        {
                            canBeEdited = false;
                            break;
                        }
                    }
                }
            }
            return canBeEdited;
        }
        public Screening ChooseScreening()
        {
            List<Screening> relevantScreenings = screenings.FindAll(screening => screening.time > DateTime.Now);
            relevantScreenings.Sort((x, y) => x.time.CompareTo(y.time));
            TextPrompt<string> scrChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.");

            for (int i = 0; i < relevantScreenings.Count; i++)
            {
                Console.WriteLine($"{i + 1,4}: {relevantScreenings[i].hall.name,15} {relevantScreenings[i].time.ToString("dd/MM/yyyy HH:mm"),16}");
                scrChoicePrompt.AddChoice(Convert.ToString(i + 1));
            }

            Console.WriteLine("\nВведите номер одного выбранного сеанса:");
            int scrNum = int.Parse(AnsiConsole.Prompt(scrChoicePrompt)) - 1;

            return screenings[scrNum];
        }
        public static Film ChooseFilm(string after)
        {
            TextPrompt<string> filmChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Неверный вариант. Повторите попытку.");

            if (after == "screening count important") // убираем варианты, где у фильма нет сеансов
            {
                foreach (Film film in Film.all)
                {
                    if (film.screenings.Count != 0)
                        filmChoicePrompt.AddChoice(film.name);
                }
            }

            else // "screening count not important"
            { 
                foreach (Film film in Film.all)
                    filmChoicePrompt.AddChoice(film.name);
            }
           
            string inputName = AnsiConsole.Prompt(filmChoicePrompt);

            foreach (Film film in Film.all)
            {
                if (film.name == inputName)
                    return film;
            }
            return new Film(); // dummyFilm
        }
        public int FindScreeningIndexInList(Screening screeningToFind)
        {

            for (int i = 0; i < screenings.Count; i++)
            {
                if (screenings[i].hall == screeningToFind.hall && screenings[i].time == screeningToFind.time)
                    return i;
            }
            return 0; // dummy index
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
            Console.WriteLine($"\nВ зале {hall.rowsNum} рядов по {hall.seatsInRowNum} мест.");
            Console.WriteLine("Введите стоимость билетов на сеанс через пробел:");

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
        public void PrintHallData(string printChoice) // печать таблицы с ценами (по аргументу "prices") или доступностью мест (по аргументу "availability")
        {
            Table table = new Table().Border(TableBorder.DoubleEdge).AddColumn(new TableColumn("Место\nРяд"));

            for (int i = 1; i < hall.seatsInRowNum + 1; i++)
                table.AddColumn(new TableColumn(Convert.ToString(i)).Centered());

            for (int i = 0; i < hall.rowsNum; i++)
            {
                table.AddEmptyRow();
                table.UpdateCell(i, 0, new Text(Convert.ToString(i + 1)));

                for (int j = 0; j < hall.seatsInRowNum; j++)
                {
                    if (printChoice == "availability")
                        table.UpdateCell(i, j + 1, Convert.ToString(seatsAvailability[i][j]));
                    else if (printChoice == "prices")
                        table.UpdateCell(i, j + 1, Convert.ToString(priceData[i][j]));
                }
            }
            AnsiConsole.Write(table);

        }
        public void UpdateSeats(List<int> place)
        {
            int row = place[0]; int seat = place[1];
            seatsAvailability[row][seat] = 'x'; // вносим, что место было куплено
        }
        public void MarkSeatsAsTaken()
        {
            AnsiConsole.Write(new Markup("Данные о местах [green](0 - место доступно;[/] [red] x - место выкуплено)[/]\n"));
            this.PrintHallData("availability");
            string answer = "да";
            do
            {
                try
                {
                    Console.WriteLine("\nПожалуйста, выберите места, занятость которых вы хотите изменить.");
                    Console.WriteLine("Введите один номер места в формате '<номер ряда> <номер места>'.");

                    string[] seatData = AnsiConsole.Prompt(new TextPrompt<string>("> ")).Split(' ');
                    List<int> ticket = new List<int> { int.Parse(seatData[0]) - 1, int.Parse(seatData[1]) - 1 };
                    int areRowSeatValid = priceData[ticket[0]][ticket[1]]; // ловим IndexOutOfRangeException до добавления к заказу

                    if (seatsAvailability[ticket[0]][ticket[1]] == '0')
                        seatsAvailability[ticket[0]][ticket[1]] = 'x';
                    else
                        AnsiConsole.Write(new Markup("Данное место уже занято.\n"));

                    AnsiConsole.Write(new Markup("Хотите продолжить выбор мест?\n"));
                    answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                        .AddChoice("да")
                                                        .AddChoice("нет")
                                                        .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                }
                catch (Exception)
                {
                    AnsiConsole.Write(new Markup("Неверное значение для ряда и/или места. Повторите ввод.\n"));
                }
            } while (answer == "да");
        }
        public bool ChangePrices()
        {
            AnsiConsole.Write(new Markup("Текущие цены:\n"));
            PrintHallData("prices");
            AnsiConsole.Write(new Markup("Вы находитесь в режиме изменения цен\n"));

            do
            {
                AnsiConsole.Write(new Markup("Введите один номер места в формате '<номер ряда> <номер места>'.\nЧтобы вернуться в режим изменения данных, введите 'м'.[/]\n"));
                string input = AnsiConsole.Prompt(new TextPrompt<string>("> "));

                if (input == "м")
                    return true;

                try
                {
                    string[] seatData = input.Split(' ');
                    int row = int.Parse(seatData[0]) - 1;
                    int seat = int.Parse(seatData[1]) - 1;
                    int areRowSeatValid = priceData[row][seat]; // ловим IndexOutOfRangeException до ввода цены

                    do
                    {
                        AnsiConsole.Write(new Markup("[lightgoldenrod2]Введите новую цену для выбранного места.[/]\n"));
                        AnsiConsole.Write(new Markup("[lightgoldenrod2]Если вы передумали менять цену на данное место, введите 'м'.[/]\n"));
                        string strPrice = AnsiConsole.Prompt(new TextPrompt<string>("> "));

                        if (strPrice == "м")
                        {
                            Console.WriteLine();
                            break;
                        }

                        try
                        {
                            int newPrice = int.Parse(strPrice);
                            if (newPrice >= 0)
                            {
                                priceData[row][seat] = newPrice;
                                return false;
                            }
                            else
                            {
                                AnsiConsole.Write(new Markup("Неверное значение для цены.Повторите ввод.\n"));
                                continue;
                            }
                        }
                        catch (Exception)
                        {
                            AnsiConsole.Write(new Markup("Неверное значение для цены.Повторите ввод.[/]\n"));
                        }
                    } while (true);
                }
                catch (Exception)
                {
                    AnsiConsole.Write(new Markup("Неверное значение для ряда и/или места.Повторите ввод.\n\n"));
                }

            } while (true);
        }
        public bool CanTimeBeEdited()
        {
            bool canBeEdited = true;

            for (int i = 0; i < hall.rowsNum; i++)
            {
                for (int j = 0; j < hall.seatsInRowNum; j++)
                {
                    if (seatsAvailability[i][j] == 'x')
                    {
                        canBeEdited = false;
                        break;
                    }
                }
            }
            return canBeEdited;
        }
    }
    class Ticket
    {
        public string username;
        public Screening screening;
        public List<int> seat;
        public int price;
        public DateTime timeBought;

        public Ticket(string username, Screening screening, List<int> seat)
        {
            this.username = username;
            this.screening = screening;
            this.seat = seat;
        }
        public void SetTimeBougth()
        {
            timeBought = DateTime.Now;
        }
        public void SetPrice(int somePrice)
        {
            price = somePrice;
        }
        public void Print()
        {
            Console.WriteLine($"Фильм {screening.film.name,15} | Зал {screening.hall.name, 15} | {screening.time.ToString("dd/MM/yyyy HH:mm")} | Ряд {seat[0] + 1, 2} | Место {seat[1] + 1, 2}");
        }
    }
    public static class CompareObject
    {
        public static bool Compare<T>(T e1, T e2)
        {
            bool flag = true;
            bool match = false;
            int countFirst, countSecond;
            foreach (PropertyInfo propObj1 in e1.GetType().GetProperties())
            {
                var propObj2 = e2.GetType().GetProperty(propObj1.Name);
                if (propObj1.PropertyType.Name.Equals("List`1"))
                {
                    dynamic objList1 = propObj1.GetValue(e1, null);
                    dynamic objList2 = propObj2.GetValue(e2, null);
                    countFirst = objList1.Count;
                    countSecond = objList2.Count;
                    if (countFirst == countSecond)
                    {
                        countFirst = objList1.Count - 1;
                        while (countFirst > -1)
                        {
                            match = false;
                            countSecond = objList2.Count - 1;
                            while (countSecond > -1)
                            {
                                match = Compare(objList1[countFirst], objList2[countSecond]);
                                if (match)
                                {
                                    objList2.Remove(objList2[countSecond]);
                                    countSecond = -1;
                                    match = true;
                                }
                                if (match == false && countSecond == 0)
                                {
                                    return false;
                                }
                                countSecond--;
                            }
                            countFirst--;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (!(propObj1.GetValue(e1, null).Equals(propObj2.GetValue(e2, null))))
                {
                    flag = false;
                    return flag;
                }
            }
            return flag;
        }
    } // https://www.c-sharpcorner.com/article/comparing-objects-in-c-sharp/
}