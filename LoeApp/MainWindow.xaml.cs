using System;
using System.Threading;
using System.Timers;
using System.Windows;
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

        public MainWindow()
        {
            InitializeComponent();

            CurrentStateControl.Init(true);

            SetTimer();
        }

        private void SetTimer()
        {
            OnTimedEvent();
            
            _refreshTimer = new Timers.Timer(TimeSpan.FromMinutes(1));
            
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

                var currentElectricityState = _timeGroupsService.GetCurrentElectricityStatus(3);

                var nextElectricityState = _timeGroupsService.GetNextElectricityStateEnum(currentElectricityState);

                CurrentStateControl.Refresh(currentElectricityState, timeBorders);
                NextStateControl.Refresh(nextElectricityState, timeNextBorders);
            }));
        }
    }
}
