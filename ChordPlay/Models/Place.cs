using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordPlay.Models
{
    public class Place
    {
        public int id { get; set; }
        public Address address { get; set; }
        public string name { get; set; }
        public string subname { get; set; }
    }
    public class Address
    {
        public string street { get; set; }
        public string zipcode { get; set; }
        public string city { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }

        public override string ToString()
        {
            return $"{street}, {zipcode} {city}";
        }
    }
}
