using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public enum SodaMachineChoice
    {
        InsertMoney = 1,
        OrderSoda = 2,
        OrderSmsSoda = 3,
        Recall = 4
    }
    public static class SodaMachine
    {
        private static int _money = 0;
        private static readonly Soda[] _inventory = new[] { new Soda { Name = "coke", Price = 20, Nr = 5 }, new Soda { Name = "sprite", Price = 20, Nr = 3 }, new Soda { Name = "fanta", Price = 20, Nr = 3 } };

        private static void PrintInventory()
        {
            foreach (var item in _inventory)
            {
                Console.WriteLine($"{item.Name} - {item.Price} NOK - ({item.Nr} available) ");
            }
        }
        private static void InsertMoney(int count)
        {
            _money += count;
        }
        private static void OrderProduct(Soda orderedSoda)
        {
            if (CheckIfProductAvailable(orderedSoda) && CheckIfEnoughMoney(orderedSoda))
            {
                _money -= orderedSoda.Price;
                orderedSoda.Nr -= 1;
                Console.WriteLine($"Giving {orderedSoda.Name} out");
                GiveMoneyBack();
            }
        }
        private static void OrderProductViaSms(Soda orderedSoda)
        {
            if (CheckIfProductAvailable(orderedSoda))
            {
                orderedSoda.Nr -= 1;
                Console.WriteLine($"Giving {orderedSoda.Name} out");
            }
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

        private static Soda GetSoda(string name)
        {
            return _inventory.Where(x => x.Name == name).FirstOrDefault();
        }
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
        private static bool CheckIfProductAvailable(Soda chosenSoda)
        {
            if (chosenSoda.Nr > 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Sorry, there is no {chosenSoda.Name} left");
                return false;
            }
        }


        /// <summary>
        /// This is the starter method for the machine
        /// </summary>
        public static void Start()
        {

            while (true)
            {
                Console.WriteLine("\n\nAvailable commands:");
                Console.WriteLine("[1] Insert money - Money put into money slot");
                Console.WriteLine($"[2] Order ({string.Join("/", _inventory.Where(x => x.Nr > 0).Select(x => x.Name))}) - Order from machines buttons");
                Console.WriteLine($"[3] Sms order ({string.Join(" / ", _inventory.Where(x => x.Nr > 0).Select(x => x.Name))}) - Order sent by sms");
                Console.WriteLine("[4] Recall - gives money back");
                Console.WriteLine("-------");
                Console.WriteLine("Inserted money: " + _money);
                Console.WriteLine("-------\n\n");
                Console.Write("Enter a number of your choice: ");
                var input = Console.ReadLine();
                SodaMachineChoice userChoice;
                while (!(Enum.TryParse(input, true, out userChoice) && Enum.IsDefined(typeof(SodaMachineChoice), userChoice)))
                {
                    Console.Write("Incorrect value. Enter a number of your choice: ");
                    input = Console.ReadLine();
                }
                switch (userChoice)
                {
                    case SodaMachineChoice.InsertMoney:
                        Console.Write("Enter an amount of money: ");
                        var moneyAmountInput = Console.ReadLine();
                        int moneyAmount;
                        while (!int.TryParse(moneyAmountInput, out moneyAmount))
                        {
                            Console.Write("Incorrect value. Enter an amount of money: ");
                            moneyAmountInput = Console.ReadLine();
                        }
                        InsertMoney(moneyAmount);
                        break;
                    case SodaMachineChoice.OrderSoda:
                        PrintInventory();
                        Console.Write("Enter a name of soda: ");
                        var sodaNameInput = Console.ReadLine();
                        while (GetSoda(sodaNameInput) == null)
                        {
                            Console.Write("Incorrect value. Enter a name of soda: ");
                            sodaNameInput = Console.ReadLine();
                        }
                        OrderProduct(GetSoda(sodaNameInput));
                        break;
                    case SodaMachineChoice.OrderSmsSoda:
                        PrintInventory();
                        Console.Write("Enter a name of soda: ");
                        sodaNameInput = Console.ReadLine();
                        while (GetSoda(sodaNameInput) == null)
                        {
                            Console.Write("Incorrect value. Enter a name of soda: ");
                            sodaNameInput = Console.ReadLine();
                        }
                        OrderProductViaSms(GetSoda(sodaNameInput));
                        break;
                    case SodaMachineChoice.Recall:
                        GiveMoneyBack();
                        break;
                    default:
                        Console.Write("It's impossible to do that operation right now");
                        break;
                }
            }
        }
    }
}
