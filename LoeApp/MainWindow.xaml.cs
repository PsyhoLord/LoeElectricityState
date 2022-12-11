using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Timers = System.Timers;

namespace LoeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TimeGroupsService _timeGroupsService = new TimeGroupsService();
        private Timers.Timer _refreshTimer;

        private int currentElectricityGroup;

        public MainWindow()
        {
            InitializeComponent();

            currentElectricityGroup = 3 - 1;
            CurrentStateControl.Init(true);
            NextStateControl.Init();

            electricityGroupList.IsEditable = true;
            electricityGroupList.IsReadOnly = true;
            electricityGroupList.ItemsSource = new List<int> {1,2,3};
            electricityGroupList.Text = $"{3}";
            electricityGroupList.SelectionChanged += (sender, args) =>
            {
                var comboBox = sender as ComboBox;
                currentElectricityGroup = comboBox.SelectedIndex;
                Refresh();
            };

            SetTimer();
        }

        private void SetTimer()
        {
            OnTimedEvent();
            
            _refreshTimer = new Timers.Timer(TimeSpan.FromSeconds(5));
            
            _refreshTimer.Elapsed += OnTimedEvent;
            _refreshTimer.AutoReset = true;
            _refreshTimer.Enabled = true;
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
    }
}
