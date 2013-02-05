using System;
using System.Collections.Generic;

namespace City
{
    public class ConfigurationData
    {
        public double DemandsPerPersonPerDay { get; set; }
        public DateTime SimulationEndDate { get; set; }
        public DateTime SimulationStartDate { get; set; }
        public string SimulationMethodName { get; set; }
        public string ConfigurationName { get; set; }

        public IEnumerable<CircleAreaObject> CircleAreas { get; set; }
        public IEnumerable<string> PoiTypes { get; set; }
        public IEnumerable<MapDistrict> Polygons { get; set; }

        public int CreditCardLimitMin { get; set; }
        public int CreditCardLimitMax { get; set; }
        public int CreditCardBalanceMin { get; set; }
        public int CreditCardBalanceMax { get; set; }
        public int TransactionValueMin { get; set; }
        public int TransactionValueMax { get; set; }
    }
}