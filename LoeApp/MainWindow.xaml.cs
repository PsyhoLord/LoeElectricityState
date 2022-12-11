﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace LoeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TimeGroupsService timeGroupsService = new TimeGroupsService();

        public MainWindow()
        {
            InitializeComponent();

            var timeGroup = timeGroupsService.GetTimeGroup(DateTimeOffset.Now);
            var timeBorders = timeGroupsService.GetTimeBordersForGroup(timeGroup);
            var timeNextBorders = timeGroupsService.GetTimeBordersForNextGroup(timeGroup);

            var currentElectricityState = timeGroupsService.GetCurrentElectricityStatus(3);

            var nextElectricityState = timeGroupsService.GetNextElectricityStateEnum(currentElectricityState);

            ApplyStateHeaderStyle(currentElectricityState, timeBorders);
            ApplyNextStateHeaderStyle(nextElectricityState, timeNextBorders);
        }

        private class StateStyle
        {
            public SolidColorBrush StateBackground { get; set; }
            public SolidColorBrush StateForeground { get; set; }
            public string StateText { get; set; }
        }

        private Dictionary<ElectricityStateEnum, StateStyle> StateStyleList =
            new()
            {
                {
                    ElectricityStateEnum.No,
                    new StateStyle
                    {
                        StateBackground = new SolidColorBrush(Color.FromRgb(127, 132, 135)),
                        StateForeground = new SolidColorBrush(Color.FromRgb(250, 248, 241)),
                        StateText = "NO"
                    }
                },
                {
                    ElectricityStateEnum.Yes,
                    new StateStyle
                    {
                        StateBackground = new SolidColorBrush(Color.FromRgb(229, 237, 183)), // rgb(216, 233, 168)
                        StateForeground = new SolidColorBrush(Color.FromRgb(78, 159, 61)),
                        StateText = "YES"
                    }
                },
                {
                    ElectricityStateEnum.Maybe,
                    new StateStyle
                    {
                        StateBackground = new SolidColorBrush(Color.FromRgb(250, 240, 175)),
                        StateForeground = new SolidColorBrush(Color.FromRgb(198, 151, 73)),
                        StateText = "MAYBE"
                    }
                }
            };

        private string FormatTimeSpan(TimeSpan time)
        {
            return $"{time.Hours:00}:{time.Minutes:00}";
        }

        private void ApplyStateHeaderStyle(ElectricityStateEnum currentElectricityState, TimeBorders timeBorders)
        {
            var currentChosenStyle = StateStyleList[currentElectricityState];
            StateBackground.Background = currentChosenStyle.StateBackground;
            StateLabel.Foreground = currentChosenStyle.StateForeground;
            StateLabel.Content = currentChosenStyle.StateText;
            StateStartLabel.Foreground = currentChosenStyle.StateForeground;
            StateEndLabel.Foreground = currentChosenStyle.StateForeground;
            StateStartLabel.Content = FormatTimeSpan(timeBorders.BorderStart);
            StateEndLabel.Content = FormatTimeSpan(timeBorders.BorderEnd);
        }

        private void ApplyNextStateHeaderStyle(ElectricityStateEnum currentElectricityState, TimeBorders timeBorders)
        {
            var currentChosenStyle = StateStyleList[currentElectricityState];
            NextStateBackground.Background = currentChosenStyle.StateBackground;
            NextStateLabel.Foreground = currentChosenStyle.StateForeground;
            NextStateLabel.Content = currentChosenStyle.StateText;
            NextStateStartLabel.Foreground = currentChosenStyle.StateForeground;
            NextEndStateLabel.Foreground = currentChosenStyle.StateForeground;
            NextStateStartLabel.Content = FormatTimeSpan(timeBorders.BorderStart);
            NextEndStateLabel.Content = FormatTimeSpan(timeBorders.BorderEnd);
        }
    }
}
