using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.MessageBox;

namespace City
{
    public partial class MainPage
    {
        public IEnumerable<CheckBox> PoiTypeBoxes { get; set; }

        public static AreaSelectorPage AreaSelectorPageInstance { get; private set; }
        public static PoiFrequencyPage PoiFrequencyPageInstance { get; private set; }
        public static MainPage MainPageInstance { get; private set; }
        public static List<SymulationType> SymulationTypes { get; set; }
        public static IEnumerable<string> TableNames { get; set; }

        private static readonly Random Random = new Random();
        private static ProgressWindow _progressDialog;
        private static readonly SolidColorBrush SavedConfigurationNameColor = Brushes.Green;
        private const string GeneratingPeopleMsg = "Generating data for people...";
        private const string GeneratingDemandsMsg = "Generating Demands...";
        private const string GeneratingTransactionsMsg = "Symulating transactions...";
        private const string GettingPoisMsg = "Getting list of POI...";
        private const string PersistingMsg = " (persisting data, this may take a while)";
        private static bool _simulationCancelled;
        private static string _selectedPath;
        public static StatisticalData StatisticalData = new StatisticalData();

        public MainPage()
        {
            InitializeComponent();

            DBUtilities.CreateDatabaseIfNeeded();

            DBUtilities.ImportSchemaCEIfNeeded();

            DBUtilities.InsertStaticDataIfNeeded();

            MainPageInstance = this;
            AreaSelectorPageInstance = new AreaSelectorPage();
            PoiFrequencyPageInstance = new PoiFrequencyPage();

            areaSelectionLabel.DataContext = AreaSelectorPageInstance;

            UpdatePoiTypeList();

            TableNames = DBUtilities.ListTablesCE();
            tableListComboBox.DataContext = TableNames;

            creditCardBalanceMin.DataContext = StatisticalData;
            creditCardBalanceMax.DataContext = StatisticalData;
            creditCardLimitMin.DataContext = StatisticalData;
            creditCardLimitMax.DataContext = StatisticalData;
            transactionValueMin.DataContext = StatisticalData;
            transactionValueMax.DataContext = StatisticalData;

            UpdateConfigurationList();

            SetUpSymulationTypeComboBox();
        }

        public void UpdatePoiTypeList()
        {
            PoiTypeBoxes = LoadPoiData();
            DataContext = PoiTypeBoxes;
        }

        private void SetUpSymulationTypeComboBox()
        {
            SymulationTypes = new List<SymulationType>
                                  {
                                      new SymulationType {Label = SymulationType.ShortestDistance, IsEnabled = true, Index = 0},
                                      new SymulationType {Label = SymulationType.Random, IsEnabled = true, Index = 1}
                                  };

            foreach (var symulationType in SymulationTypes)
                symulationTypeComboBox.Items.Add(
                    new ComboBoxItem {Content = symulationType.Label, IsEnabled = symulationType.IsEnabled});

            symulationTypeComboBox.SelectedIndex = 0;
        }

        private void UpdateConfigurationList()
        {
            configurationListBox.DataContext = DBUtilities.ListConfigurations();
        }

        private static IEnumerable<CheckBox> LoadPoiData()
        {
            // Cannot be converted to Linq because of automatic object disposing.
            var customBoxes = new List<CheckBox>();

            using (var ctx = new CityContainer())
            {
                // ReSharper disable LoopCanBeConvertedToQuery
                foreach (var poiType in ctx.PoiTypes)
                // ReSharper restore LoopCanBeConvertedToQuery
                {
                    var content = string.Format("{0} ({1})", poiType.Code, poiType.Frequency);
                    customBoxes.Add(new CheckBox { Content = content, Name = poiType.Code});
                }
            }
            return customBoxes;
        }

        private void AreaSelectBtnClicked(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(AreaSelectorPageInstance);
        }

