using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace City
{
    public class StatisticalData
    {
        private Dictionary<PoiType, double> _poiTypesDistributionDict;
        
        private readonly Random _random = new Random();

        private int _creditCardLimitMin;
        private int _creditCardLimitMax = 30000;
        private int _creditCardBalanceMin;
        private int _creditCardBalanceMax = 10000;
        private int _transactionValueMin;
        private int _transactionValueMax = 1000;

        private const int InitialValue = 0;

        public string CreditCardLimitMin
        {
            get { return _creditCardLimitMin.ToString(); }
            set { _creditCardLimitMin = !string.IsNullOrEmpty(value) ? int.Parse(value) : InitialValue; }
        }

        public string CreditCardLimitMax
        {
            get { return _creditCardLimitMax.ToString(); }
            set { _creditCardLimitMax = !string.IsNullOrEmpty(value) ? int.Parse(value) : InitialValue; }
        }

        public string CreditCardBalanceMin
        {
            get { return _creditCardBalanceMin.ToString(); }
            set { _creditCardBalanceMin = !string.IsNullOrEmpty(value) ? int.Parse(value) : InitialValue; }
        }

        public string CreditCardBalanceMax
        {
            get { return _creditCardBalanceMax.ToString(); }
            set { _creditCardBalanceMax = !string.IsNullOrEmpty(value) ? int.Parse(value) : InitialValue; }
        }

        public string TransactionValueMin
        {
            get { return _transactionValueMin.ToString(); }
            set { _transactionValueMin = !string.IsNullOrEmpty(value) ? int.Parse(value) : InitialValue; }
        }

        public string TransactionValueMax
        {
            get { return _transactionValueMax.ToString(); }
            set { _transactionValueMax = !string.IsNullOrEmpty(value) ? int.Parse(value) : InitialValue; }
        }

        public double TransactionValue
        {
            get
            {
                var transactionValue = _random.NextDouble() * _transactionValueMax + _transactionValueMin;
                return Math.Round(transactionValue, 2);
            }
        }

        public string CreditCardNumber
        {
            get
            {
                var sb = new StringBuilder(16);
                do
                {
                    sb.Append(_random.Next(0, 10));

                } while (sb.Length < 16);

                return sb.ToString();
            }
        }

        public double CreditCardLimit
        {
            get
            {
                var limit = -1 * _random.Next(_creditCardLimitMin, _creditCardLimitMax);
                return limit - (limit % 100);
            }
        }

        public double CreditCardBalance
        {
            get
            {
                var balance = _random.NextDouble() * _creditCardBalanceMax + _creditCardBalanceMin;
                return Math.Round(balance, 2);
            }
        }


        public PoiType TakePoiByDistribution(HashSet<PoiType> poiSet)
        {
            if (_poiTypesDistributionDict == null)
            {
                _poiTypesDistributionDict = new Dictionary<PoiType, double>();

                var totalFrequency = poiSet.Sum(poiType => poiType.Frequency);
                var orderedPois = poiSet.OrderBy(p => p.Frequency);

                double currentFreq = 0;

                foreach (var poiType in orderedPois)
                {
                    currentFreq += poiType.Frequency/totalFrequency;
                    _poiTypesDistributionDict.Add(poiType, currentFreq);
                }
            }

            var d = _random.NextDouble();

            return (from dictEntry in _poiTypesDistributionDict where d <= dictEntry.Value select dictEntry.Key).FirstOrDefault();
        }
    }
}