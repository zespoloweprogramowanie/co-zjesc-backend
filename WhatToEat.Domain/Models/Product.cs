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
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }
    }
}