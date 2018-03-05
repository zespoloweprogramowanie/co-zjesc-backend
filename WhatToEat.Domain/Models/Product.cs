using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WhatToEat.Domain.Models
{
    public class Product
    {
        public Product()
        {
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        [DisplayName("Nazwa")]
        public String Name { get; set; }

        [MaxLength(500)]
        [DisplayName("Zdjęcie")]
        public String Image { get; set; }
    }

    public class ProductDTO
    {
        public int id { get; set; }

        public string name { get; set; }

        public string image { get; set; }
    }
}