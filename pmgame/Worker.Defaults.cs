using System;
using System.Collections.Generic;

namespace pmgame
{
    public partial class Worker
    {
        public static class Defaults
        {
            public static DateTime clockIn = new DateTime(1, 1, 1, 8, 0, 0);
            public static DateTime clockOut = new DateTime(1, 1, 1, 17, 0, 0);
            
            public static TimeSpan contextSwitchPenalty = TimeSpan.FromMinutes(1);
            
            public static HashSet<DayOfWeek> workDays = new HashSet<DayOfWeek>(7) {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
            };  
        }
    }

}
