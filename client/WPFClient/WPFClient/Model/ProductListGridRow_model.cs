﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    internal class ProductListGridRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Availibility { get; set; }
        public bool IsSelected { get; set; }
        public int Count { get; set; }
    }
}
