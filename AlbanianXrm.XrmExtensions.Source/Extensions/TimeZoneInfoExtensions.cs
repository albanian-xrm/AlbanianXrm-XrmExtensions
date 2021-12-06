using System;

namespace AlbanianXrm.DateTimes.Extensions.Extensions
{
    public static class TimeZoneInfoExtensions
    {
        public static DateTime ConvertToUTC(this TimeZoneInfo timeZoneInfo, DateTime localDateTime)
        {
            var adjustment = timeZoneInfo.GetAdjustmentRule(localDateTime);
            if (adjustment == null)
            {
                return DateTime.SpecifyKind(localDateTime.Add(-timeZoneInfo.BaseUtcOffset), DateTimeKind.Utc);
            }
            var timeDifference = timeZoneInfo.BaseUtcOffset;
            if (timeZoneInfo.SupportsDaylightSavingTime && timeZoneInfo.IsDaylightSavingTime(localDateTime))
            {
                timeDifference = timeDifference.Add(adjustment.DaylightDelta);
            }
            return DateTime.SpecifyKind(localDateTime.Add(-timeDifference), DateTimeKind.Utc);
        }

        public static DateTime ConvertToLocal(this TimeZoneInfo timeZoneInfo, DateTime utcDateTime)
        {
            var adjustment = timeZoneInfo.GetAdjustmentRule(utcDateTime);
            if (adjustment == null)
            {
                return DateTime.SpecifyKind(utcDateTime.Add(timeZoneInfo.BaseUtcOffset), DateTimeKind.Unspecified);
            }
            var timeDifference = timeZoneInfo.BaseUtcOffset;
            if (timeZoneInfo.SupportsDaylightSavingTime && timeZoneInfo.IsDaylightSavingTime(utcDateTime))
            {
                timeDifference = timeDifference.Add(adjustment.DaylightDelta);
            }
            return DateTime.SpecifyKind(utcDateTime.Add(timeDifference), DateTimeKind.Unspecified);
        }

        public static DateTime GetDaylightTransitionAsDate(this TimeZoneInfo.TransitionTime transition, DateTime date)
        {
            if (transition.IsFixedDateRule)
            {
                return new DateTime(date.Year, transition.Month, transition.Day);
            }
            else
            {
                var transitionMonth1st = new DateTime(date.Year, transition.Month, 1);

                var daysToDayOfWeek = transition.DayOfWeek - transitionMonth1st.DayOfWeek;
                var weeks = daysToDayOfWeek >= 0 ? transition.Week - 1 : transition.Week;
                var transitionMonthDay = transitionMonth1st.AddDays(daysToDayOfWeek);
                if (transitionMonthDay.AddDays(7 * weeks).Month != transition.Month)
                {
                    weeks -= 1;
                }
                return transitionMonthDay.AddDays(7 * weeks);
            }
        }

        public static TimeZoneInfo.AdjustmentRule GetAdjustmentRule(this TimeZoneInfo localTimezone, DateTime date)
        {
            var adjustments = localTimezone.GetAdjustmentRules();
            // Iterate adjustment rules for time zone
            foreach (TimeZoneInfo.AdjustmentRule adjustment in adjustments)
            {
                // Determine if this adjustment rule covers year desired
                if (adjustment.DateStart <= date && adjustment.DateEnd >= date)
                    return adjustment;
            }
            return null;
        }
    }
}
