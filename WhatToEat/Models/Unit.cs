using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class Unit
    {
        public Unit()
        {
            
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }
    }
}