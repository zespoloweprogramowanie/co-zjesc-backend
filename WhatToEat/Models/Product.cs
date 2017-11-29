using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class Product
    {
        public Product()
        {
            Recipes = new HashSet<Recipe>();
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public String Name { get; set; }

        [MaxLength(500)]
        public String Image { get; set; }

        [JsonIgnore]
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}