        private void SaveConfigurationBtnClick(object sender, RoutedEventArgs e)
        {
            var configurationName = configurationNameTextBox.Text;

            var selectedPoi = from poiBox in PoiTypeBoxes where poiBox.IsChecked == true select poiBox.Name;

            double demandsPerPersonPerDay;
            if (!double.TryParse(demandsPerPersonPerDayTextBox.Text, out demandsPerPersonPerDay))
            {
                MessageBox.Show("Could not save configuration. Non numeric value for Person Demands / Day field.");
                return;
            }

            var configurationData = new ConfigurationData
                                        {
                                            ConfigurationName = configurationName,
                                            CircleAreas = AreaSelectorPageInstance.Areas,
                                            PoiTypes = selectedPoi,
                                            Polygons = districtArchitect.Polygons,
                                            SimulationMethodName = symulationTypeComboBox.Text,
                                            SimulationStartDate = startDatePicker.SelectedDate ?? DateTime.Now,
                                            SimulationEndDate = endDatePicker.SelectedDate ?? DateTime.Now,
                                            DemandsPerPersonPerDay = demandsPerPersonPerDay,
                                            CreditCardBalanceMin = int.Parse(StatisticalData.CreditCardBalanceMin),
                                            CreditCardBalanceMax = int.Parse(StatisticalData.CreditCardBalanceMax),
                                            CreditCardLimitMin = int.Parse(StatisticalData.CreditCardLimitMin),
                                            CreditCardLimitMax = int.Parse(StatisticalData.CreditCardLimitMax),
                                            TransactionValueMin = int.Parse(StatisticalData.TransactionValueMin),
                                            TransactionValueMax = int.Parse(StatisticalData.TransactionValueMax)
                                        };

            if(!DBUtilities.SaveConfiguration(configurationData))
            {
                MessageBox.Show(string.Format("Configuration with name \"{0}\" already exists.", configurationName));
                return;
            }

            configurationNameTextBox.Foreground = SavedConfigurationNameColor;
            UpdateConfigurationList();
        }

        private void LoadConfigurationBtnClick(object sender, RoutedEventArgs e)
        {
            if(configurationListBox.SelectedItem == null)
            {
                MessageBox.Show("Configuration must be selected first.");
                return;
            }

            var data = DBUtilities.LoadConfiguration(configurationListBox.SelectedItem as string);

            StatisticalData.CreditCardBalanceMin = data.CreditCardBalanceMin.ToString();
            creditCardBalanceMin.Text = data.CreditCardBalanceMin.ToString();

            StatisticalData.CreditCardBalanceMax = data.CreditCardBalanceMax.ToString();
            creditCardBalanceMax.Text = data.CreditCardBalanceMax.ToString();

            StatisticalData.CreditCardLimitMin = data.CreditCardLimitMin.ToString();
            creditCardLimitMin.Text = data.CreditCardLimitMin.ToString();

            StatisticalData.CreditCardLimitMax = data.CreditCardLimitMax.ToString();
            creditCardLimitMax.Text = data.CreditCardLimitMax.ToString();

            StatisticalData.TransactionValueMin = data.TransactionValueMin.ToString();
            transactionValueMin.Text = data.TransactionValueMin.ToString();

            StatisticalData.TransactionValueMax = data.TransactionValueMax.ToString();
            transactionValueMax.Text = data.TransactionValueMax.ToString();

            configurationNameTextBox.Text = data.ConfigurationName;
            configurationNameTextBox.Foreground = SavedConfigurationNameColor;

            // Set POI area circles
            AreaSelectorPageInstance.Areas = data.CircleAreas.ToList();

            // Deselect all POI types before loading
            foreach (var poiTypeBox in PoiTypeBoxes)
                poiTypeBox.IsChecked = false;

            // Select saved POI types
            foreach (var poiType in data.PoiTypes)
            {
                var type = poiType;
                PoiTypeBoxes.Single(b => b.Name == type).IsChecked = true;
            }

            // Remove all polygons before loading
            districtArchitect.RemovePolygons();

            // Draw saved polygons
            foreach (var polygon in data.Polygons)
            {
                districtArchitect.DrawPolygon(polygon.Locations, polygon.DistrictName, polygon.Population);
            }

            // Restore simulation settings
            startDatePicker.SelectedDate = data.SimulationStartDate;
            endDatePicker.SelectedDate = data.SimulationEndDate;
            demandsPerPersonPerDayTextBox.Text = data.DemandsPerPersonPerDay.ToString();
            symulationTypeComboBox.SelectedIndex = SymulationTypes.Single(t => t.Label == data.SimulationMethodName).Index;
        }

