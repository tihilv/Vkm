using System;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service.Calendar
{
  public interface ICalendarService: IService
  {
    AppointmentInfo[] GetCalendar(DateTime from, DateTime to);
    bool TryShowAppointment(AppointmentInfo appointmentInfo);
  }
}