using System;
using System.Timers;
using System.Windows;

namespace LoeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TimeGroupsService _timeGroupsService = new TimeGroupsService();
        private static Timer RefreshTimer;

        public MainWindow()
        {
            InitializeComponent();

            CurrentStateControl.Init(true);

            SetTimer();
        }

        private void SetTimer()
        {
            OnTimedEvent();
            // Create a timer with a two second interval.
            RefreshTimer = new Timer(TimeSpan.FromMinutes(1));
            // Hook up the Elapsed event for the timer. 
            RefreshTimer.Elapsed += OnTimedEvent;
            RefreshTimer.AutoReset = true;
            RefreshTimer.Enabled = true;
        }

        private void OnTimedEvent(object source = null, ElapsedEventArgs? e = null)
        {
            Refresh();
        }

        private void Refresh()
        {
            var timeGroup = _timeGroupsService.GetTimeGroup(DateTimeOffset.Now);
            var timeBorders = _timeGroupsService.GetTimeBordersForGroup(timeGroup);
            var timeNextBorders = _timeGroupsService.GetTimeBordersForNextGroup(timeGroup);

            var currentElectricityState = _timeGroupsService.GetCurrentElectricityStatus(3);

            var nextElectricityState = _timeGroupsService.GetNextElectricityStateEnum(currentElectricityState);

            CurrentStateControl.Refresh(currentElectricityState, timeBorders);
            NextStateControl.Refresh(nextElectricityState, timeNextBorders);
        }
    }
}