        private void RemoveConfigurationBtnClick(object sender, RoutedEventArgs e)
        {
            if (configurationListBox.SelectedItem == null)
            {
                MessageBox.Show("Configuration must be selected first.");
                return;
            }
            var cfgName = configurationListBox.SelectedItem as string;

            if (MessageBox.Show(string.Format("Delete configuration '{0}'?", cfgName),
                "Delete confirmation", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            if(!DBUtilities.RemoveConfiguration(cfgName))
            {
                MessageBox.Show("Selected configuration could not be removed.");
            }

            MessageBox.Show("Configuration removed successfully.");

            UpdateConfigurationList();
        }

        private static void RunThread(string cfgName, Action<string, ProgressWindow> action)
        {
            _progressDialog = new ProgressWindow();

            var worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            worker.DoWork +=
                delegate(object s, DoWorkEventArgs args)
                {
                    _progressDialog.Args = args;
                    action.Invoke(cfgName, _progressDialog);

                };
            worker.RunWorkerCompleted += delegate { _progressDialog.Close(); };
            worker.RunWorkerAsync();

            _progressDialog.Worker = worker;
            _progressDialog.ShowDialog();
        }

        private void RunSymulationBtnClick(object sender, RoutedEventArgs e)
        {
            var cfgName = configurationNameTextBox.Text;

            if (!DBUtilities.ListConfigurations().Contains(cfgName))
            {
                MessageBox.Show("Please save your configuration before running the symulation.");
                return;
            }

            RunThread(cfgName, GeneratePeopleData);

            if (!_simulationCancelled) RunThread(cfgName, DownloadPois);

            if (!_simulationCancelled) RunThread(cfgName, GenerateDemands);

            if (!_simulationCancelled) RunThread(cfgName, GenerateTransactions);

            MessageBox.Show(_simulationCancelled
                                ? "Simulation cancelled."
                                : "Simulation finished. See application database or use the export utility for the results.");

            _simulationCancelled = false;
        }

        private void DownloadPois(string cfgName, ProgressWindow progress)
        {
            var poiTypes = new List<string>();
            using(var ctx = new CityContainer())
            {
                var cfg = ctx.Configurations.Single(c => c.Name == cfgName);
                poiTypes.AddRange(cfg.PoiTypes.Select(poiType => poiType.Code));
            }

            var allPois = new HashSet<PoiObject>();
            foreach (var area in AreaSelectorPageInstance.Areas)
            {
                if (progress.Worker.CancellationPending)
                {
                    progress.Args.Cancel = true;
                    _simulationCancelled = true;
                    return;
                }

                var hashPois = MapsQueryEngine.Query(
                    area.Latitude, area.Longitude, 
                    (int)(area.Range * 1000), poiTypes);
            
                foreach (var poi in hashPois)
                    allPois.Add(poi);

                ++progress.Current;
                progress.InvokeUpdate(AreaSelectorPageInstance.Areas.Count, GettingPoisMsg);
            }

            progress.InvokeUpdate(AreaSelectorPageInstance.Areas.Count, GettingPoisMsg + PersistingMsg);

            DBUtilities.GeneratePOI(cfgName, allPois);
        }

        private static void GeneratePeopleData(string cfgName, ProgressWindow progress)
        {
            using (var ctx = new CityContainer())
            {
                var cfgDistricts = ctx.Districts.Where(d => d.Configuration.Name == cfgName);
                var totalAddresses = cfgDistricts.Select(d => d.Population).Sum();
                foreach (var district in cfgDistricts)
                {
                    Logger.LogInfo("generating people for district.");

                    var locations = GeoUtilities.MapPointsToLocations(district.MapPoints);

                    for (var i = 0; i < district.Population; i++)
                    {
                        if (progress.Worker.CancellationPending)
                        {
                            progress.Args.Cancel = true;
                            _simulationCancelled = true;
                            return;
                        }

                        Logger.LogInfo("generating person {0} / {1}", i + 1, district.Population);

                        var bankId = Random.Next(1, ctx.Banks.Count() + 1);
                        var currencyId = Random.Next(1, ctx.Currencies.Count() + 1);
                        var ccNumber = StatisticalData.CreditCardNumber;
                        var personCreditCard = new CreditCard
                                                   {
                                                       Balance = StatisticalData.CreditCardBalance,
                                                       Limit = StatisticalData.CreditCardLimit,
                                                       Bank = ctx.Banks.Single(b => b.Id == bankId),
                                                       Currency = ctx.Currencies.Single(c => c.Id == currencyId),
                                                       Number = ccNumber
                                                   };

                        var templateId = Random.Next(1, ctx.PersonTemplates.Count() + 1);
                        var person = new Person
                                         {
                                             Template = ctx.PersonTemplates.Single(t => t.Id == templateId),
                                             CreditCard = personCreditCard
                                         };

                        var addressGeo = GeoUtilities.RandomLocationWithinPolygon(locations);
                        var address = new Address
                                          {
                                              District = district,
                                              MapPoint = new MapPoint
                                                             {
                                                                 Lat = addressGeo.Latitude,
                                                                 Lng = addressGeo.Longitude
                                                             }
                                          };
                        address.Residents.Add(person);

                        ctx.Addresses.AddObject(address);

                        ++progress.Current;
                        progress.InvokeUpdate(totalAddresses, GeneratingPeopleMsg);
                    }
                }
                progress.InvokeUpdate(totalAddresses, GeneratingPeopleMsg + PersistingMsg);
                ctx.SaveChanges();
            }
        }

        private static void GenerateDemands(string cfgName, ProgressWindow progress)
        {
            try
            {
                Logger.LogInfo("Adding demand pool...");
                using (var ctx = new CityContainer())
                {
                    var cfg = ctx.Configurations.Single(c => c.Name == cfgName);
                    
                    var peopleInConfiguration = 
                        ctx.People.Where(p => p.Address.District.Configuration.Name == cfgName).OrderBy(o => o.Id);

                    //poi types from poi list (not configuration) because some poi types might be missing (not found)
                    var poiTypeCollections = ctx.Pois.Select(p => p.Type);
                    var poiSet = new HashSet<PoiType>();
                    foreach (var poiTypeCollection in poiTypeCollections)
                        foreach (var poiType in poiTypeCollection)
                            poiSet.Add(poiType);

                    var statisticalData = new StatisticalData();

                    var totalDays = cfg.SimulationEndDate.Subtract(cfg.SimulationStartDate).Days + 1;
                    var totalPeople = peopleInConfiguration.Count();
                    var demandsPerDay = (int) (totalPeople * cfg.PersonDemandsPerDay);
                    var totalDemands = totalDays * demandsPerDay;

                    for (var dayNr = 0; dayNr < totalDays; dayNr++)
                    {
                        var currentDate = cfg.SimulationStartDate.AddDays(dayNr);

                        for (var demandNr = 0; demandNr < demandsPerDay; demandNr++)
                        {
                            if (progress.Worker.CancellationPending)
                            {
                                progress.Args.Cancel = true;
                                _simulationCancelled = true;
                                return;
                            }

                            var randomPersonIndex = Random.Next(0, totalPeople);
                            var person = peopleInConfiguration.Skip(randomPersonIndex).First();

                            var demandDateTime = currentDate
                                .AddHours(Random.Next(0, 24))
                                .AddMinutes(Random.Next(0, 60))
                                .AddSeconds(Random.Next(0, 60));
                            var demand = new Demand
                                             {
                                                 Person = person,
                                                 DateTime = demandDateTime,
                                                 PoiType = statisticalData.TakePoiByDistribution(poiSet)
                                             };

                            ctx.Demands.AddObject(demand);
                            ++progress.Current;
                            progress.InvokeUpdate(totalDemands, GeneratingDemandsMsg);
                        }
                    }
                    progress.InvokeUpdate(totalDemands, GeneratingDemandsMsg + PersistingMsg);
                    ctx.SaveChanges(); // Persist after each day.
                }
                Logger.LogInfo("Demands pool added.");
            }
            catch(Exception e1)
            {
                Logger.LogInfo(e1.Message);
            }
        }

        private void GenerateTransactions(string cfgName, ProgressWindow progress)
        {

            using(var ctx = new CityContainer())
            {
                var selectionMethodName = ctx.Configurations.Single(c => c.Name == cfgName).SelectionMethod.Name;
                Logger.LogInfo("Using POI selection method: {0}", selectionMethodName);

                var demandsInConfiguration = ctx.Demands.Where(d => d.Person.Address.District.Configuration.Name == cfgName);
                var totalTransactions = demandsInConfiguration.Count();
                foreach (var demand in demandsInConfiguration)
                {
                    if (progress.Worker.CancellationPending)
                    {
                        progress.Args.Cancel = true;
                        _simulationCancelled = true;
                        return;
                    }

                    var demandType = demand.PoiType.Code;
                    var matchingPois = ctx.Pois.Where(p => p.Type.Select(k => k.Code).Contains(demandType));

                    var selectedPoi = SelectPoi(matchingPois, demand.Person.Address.MapPoint, selectionMethodName);

                    if (selectedPoi == null) continue;

                    var transactionValue = StatisticalData.TransactionValue;
                    var creditCard = demand.Person.CreditCard;

                    var balanceAfterTransaction = creditCard.Balance - transactionValue;
                    
                    if (balanceAfterTransaction < creditCard.Limit) continue;
                    creditCard.Balance -= transactionValue;
                    creditCard.Balance = Math.Round(creditCard.Balance, 2);

                    var transactionDateTime = demand.DateTime
                        .AddHours(Random.Next(0,24))
                        .AddMinutes(Random.Next(0,60))
                        .AddSeconds(Random.Next(0,60));

                    var transaction = new Transaction
                                          {
                                              Demand = demand,
                                              CreditCard = creditCard,
                                              Poi = selectedPoi,
                                              Value = transactionValue,
                                              DateTime = transactionDateTime
                                          };

                    ctx.Transactions.AddObject(transaction);
                    demand.Fulfilled = true;

                    ++progress.Current;
                    progress.InvokeUpdate(totalTransactions, GeneratingTransactionsMsg);
                }
                progress.InvokeUpdate(totalTransactions, GeneratingTransactionsMsg + PersistingMsg);
                ctx.SaveChanges();
            }
        }

        private static Poi SelectPoi(IEnumerable<Poi> matchingPois, MapPoint personMapPoint, string selectionMethodName)
        {
            Poi selectedPoi = null;

            if (selectionMethodName == SymulationType.ShortestDistance)
            {
                var personLocation = GeoUtilities.LocationFromMapPoint(personMapPoint);

                var shortestDistance = double.MaxValue;
                foreach (var matchingPoi in matchingPois)
                {
                    var poiLocation = GeoUtilities.LocationFromMapPoint(matchingPoi.MapPoint);
                    var distance = GeoUtilities.DirectDistance(personLocation, poiLocation);

                    if (distance >= shortestDistance) continue;

                    shortestDistance = distance;
                    selectedPoi = matchingPoi;
                }
            }
            else if (selectionMethodName == SymulationType.Random)
                selectedPoi = matchingPois.ElementAt(Random.Next(0, matchingPois.Count()));

            return selectedPoi;
        }

        private void ExportBtnClick(object sender, RoutedEventArgs e)
        {
            var exportMethod = exportTypeComboBox.Text;

            var selectedTables = tableListComboBox.SelectedItems.Cast<string>();

            _progressDialog = new ProgressWindow();

            var worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            worker.DoWork +=
                delegate(object s, DoWorkEventArgs args)
                {
                    _progressDialog.Args = args;
                    ExportTables(selectedTables, exportMethod, ";", _progressDialog);

                };
            worker.RunWorkerCompleted += delegate { _progressDialog.Close(); };
            worker.RunWorkerAsync();

            _progressDialog.Worker = worker;
            _progressDialog.ShowDialog();
        }

        public class DirectoryBrowser
        {
            private readonly Thread _thread;
            private readonly FolderBrowserDialog _dialog;

            public DirectoryBrowser()
            {
                _thread = new Thread(ShowBrowser);
                _thread.SetApartmentState(ApartmentState.STA);

                _dialog = new FolderBrowserDialog
                {
                    Description = @"Select output directory.",
                    RootFolder = Environment.SpecialFolder.MyComputer,
                    ShowNewFolderButton = true,
                };
            }

            public string SelectedPath { 
                get { return _dialog.SelectedPath; } 
                set { _dialog.SelectedPath = value; } 
            }
            public DialogResult DialogResult { get; set; }

            public void Show()
            {
                _thread.Start(this);
                _thread.Join();
            }

            private void ShowBrowser(object o)
            {
                DialogResult = _dialog.ShowDialog();
            }
        }

        public static void ExportTables(IEnumerable<string> selectedTables, string exportMethod, string separator, ProgressWindow progress)
        {
            var t = new DirectoryBrowser();
            if (!string.IsNullOrEmpty(_selectedPath))
                t.SelectedPath = _selectedPath;
            t.Show();
            
            if (t.DialogResult != DialogResult.OK) return;

            _selectedPath = t.SelectedPath;

            Logger.LogInfo("Exporting data to directory: {0}", _selectedPath);

            var dbPath = DBUtilities.DbPath;

            foreach (var selectedTable in selectedTables)
            {
                if (progress.Worker.CancellationPending)
                {
                    progress.Args.Cancel = true;
                    return;
                }

                var startInfo = new ProcessStartInfo
                                    {
                                        FileName = "SqlCeCmd40.exe",
                                        WorkingDirectory = Directory.GetCurrentDirectory() + "\\tools",
                                        CreateNoWindow = true,
                                        UseShellExecute = true,
                                        WindowStyle = ProcessWindowStyle.Hidden,
                                    };

                if (exportMethod == "CSV") //export CSV
                {
                    var outputFile = string.Format("{0}\\{1}.csv", _selectedPath, selectedTable);

                    startInfo.Arguments = string.Format(
                        "-d \"Data Source={0}\" -q \"SELECT * FROM {1}\" -o \"{2}\" -h {3} -s \"{4}\" -W",
                        dbPath, selectedTable, outputFile, Int32.MaxValue, separator);

                }
                else //export XML
                {
                    var outputFile = string.Format("{0}\\{1}.xml", _selectedPath, selectedTable);

                    startInfo.Arguments = string.Format(
                        "-d \"Data Source={0}\" -q \"SELECT * FROM {1}\" -o \"{2}\" -x",
                        dbPath, selectedTable, outputFile);
                }
                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                ++progress.Current;
                progress.InvokeUpdate(selectedTables.Count(), string.Format("Exporting table '{0}'", selectedTable));
            }
        }

        private void AllowNumbersOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !CommonUtilities.IsNumberAllowed(e.Text);
        }

        private void AllowDoubleNumbers(object sender, TextCompositionEventArgs e)
        {
            var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            e.Handled = !e.Text.All(cc => Char.IsNumber(cc) || cc.ToString() == decimalSeparator);
            base.OnPreviewTextInput(e);
        }

        private void AdjustPoiFrequenciesBtnClicked(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(PoiFrequencyPageInstance);
        }
    }
}