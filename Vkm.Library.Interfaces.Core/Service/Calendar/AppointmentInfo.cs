using System;

namespace Vkm.Library.Interfaces.Service.Calendar
{
  public class AppointmentInfo : IEquatable<AppointmentInfo>
  {
    public DateTime FromDateTime { get; private set; }
    public DateTime ToDateTime { get; private set; }
    public string Location { get; private set; }
    public string Organiser { get; private set; }
    public string Subject { get; private set; }

    public AppointmentInfo(DateTime fromDateTime, DateTime toDateTime, string location, string organiser, string subject)
    {
      FromDateTime = fromDateTime;
      ToDateTime = toDateTime;
      Location = location;
      Organiser = organiser;
      Subject = subject;
    }

    public bool Equals(AppointmentInfo other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return FromDateTime.Equals(other.FromDateTime) && ToDateTime.Equals(other.ToDateTime) && string.Equals(Location, other.Location, StringComparison.OrdinalIgnoreCase) && string.Equals(Organiser, other.Organiser, StringComparison.OrdinalIgnoreCase) && string.Equals(Subject, other.Subject, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((AppointmentInfo) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = FromDateTime.GetHashCode();
        hashCode = (hashCode * 397) ^ ToDateTime.GetHashCode();
        hashCode = (hashCode * 397) ^ (Location != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Location) : 0);
        hashCode = (hashCode * 397) ^ (Organiser != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Organiser) : 0);
        hashCode = (hashCode * 397) ^ (Subject != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Subject) : 0);
        return hashCode;
      }
    }

    public static bool operator ==(AppointmentInfo left, AppointmentInfo right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(AppointmentInfo left, AppointmentInfo right)
    {
      return !Equals(left, right);
    }
  }
}