using System;
using System.Collections.Generic;
using LoeApp.Helpers;

namespace LoeApp.Services;

public class TimeGroupsService
{
    private HtmlParseService _htmlParseService = new HtmlParseService();
    
    private readonly List<int> _groupOffsets = new() { 0, 1, 2 };

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

    public List<DataRow> GetLoeStatus()
    {
        return _htmlParseService.GetStatesPerStreet();
    }

    public TimeBorders GetTimeBordersForGroup(int timeGroup)
    {
        return TimeBordersList[timeGroup];
    }

    public TimeBorders GetTimeBordersForNextGroup(int timeGroup)
    {
        return TimeBordersList[ClearOffsetFromPeriodic(++timeGroup, 5)];
    }

    public int GetTimeGroup(DateTimeOffset offset)
    {
        var hours = offset.Hour;

        if (hours < 1) return 5;

        for (var i = 0; i < TimeBordersList.Count; i++)
        {
            if (hours >= TimeBordersList[i].BorderStart.TotalHours
                && hours <= TimeBordersList[i].BorderEnd.TotalHours)
            {
                if (hours == TimeBordersList[i].BorderEnd.TotalHours && offset.Minute != 0)
                    continue;
                return i;
            }
        }

        throw new NotImplementedException();
    }

    public ElectricityStateEnum GetCurrentElectricityStatus(int group)
    {
        var currentTime = DateTimeOffset.Now;

        var offSet = GetTimeGroupOffset(0, currentTime);
        offSet = GetDayOfWeekOffset(offSet, currentTime.DayOfWeek);
        offSet = GetGroupOffset(offSet, _groupOffsets[group]);

        return (ElectricityStateEnum)offSet;
    }

    public ElectricityStateEnum GetNextElectricityStateEnum(ElectricityStateEnum currentElectricityState)
    {
        var newState = (int)currentElectricityState + 1;

        newState = ClearOffsetFromPeriodic(newState, 3);

        return (ElectricityStateEnum)newState;
    }

    private int GetTimeGroupOffset(int prevOffset, DateTimeOffset currentTime)
    {
        var timeGroup = GetTimeGroup(currentTime);

        var offset = timeGroup;

        return ClearOffsetFromPeriodic(offset, 3);
    }

    private int GetDayOfWeekOffset(int prevOffset, DayOfWeek dayOfWeek)
    {
        var dayOfWeekUkraine = MainHelper.GetDayOfWeekNumber(dayOfWeek);

        var offset = prevOffset + dayOfWeekUkraine;

        return ClearOffsetFromPeriodic(offset, 3);
    }

    private int GetGroupOffset(int prevOffset, int group)
    {
        var offset = prevOffset + group;
        return ClearOffsetFromPeriodic(offset, 3);
    }

    private int ClearOffsetFromPeriodic(int offset, int period)
    {
        if (offset < period)
            return offset;

        var x = offset / period;
        offset -= period * x;

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