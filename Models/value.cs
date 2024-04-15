using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HeartDisease.Models
{
    public class value
    {
        public value()
        {
            result = "";
        }
        public int age { get; set; }
        public string chest_pain_type { get; set; }
        public int rest_blood_pressure { get; set; }
        public string blood_sugar { get; set; }
        public string rest_electro { get; set; }
        public int max_heart_rate { get; set; }
        public string exercice_angina { get; set; }

        public string result { get; set; }
    }
}