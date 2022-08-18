using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Soda
    {
        public string Name { get; set; }
        //Added Price field as it's also Soda property
        public int Price { get; set; }
        //Changed name because "AmountAvailable" seemed more acccurate than "Nr" 
        public int AmountAvailable { get; set; }

    }
}
