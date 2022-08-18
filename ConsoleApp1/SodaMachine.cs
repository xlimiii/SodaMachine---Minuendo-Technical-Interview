using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class SodaMachine
    {
        private static int _money = 0;
        private static readonly Soda[] _inventory = new[] { new Soda { Name = "coke", Price = 20, AmountAvailable = 4 }, new Soda { Name = "sprite", Price = 30, AmountAvailable = 5 }, new Soda { Name = "fanta", Price = 20, AmountAvailable = 3 } };

        /// <summary>
        /// This is the starter method for the machine
        /// </summary>
        public static void Start()
        {
            while (true)
            {
                PrintMenu();
                var userChoice = GetUserChoice();
                ProcessUserChoice(userChoice);
                Console.Write("Press any button to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        /* 
         These methods contain main business logic in the project. 
         There are three main operations which can be chosen by user:
            1. Insert money
            2. Order soda (We can order it 2a. with money inserted into the machine or 2b. via SMS)
            3. Recall 
         */
        #region main operations methods
        private static void ProcessUserChoice(SodaMachineChoice userChoice)
        {
            switch (userChoice)
            {
                case SodaMachineChoice.InsertMoney:
                    var moneyInsertedByUser = GetUserMoney();
                    InsertMoney(moneyInsertedByUser);
                    break;
                case SodaMachineChoice.OrderSoda:
                case SodaMachineChoice.OrderSmsSoda:
                    PrintInventory();
                    var soda = GetUserSodaChoice();
                    OrderSoda(soda, userChoice);
                    break;
                case SodaMachineChoice.Recall:
                    GiveMoneyBack();
                    break;
                default:
                    Console.Write("It's impossible to do that operation right now");
                    break;
            }
        }
        private static void InsertMoney(int count)
        {
            _money += count;
        }

        private static void OrderSoda(Soda orderedSoda, SodaMachineChoice choice)
        {
            bool isOrderSuccess = false;
            if (CheckIfSodaAvailable(orderedSoda))
            {
                switch (choice)
                {
                    case SodaMachineChoice.OrderSoda:
                        isOrderSuccess = OrderSodaFromMachine(orderedSoda);
                        break;
                    case SodaMachineChoice.OrderSmsSoda:
                        isOrderSuccess = OrderSodaViaSms(orderedSoda);
                        break;
                    default:
                        break;
                }
            }

            if (isOrderSuccess)
            {
                orderedSoda.AmountAvailable -= 1;
                Console.WriteLine($"***Giving {orderedSoda.Name} out***");
            }
            else
            {
                Console.WriteLine($"Order failed");
            }
        }
        private static bool OrderSodaFromMachine(Soda orderedSoda)
        {
            if (CheckIfEnoughMoney(orderedSoda))
            {
                _money -= orderedSoda.Price;
                GiveMoneyBack();
                return true;
            }
            else
            {
                return false;
            }
        }
        // Even though there is no case so far when ordering soda via SMS could go wrong, I've still decided to extract that method to have cleaner code
        private static bool OrderSodaViaSms(Soda orderedSoda)
        {
            Console.WriteLine($"***Sms received - your {orderedSoda.Name} order is processed***");
            return true;
        }
        private static void GiveMoneyBack()
        {
            if (_money > 0)
            {
                Console.WriteLine($"***Giving {_money} NOK back***");
                _money = 0;
            }
            else
            {
                Console.WriteLine($"There is no money left in the machine");
            }
        }
        #endregion
        /*
         These methods are responsible for getting data from data storage (in that case it's just a static "_inventory" field but they would be similar if the data was stored for example in the database. 
         */
        #region repository-like methods
        private static Soda GetSoda(string name)
        {
            return _inventory.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        private static IEnumerable<Soda> GetInventory(bool onlyAvailableSodas)
        {
            return onlyAvailableSodas ? _inventory.Where(x => x.AmountAvailable > 0) : _inventory;
        }
        #endregion

        /*
        These validation methods are called in order to check if operation could be done. 
        And if not - they print a message for user with the reason.

        Potential area for improvement: if the application was bigger, they could be extracted to a different class and they could be generalized (for example instead having Soda object as an argument and getting value from _money static field, they could have two arguments: 1. moneyAvailable 2. price - then they could be used for different similar object like e.g. Snack as well 
         */

        #region validation methods
        private static bool CheckIfEnoughMoney(Soda chosenSoda)
        {
            if (_money >= chosenSoda.Price)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"You don't have enough money - you need {chosenSoda.Price - _money} NOK more");
                return false;
            }
        }

        private static bool CheckIfSodaAvailable(Soda chosenSoda)
        {

            if (chosenSoda.AmountAvailable > 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Sorry, there is no {chosenSoda.Name} left");
                return false;
            }
        }
        #endregion

        /* 
         These methods have been extracted in order to improve code readability
        */
        #region simple printing methods
        private static void PrintMenu()
        {
            var availableSodas = GetInventory(onlyAvailableSodas: true);
            Console.WriteLine("\n\nAvailable commands:");
            Console.WriteLine("[1] Insert money - Money put into money slot");
            Console.WriteLine($"[2] Order ({string.Join("/", availableSodas.Select(x => x.Name))}) - Order from machines buttons");
            Console.WriteLine($"[3] Sms order ({string.Join(" / ", availableSodas.Select(x => x.Name))}) - Order sent by sms");
            Console.WriteLine("[4] Recall - gives money back");
            Console.WriteLine("-------");
            Console.WriteLine("Inserted money: " + _money);
            Console.WriteLine("-------\n\n");
        }
        private static void PrintInventory()
        {
            var availableSodas = GetInventory(onlyAvailableSodas: true);
            Console.WriteLine("\n\nAvailable sodas:");
            Console.WriteLine("-------");
            foreach (var item in availableSodas)
            {
                Console.WriteLine($"{item.Name} - {item.Price} NOK - ({item.AmountAvailable} available) ");
            }
            Console.WriteLine("-------\n\n");

        }
        #endregion

        /* 
         As it's a console application, I've decided to separate all the methods that are responsible for dealing with user input.
         All of them has quite similar structure 
         1. they print a request for entering user input
         2. they take user input
         3. they validate user input 
         4. in case when user input is incorrect - they print a message and ask user to enter his choice again (until it's correct)
         
         Potential area for improvement: to create one function for dealing with user input regardless return 
        */
        #region methods dealing with user input
        private static SodaMachineChoice GetUserChoice()
        {
            Console.Write("Enter a number of your choice: ");
            var input = Console.ReadLine();
            SodaMachineChoice userChoice;
            while (!(Enum.TryParse(input, true, out userChoice) && Enum.IsDefined(typeof(SodaMachineChoice), userChoice)))
            {
                Console.Write("Incorrect value. Enter a number of your choice: ");
                input = Console.ReadLine();
            }
            return userChoice;
        }
        private static int GetUserMoney()
        {
            Console.Write("Enter an amount of money: ");
            var moneyAmountInput = Console.ReadLine();
            int moneyAmount;
            while (!int.TryParse(moneyAmountInput, out moneyAmount))
            {
                Console.Write("Incorrect value. Enter an amount of money: ");
                moneyAmountInput = Console.ReadLine();
            }
            return moneyAmount;
        }
        private static Soda GetUserSodaChoice()
        {
            Console.Write("Enter a name of soda: ");
            var sodaNameInput = Console.ReadLine();
            var soda = GetSoda(sodaNameInput);
            while (soda == null)
            {
                Console.Write("Incorrect value. Enter a name of soda: ");
                sodaNameInput = Console.ReadLine();
                soda = GetSoda(sodaNameInput);
            }
            return soda;
        }
        #endregion
    }
}
