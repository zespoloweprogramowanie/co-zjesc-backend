using System.ComponentModel.DataAnnotations;

namespace WhatToEat.Domain.Models
{
    public class Rate
    {
        [Key]
        public int Id { get; set; }

        public int? Amount { get; set; } //wartość oceny od 1 do 5

    }
}