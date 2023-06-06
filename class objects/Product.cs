using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Continuum_Integration.class_objects
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public float price { get; set; }
        public int categoryId { get; set; }

    }
}
