using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LoeApp.Helpers;
using LoeApp.Services;

namespace LoeApp.Controls
{
    /// <summary>
    /// Interaction logic for ElectricityStateControl.xaml
    /// </summary>
    public partial class ElectricityStateControl : UserControl
    {
        private Dictionary<ElectricityStateEnum, StateStyle> StateStyleList =
            new()
            {
                {
                    ElectricityStateEnum.No,
                    new StateStyle
                    {
                        StateBackground = MainHelper.CreateColorBrushRgb(127, 132, 135),
                        StateForeground = MainHelper.CreateColorBrushRgb(250, 248, 241),
                        StateProgressForeground = MainHelper.CreateColorBrushRgb(65, 63, 66),
                        StateText = "Немає енергія"
                    }
                },
                {
                    ElectricityStateEnum.Yes,
                    new StateStyle
                    {
                        StateBackground = MainHelper.CreateColorBrushRgb(229, 237, 183), // rgb(216, 233, 168)
                        StateForeground = MainHelper.CreateColorBrushRgb(30, 81, 40),
                        StateProgressForeground = MainHelper.CreateColorBrushRgb(78, 159, 61),
                        StateText = "Є енергії"
                    }
                },
                {
                    ElectricityStateEnum.Maybe,
                    new StateStyle
                    {
                        StateBackground = MainHelper.CreateColorBrushRgb(250, 240, 175),
                        StateForeground = MainHelper.CreateColorBrushRgb(115, 95, 50),
                        StateProgressForeground = MainHelper.CreateColorBrushRgb(198, 151, 73),
                        StateText = "Можливе відключення"
                    }
                }
            };

        public ElectricityStateControl()
        {
            InitializeComponent();
        }

        public void Init(bool progressBarVisible = false)
        {
            currentStateProgressBar.Visibility = progressBarVisible ? Visibility.Visible : Visibility.Hidden;
        }

        

        public void Refresh(ElectricityStateEnum currentElectricityState, TimeBorders timeBorders)
        {
            var currentChosenStyle = StateStyleList[currentElectricityState];
            StateBackground.Background = currentChosenStyle.StateBackground;
            StateLabel.Foreground = currentChosenStyle.StateForeground;
            StateLabel.Content = currentChosenStyle.StateText;
            StateStartLabel.Foreground = currentChosenStyle.StateForeground;
            StateEndLabel.Foreground = currentChosenStyle.StateForeground;
            StateStartLabel.Content = MainHelper.FormatTimeSpan(timeBorders.BorderStart);
            StateEndLabel.Content = MainHelper.FormatTimeSpan(timeBorders.BorderEnd);

            if (currentStateProgressBar.Visibility == Visibility.Visible)
            {
                ApplyStateForProgressBar(currentChosenStyle, timeBorders);
            }
        }

        private void ApplyStateForProgressBar(StateStyle currentChosenStyle, TimeBorders timeBorders)
        {
            currentStateProgressBar.Background = currentChosenStyle.StateBackground;
            currentStateProgressBar.Foreground = currentChosenStyle.StateProgressForeground;

            var dateTime = DateTimeOffset.Now;
            dateTime = dateTime - timeBorders.BorderStart;

            var totalHours = dateTime.Hour;
            var totalMinutes = dateTime.Minute;

            currentStateProgressBar.Value = totalHours * 100 + totalMinutes;
        }

        private class StateStyle
        {
            public SolidColorBrush StateBackground { get; set; }
            public SolidColorBrush StateForeground { get; set; }
            public SolidColorBrush StateProgressForeground { get; set; }
            public string StateText { get; set; }
        }
    }

    
}
