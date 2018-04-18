using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatToEat.Domain.Models.DTO
{

    public class RecipeImport
    {
        public string Title { get; set; }

        public List<string> Images { get; set; }

        public List<Product> Products { get; set; }

        public string Description { get; set; }

        public int Difficulty { get; set; }

        public int EstimatedCost { get; set; }

        public int PortionCount { get; set; }

        public int TimeToPrepare { get; set; }

        public string Category { get; set; }

        public List<string> Tags { get; set; }

        public double AverageRate { get; set; }

        public class Product
        {
            public string Name { get; set; }

            public string Unit { get; set; }

            public double Amount { get; set; }

        }
    }
}
