using System;

namespace DhtDatabase
{
    public class DhtModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double TemperatureInC { get; set; }
        public double TemperatureInF { get; set; }
        public double Humidity { get; set; }

    }
}