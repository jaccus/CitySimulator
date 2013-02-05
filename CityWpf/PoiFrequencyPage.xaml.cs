using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace City
{
    /// <summary>
    /// Interaction logic for PoiFrequencyPage.xaml
    /// </summary>
    public partial class PoiFrequencyPage
    {
        public IEnumerable<FrequencyBox> PoiFrequencyBoxes { get; set; }

        public PoiFrequencyPage()
        {
            InitializeComponent();

            PoiFrequencyBoxes = LoadPoiFrequencies();
            DataContext = PoiFrequencyBoxes;
        }

        private void SavePoiFrequencyBtnClick(object sender, RoutedEventArgs e)
        {
            UpdatePoiFrequencies();

            MainPage.MainPageInstance.UpdatePoiTypeList();
        }

        private void UpdatePoiFrequencies()
        {
            using(var ctx = new CityContainer())
            {
                foreach (var poiFrequencyBox in PoiFrequencyBoxes)
                {
                    var poiBoxName = poiFrequencyBox.Content.ToString();

                    var poiType = ctx.PoiTypes.Single(t => t.Code == poiBoxName);

                    double freqDouble;
                    if (!double.TryParse(poiFrequencyBox.Frequency, out freqDouble))
                    {
                        MessageBox.Show(string.Format("Invalid frequency value for POI '{0}", poiBoxName));
                        return;
                    }

                    poiType.Frequency = freqDouble;
                }
                ctx.SaveChanges();
            }
        }

        private static IEnumerable<FrequencyBox> LoadPoiFrequencies()
        {
            var frequencyBoxes = new List<FrequencyBox>();

            using (var ctx = new CityContainer())
            {
                // ReSharper disable LoopCanBeConvertedToQuery
                foreach (var poiType in ctx.PoiTypes)
                // ReSharper restore LoopCanBeConvertedToQuery
                    frequencyBoxes.Add(new FrequencyBox { Content = poiType.Code, Frequency = poiType.Frequency.ToString() });
            }
            return frequencyBoxes;
        }
    }
}
