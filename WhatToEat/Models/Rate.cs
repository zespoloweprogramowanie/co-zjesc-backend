using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class Rate
    {
        [Key]
        public int Id { get; set; }

        public int? Amount { get; set; } //wartość oceny od 1 do 5

    }
}