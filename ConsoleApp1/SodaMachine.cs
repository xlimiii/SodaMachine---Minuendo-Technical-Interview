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
        private static readonly Soda[] _inventory = new[] { new Soda { Name = "coke", Price = 20, Nr = 5 }, new Soda { Name = "sprite", Price = 20, Nr = 3 }, new Soda { Name = "fanta", Price = 20, Nr = 3 } };

        /// <summary>
        /// This is the starter method for the machine
        /// </summary>
        public static void Start()
        {
            while (true)
            {
                PrintMenu();
                var userChoice = GetUserChoice();
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
                        OrderProduct(soda, userChoice);
                        break;
                    case SodaMachineChoice.Recall:
                        GiveMoneyBack();
                        break;
                    default:
                        Console.Write("It's impossible to do that operation right now");
                        break;
                }
                Console.Write("Press any button to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        #region main operation methods
        private static void InsertMoney(int count)
        {
            _money += count;
        }

        private static void OrderProduct(Soda orderedSoda, SodaMachineChoice choice) 
        {
            bool isOrderSuccess = false;
            if (CheckIfProductAvailable(orderedSoda))
            {
                switch (choice)
                {
                    case SodaMachineChoice.OrderSoda:
                        isOrderSuccess = OrderProductFromMachine(orderedSoda);
                        break;
                    case SodaMachineChoice.OrderSmsSoda:
                        isOrderSuccess = OrderProductViaSms(orderedSoda);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Sorry, there is no {orderedSoda.Name} left");
            }

            if (isOrderSuccess)
            {
                orderedSoda.Nr -= 1;
                Console.WriteLine($"Giving {orderedSoda.Name} out");
            }
            else
            {
                Console.WriteLine($"Order failed");
            }
        }
        private static bool OrderProductFromMachine(Soda orderedSoda)
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
        private static bool OrderProductViaSms(Soda orderedSoda)
        {
            Console.WriteLine($"Sms received - your order is processed");
            return true;
        }
        private static void GiveMoneyBack()
        {
            if (_money > 0)
            {
                Console.WriteLine($"Giving {_money} NOK back");
                _money = 0;
            }
            else
            {
                Console.WriteLine($"There is no money left in the machine");
            }
        }
        #endregion
        #region repository-like methods
        private static Soda GetSoda(string name)
        {
            return _inventory.Where(x => x.Name == name).FirstOrDefault();
        }
        #endregion

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
      
        private static bool CheckIfProductAvailable(Soda chosenSoda) => chosenSoda.Nr > 0;
        #endregion

        #region simple printing methods
        private static void PrintMenu()
        {
            Console.WriteLine("\n\nAvailable commands:");
            Console.WriteLine("[1] Insert money - Money put into money slot");
            Console.WriteLine($"[2] Order ({string.Join("/", _inventory.Where(x => x.Nr > 0).Select(x => x.Name))}) - Order from machines buttons");
            Console.WriteLine($"[3] Sms order ({string.Join(" / ", _inventory.Where(x => x.Nr > 0).Select(x => x.Name))}) - Order sent by sms");
            Console.WriteLine("[4] Recall - gives money back");
            Console.WriteLine("-------");
            Console.WriteLine("Inserted money: " + _money);
            Console.WriteLine("-------\n\n");
        }
        private static void PrintInventory()
        {
            Console.WriteLine("\n\nAvailable sodas:");
            foreach (var item in _inventory.Where(x => x.Nr > 0))
            {
                Console.WriteLine($"{item.Name} - {item.Price} NOK - ({item.Nr} available) ");
            }
        }
        #endregion

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
