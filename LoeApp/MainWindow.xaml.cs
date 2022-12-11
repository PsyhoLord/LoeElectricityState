using System;
using System.Windows;

namespace LoeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TimeGroupsService _timeGroupsService = new TimeGroupsService();

        public MainWindow()
        {
            InitializeComponent();

            CurrentStateControl.Init(true);
            
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
