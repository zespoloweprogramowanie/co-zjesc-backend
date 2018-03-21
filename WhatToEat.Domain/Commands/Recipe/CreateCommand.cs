using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatToEat.Domain.Commands.Recipe
{

    public class CreateCommand
    {
        public string title { get; set; }
        public List<string> images { get; set; }
        public List<Product> products { get; set; }
        public string description { get; set; }
        public int difficulty { get; set; }
        public int estimatedCost { get; set; }
        public int portionCount { get; set; }
        public int timeToPrepare { get; set; }

        public class Product
        {
            public string name { get; set; }
            public int unit { get; set; }
            public double amount { get; set; }
        }

    }

    public class UpdateCommand
    {
        public int id { get; set; }
        public string title { get; set; }
        public List<string> images { get; set; }
        public List<Product> products { get; set; }
        public string description { get; set; }
        public int difficulty { get; set; }
        public int estimatedCost { get; set; }
        public int portionCount { get; set; }
        public int timeToPrepare { get; set; }

        public class Product
        {
            public string name { get; set; }
            public int unit { get; set; }
            public double amount { get; set; }
        }

    }
}
