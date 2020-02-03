using System;
using System.Drawing;
using Vkm.Api.Options;

namespace Vkm.Library.Calendar
{
  [Serializable]
  public class CalendarElementOptions : IOptions
  {
    private Color _upcomingMeetingColor;
    private TimeSpan _upcomingMeetingNotificationPeriod;
    private TimeSpan _expiredMeetingTolerance;

    public Color UpcomingMeetingColor
    {
      get => _upcomingMeetingColor;
      set => _upcomingMeetingColor = value;
    }

    public TimeSpan UpcomingMeetingNotificationPeriod
    {
      get => _upcomingMeetingNotificationPeriod;
      set => _upcomingMeetingNotificationPeriod = value;
    }

    public TimeSpan ExpiredMeetingTolerance
    {
      get => _expiredMeetingTolerance;
      set => _expiredMeetingTolerance = value;
    }
  }
}