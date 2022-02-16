using Spectre.Console;
using System.Globalization;
using System.Text.RegularExpressions;

class Program
{
    public static void Main(string[] args)
    {
        Administrator admin = new Administrator();
        Console.WriteLine("С началом работы! Вы находитесь в режиме администратора для ввода данных.");
        StartAdministratorInterface();
        UserInterface();
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
                Administrator.AddNewScreening(film);
                AnsiConsole.Write(new Markup($"Хотите добавить еще сеанс для фильма {film.name}?\n"));
                answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                            .AddChoice("да")
                                            .AddChoice("нет")
                                            .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.\n"));
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
            Console.WriteLine("Вы находитесь в меню пользователя");
            Console.WriteLine("1 - приобрести билеты,");
            Console.WriteLine("2 - вернуть билеты");
            Console.WriteLine("3 - выйти из меню пользователя и вернуться к меню выбора интерфейса.");
            string command = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .AddChoice("3")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            Console.WriteLine();

            if (command == "1")
                currUser.MakeOrder();

            else if (command == "2")
            { }//ReturnTickets();

            else if (command == "3")
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
    static DateTime GetDate()
    {
        while (true)
        {
            Console.WriteLine("Введите дату в формате ДД/ММ/ГГГГ ЧЧ:ММ");
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
        public static void AddNewScreening(Film currFilm)
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
                    foreach (Hall hall in film.halls)
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

            Console.WriteLine($"\nВыберите время для показа фильма {currFilm.name} в зале {chosenHallName}.");

            DateTime showDate = GetDate();
            foreach (Screening screening in currFilm.screenings)
            {
                if (screening.hall == chosenHall && screening.time == showDate)
                {
                    Console.WriteLine("Данный сеанс уже есть в базе.");
                    return;
                }
            }
            Screening newScreening = new Screening { film = currFilm, hall = chosenHall, time = showDate };
            newScreening.SetInitialAvailability();
            newScreening.SetInitialPrices();
            currFilm.screenings.Add(newScreening);

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
            Console.WriteLine("Введите свое уникальное имя пользователя:");
            while (!success)
            {
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
        public Dictionary<Screening, List<List<int>>> ReadOneScreeningOrder()
        {
            Dictionary<Screening, List<List<int>>> currOrder = new Dictionary<Screening, List<List<int>>>();
            string answer = "да";

            Console.WriteLine("Выберите фильм.");
            Film chosenFilm = Film.ChooseFilm();
            Console.WriteLine();

            List<Screening> relevantScreenings = chosenFilm.screenings.FindAll(screening => screening.time > DateTime.Now);
            relevantScreenings.Sort((x, y) => x.time.CompareTo(y.time));
            TextPrompt<string> scrChoicePrompt = new TextPrompt<string>("");

            for (int i = 0; i < relevantScreenings.Count; i++)
            {
                Console.WriteLine($"{i + 1,4}: {relevantScreenings[i].hall.name,15} {relevantScreenings[i].time.ToString("MM/dd/yyyy HH:mm"),16}");
                scrChoicePrompt.AddChoice(Convert.ToString(i));
            } // печатаю 

            Console.WriteLine("\nВведите номер одного выбранного сеанса:");
            int scrNum = int.Parse(AnsiConsole.Prompt(scrChoicePrompt)) - 1;
            int indInScreenings = chosenFilm.FindScreeningIndexInList(relevantScreenings[scrNum]);

            AnsiConsole.Write(new Markup("Доступные места [green](0 - место доступно;[/] [red] x - место выкуплено)[/]\n"));
            chosenFilm.screenings[indInScreenings].Print_Hall_Data("availability");
            AnsiConsole.Write(new Markup("Цены на билеты\n"));
            chosenFilm.screenings[indInScreenings].Print_Hall_Data("prices");

            // в currOrder записываются списки вида {<ряд>, <место>} c ключом Screening
            do
            {
                try
                {
                    Console.WriteLine("\nПожалуйста, выберите места, которые вы хотите выкупить.");
                    Console.WriteLine("Введите один номер места в формате '<номер ряда> <номер места>'.");

                    string[] seatData = AnsiConsole.Prompt(new TextPrompt<string>("> ")).Split(' ');

                    List<int> ticket = new List<int> { int.Parse(seatData[0]) - 1, int.Parse(seatData[1]) - 1 };
                    int areRowSeatValid = chosenFilm.screenings[indInScreenings].priceData[ticket[0]][ticket[1]]; // ловим IndexOutOfRangeException до добавления к заказу

                    // проверка, было ли это же место ранее добавлено в текущий заказ пользователя
                    bool alreadyInOrder = false;

                    if (currOrder.ContainsKey(chosenFilm.screenings[indInScreenings]))
                    {
                        foreach (var existingTicket in currOrder[chosenFilm.screenings[indInScreenings]])
                        {
                            if (ticket[0] == existingTicket[0] && ticket[1] == existingTicket[1])
                                alreadyInOrder = true;
                            break;
                        }
                    }

                    if (alreadyInOrder)
                        AnsiConsole.Write(new Markup("Вы уже выбрали это место.\n"));

                    // проверка, свободно ли место
                    else if (chosenFilm.screenings[indInScreenings].seatsAvailability[ticket[0]][ticket[1]] == '0')
                    {
                        // добавляем в заказ
                        if (currOrder.ContainsKey(chosenFilm.screenings[indInScreenings]))
                            currOrder[chosenFilm.screenings[indInScreenings]].Add(ticket);
                        else
                            currOrder.Add(chosenFilm.screenings[indInScreenings], new List<List<int>> { ticket });
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

            string answer = "да"; // резервируем билеты
            do
            {
                Dictionary<Screening, List<List<int>>> ticketsToReserve = ReadOneScreeningOrder();
                foreach (KeyValuePair<Screening, List<List<int>>> kvp in ticketsToReserve)
                {
                    foreach (List<int> seatsData in kvp.Value)
                    {
                        Ticket currTicket = new Ticket(username, kvp.Key, seatsData);
                        reservedTickets.Add(currTicket);
                    }
                } // добавили в бронирование

                AnsiConsole.Write(new Markup("Хотите продолжить ввод данных для покупки билетов?\n"));
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
        public List<Hall> halls = new List<Hall>();
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
            Console.WriteLine($"\nВведите язык ддя фильма {name}:");
            string langCode = AnsiConsole.Prompt(new TextPrompt<string>("1 - русский, 2 - английский")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (langCode == "1")
                language = "русский";
            if (langCode == "2")
                language = "английский";
        }

        public static Film ChooseFilm()
        {
            TextPrompt<string> filmChoicePrompt = new TextPrompt<string>("");
            foreach (Film film in Film.all)
                filmChoicePrompt.AddChoice(film.name);
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
            Console.WriteLine($"В зале {hall.rowsNum} рядов по {hall.seatsInRowNum} мест.");
            Console.WriteLine("\nВведите стоимость билетов на сеанс через пробел:");

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
        public void Print_Hall_Data(string printChoice) // печать таблицы с ценами (по аргументу "prices") или доступностьюю мест (по аргументу "availability")
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
    }

    class Ticket
    {
        public static List<Ticket> all = new List<Ticket>();

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
            Console.WriteLine($"Фильм {screening.film,15} | Зал {screening.hall, 15} | Время {screening.time.ToString("MM/dd/yyyy HH:mm")} | Ряд {seat[0], 2} | Место {seat[1], 2}");
        }
    }
}