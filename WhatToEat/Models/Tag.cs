using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class Tag
    {
        public Tag()
        {

        }

        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
    }
}