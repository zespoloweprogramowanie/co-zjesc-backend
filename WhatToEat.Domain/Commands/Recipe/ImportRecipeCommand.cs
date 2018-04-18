using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatToEat.Domain.Commands.Recipe
{
    class ImportRecipeCommand
    {
        public string Title { get; set; }
        public List<string> Images { get; set; }
        public List<Product> Products { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public int EstimatedCost { get; set; }
        public int PortionCount { get; set; }
        public int TimeToPrepare { get; set; }
        public int Category { get; set; }

    }

    public class Product
    {
        public string Name { get; set; }
        public int Unit { get; set; }
        public double Amount { get; set; }
    }
}
