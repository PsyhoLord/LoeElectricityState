using System;
using System.Collections.Generic;

namespace LoeApp;

public class TimeGroupsService
{
    private List<ElectricityStateEnum> StatesOffsets =
        new()
        {
            ElectricityStateEnum.No,
            ElectricityStateEnum.Yes,
            ElectricityStateEnum.Maybe
        };
    private List<int> GroupOffsets = new() { 0, 1, 2 };

    private List<TimeBorders> TimeBordersList = new()
    {
        new()
        {
            BorderStart = new TimeSpan(1, 0, 0),
            BorderEnd = new TimeSpan(5, 0, 0)
        },
        new()
        {
            BorderStart = new TimeSpan(5, 0, 0),
            BorderEnd = new TimeSpan(9, 0, 0)
        },
        new()
        {
            BorderStart = new TimeSpan(9, 0, 0),
            BorderEnd = new TimeSpan(13, 0, 0)
        },
        new()
        {
            BorderStart = new TimeSpan(13, 0, 0),
            BorderEnd = new TimeSpan(17, 0, 0)
        },
        new()
        {
            BorderStart = new TimeSpan(17, 0, 0),
            BorderEnd = new TimeSpan(21, 0, 0)
        },
        new()
        {
            BorderStart = new TimeSpan(21, 0, 0),
            BorderEnd = new TimeSpan(25, 0, 0)
        }
    };

    public TimeBorders GetTimeBordersForGroup(int timeGroup)
    {
        return TimeBordersList[timeGroup];
    }

    public TimeBorders GetTimeBordersForNextGroup(int timeGroup)
    {
        timeGroup++;
        if (timeGroup > 5)
            timeGroup -= 5;

        return TimeBordersList[timeGroup];
    }

    public int GetTimeGroup(DateTimeOffset offset)
    {
        var hours = offset.Hour;

        if (hours < 1) return 5;

        for (var i = 0; i < TimeBordersList.Count; i++)
        {
            if (hours >= TimeBordersList[i].BorderStart.TotalHours
                && hours <= TimeBordersList[i].BorderEnd.TotalHours)
                return i;
        }

        throw new NotImplementedException();
    }

    private int GetDayOfWeekNumber(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {

            case DayOfWeek.Monday:
                return 0;
            case DayOfWeek.Tuesday:
                return 1;
            case DayOfWeek.Wednesday:
                return 2;
            case DayOfWeek.Thursday:
                return 3;
            case DayOfWeek.Friday:
                return 4;
            case DayOfWeek.Saturday:
                return 5;
            case DayOfWeek.Sunday:
                return 6;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public ElectricityStateEnum GetCurrentElectricityStatus(int group)
    {
        var currentTime = DateTimeOffset.Now;

        var offSet = GetTimeGroupOffset(0, currentTime);
        offSet = GetDayOfWeekOffset(offSet, currentTime.DayOfWeek);
        offSet = GetGroupOffset(offSet, GroupOffsets[group - 1]);


        return (ElectricityStateEnum)offSet;
    }

    public ElectricityStateEnum GetNextElectricityStateEnum(ElectricityStateEnum currentElectricityState)
    {
        var newState = (int)currentElectricityState + 1;

        if (newState > 2)
            newState -= 3;

        return (ElectricityStateEnum)newState;
    }

    private int GetTimeGroupOffset(int prevOffset, DateTimeOffset currentTime)
    {
        var timeGroup = GetTimeGroup(currentTime);

        var offset = timeGroup;

        if (offset > 2)
        {
            var x = offset / 3;
            offset -= 3 * x;
        }

        return offset;
    }

    private int GetDayOfWeekOffset(int prevOffset, DayOfWeek dayOfWeek)
    {
        var dayOfWeekUkraine = GetDayOfWeekNumber(dayOfWeek);

        var offset = (int)prevOffset + dayOfWeekUkraine;

        if (offset > 2)
        {
            var x = offset / 3;
            offset -= 3 * x;
        }

        return offset;
    }

    private int GetGroupOffset(int prevOffset, int group)
    {
        var offset = prevOffset + group;

        if (offset > 2)
        {
            var x = offset / 3;
            offset -= 3 * x;
        }

        return offset;
    }
}



public enum ElectricityStateEnum
{
    No,
    Yes,
    Maybe
}


public class TimeBorders
{
    public TimeSpan BorderStart { get; set; }
    public TimeSpan BorderEnd { get; set; }
}