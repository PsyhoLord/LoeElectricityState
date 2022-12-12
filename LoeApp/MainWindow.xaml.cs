using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LoeApp.Services;
using Timers = System.Timers;

namespace LoeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TimeGroupsService _timeGroupsService = new();
        private Timers.Timer _refreshTimer;
        private Timers.Timer _refreshStreetTimer;

        private int currentElectricityGroup;
        private int _currentStreet = -1;
        private List<DataRow> _streets;

        public MainWindow()
        {
            InitializeComponent();

            currentElectricityGroup = 3 - 1;
            CurrentStateControl.Init(true);
            NextStateControl.Init();

            _streets = _timeGroupsService.GetLoeStatus();

            InitElectricityGroupList();
            InitStreetGroupList();

            //SettlementControl.SetTitle("Settlement:");
            //StreetControl.SetTitle("Street:");
            //BuildingControl.SetTitle("Building:");
            //TurnOffTypeControl.SetTitle("Turn Off Type:");
            //ReasonControl.SetTitle("Reason:");
            //TurnOffTimeControl.SetTitle("Turn Off Time:");
            //ExpectedTurnOnTimeControl.SetTitle("Expected Turn On Time:");

            SettlementControl.SetTitle("Населений пункт:");
            StreetControl.SetTitle("Вулиця:");
            BuildingControl.SetTitle("Будинок:");
            TurnOffTypeControl.SetTitle("Тип вимкнення:");
            ReasonControl.SetTitle("Причина:");
            TurnOffTimeControl.SetTitle("Час вимкнення:");
            ExpectedTurnOnTimeControl.SetTitle("Очікуваний час ввімкнення:");

            SetTimer();
        }

        private void InitElectricityGroupList()
        {
            electricityGroupList.IsEditable = false;
            electricityGroupList.IsReadOnly = true;
            electricityGroupList.ItemsSource = new List<int> { 1, 2, 3 };
            electricityGroupList.Text = $"{3}";
            electricityGroupList.SelectionChanged += (sender, args) =>
            {
                var comboBox = sender as ComboBox;
                currentElectricityGroup = comboBox.SelectedIndex;
                Refresh();
            };
        }

        private void InitStreetGroupList()
        {
            electricityStreetList.DisplayMemberPath = "Street";
            electricityStreetList.IsEditable = true;
            electricityStreetList.IsReadOnly = false;
            electricityStreetList.IsTextSearchEnabled = true;
            electricityStreetList.ItemsSource = _streets;
            electricityStreetList.Text = $"Оберіть вулицю";
            electricityStreetList.SelectionChanged += (sender, args) =>
            {
                var comboBox = sender as ComboBox;
                _currentStreet = comboBox.SelectedIndex;
                
                if(_currentStreet != -1)
                    UpdateCurrentStreetStatus(_streets[_currentStreet]);
            };
        }

        private void SetTimer()
        {
            OnTimedEvent();
            
            _refreshTimer = new Timers.Timer(TimeSpan.FromSeconds(5));
            
            _refreshTimer.Elapsed += OnTimedEvent;
            _refreshTimer.AutoReset = true;
            _refreshTimer.Enabled = true;

            _refreshStreetTimer = new Timers.Timer(TimeSpan.FromMinutes(5));

            _refreshStreetTimer.Elapsed += RefreshStreet;
            _refreshStreetTimer.AutoReset = true;
            _refreshStreetTimer.Enabled = true;
        }

        private void RefreshStreet(object? sender = null, ElapsedEventArgs e = null)
        {
            if (_currentStreet != -1)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate {
                    _streets = _timeGroupsService.GetLoeStatus();
                    UpdateCurrentStreetStatus(_streets[_currentStreet]);
                }));
            }
        }

        private void OnTimedEvent(object source = null, ElapsedEventArgs? e = null)
        {
            Refresh();
        }

        private void Refresh()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate {
                CurrentTimeLabel.Content = $"{DateTime.Now:HH:mm}";

                var timeGroup = _timeGroupsService.GetTimeGroup(DateTimeOffset.Now);
                var timeBorders = _timeGroupsService.GetTimeBordersForGroup(timeGroup);
                var timeNextBorders = _timeGroupsService.GetTimeBordersForNextGroup(timeGroup);

                var currentElectricityState = _timeGroupsService.GetCurrentElectricityStatus(currentElectricityGroup);

                var nextElectricityState = _timeGroupsService.GetNextElectricityStateEnum(currentElectricityState);

                CurrentStateControl.Refresh(currentElectricityState, timeBorders);
                NextStateControl.Refresh(nextElectricityState, timeNextBorders);
            }));
        }

        private void UpdateCurrentStreetStatus(DataRow dataRow)
        {
            SettlementControl.SetDescription(dataRow.Settlement);
            StreetControl.SetDescription(dataRow.Street);
            BuildingControl.SetDescription(dataRow.Building);
            TurnOffTypeControl.SetDescription(dataRow.TurnOffType);
            ReasonControl.SetDescription(dataRow.Reason);
            TurnOffTimeControl.SetDescription(dataRow.TurnOffTime);
            ExpectedTurnOnTimeControl.SetDescription(dataRow.ExpectedTurnOnTime);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var destinationUrl =
                "https://poweroff.loe.lviv.ua/search_off?csrfmiddlewaretoken=S3nXwVXUie4HvyLehiZ4odjh2i6fnGWx0sKuX3TPRl0gJo2K9NbBzi70rvNh1smQ&city=%D0%9B%D1%8C%D0%B2%D1%96%D0%B2&street=&otg=&q=%D0%9F%D0%BE%D1%88%D1%83%D0%BA";
            var sInfo = new System.Diagnostics.ProcessStartInfo(destinationUrl)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }

        private void ButtonRefreshStreet_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshStreet();
        }
    }
}
