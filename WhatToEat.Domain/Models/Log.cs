using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatToEat.Domain.Models
{
    public class Log
    {
        public Log()
        {
            
        }

        [Key]
        public int Id { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }
    }
}
