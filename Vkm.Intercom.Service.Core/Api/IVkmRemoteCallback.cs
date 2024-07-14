using Vkm.Api.Basic;
using Vkm.Api.Drawable;
using Vkm.Api.Identification;

namespace Vkm.Intercom.Service.Api
{
  public interface IVkmRemoteCallback : IRemoteService
  {
    [OneWay]
    void ButtonPressed(Identifier layoutId, Location location, ButtonEvent buttonEvent);
  }
}