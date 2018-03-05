using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WhatToEat.Domain.Models
{
    public class Unit
    {
        public Unit()
        {
            
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        [DisplayName("Nazwa")]
        public string Name { get; set; }
    }
}