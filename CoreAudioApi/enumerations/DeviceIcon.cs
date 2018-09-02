using System;
using System.Collections.Generic;

namespace CoreAudioApi.enumerations
{
    public enum DeviceIcon
    {
        DesktopMicrophone,
        Digital,
        Headphones,
        Headset,
        Kinect,
        LineIn,
        Phone,
        Speakers,
        StereoMix,
        Monitor,
        Unknown
    }

    static class DeviceIconHelper
    {
        private static readonly Dictionary<string, DeviceIcon> IconMap = new Dictionary<string, DeviceIcon>
        {
            {"0", DeviceIcon.Speakers},
            {"1", DeviceIcon.Speakers},
            {"2", DeviceIcon.Headphones},
            {"3", DeviceIcon.LineIn},
            {"4", DeviceIcon.Digital},
            {"5", DeviceIcon.DesktopMicrophone},
            {"6", DeviceIcon.Headset},
            {"7", DeviceIcon.Phone},
            {"8", DeviceIcon.Monitor},
            {"9", DeviceIcon.StereoMix},
            {"10", DeviceIcon.Speakers},
            {"11", DeviceIcon.Kinect},
            {"12", DeviceIcon.DesktopMicrophone},
            {"13", DeviceIcon.Speakers},
            {"14", DeviceIcon.Headphones},
            {"15", DeviceIcon.Speakers},
            {"16", DeviceIcon.Headphones},
            {"3004", DeviceIcon.Speakers},
            {"3010", DeviceIcon.Speakers},
            {"3011", DeviceIcon.Headphones},
            {"3012", DeviceIcon.LineIn},
            {"3013", DeviceIcon.Digital},
            {"3014", DeviceIcon.DesktopMicrophone},
            {"3015", DeviceIcon.Headset},
            {"3016", DeviceIcon.Phone},
            {"3017", DeviceIcon.Monitor},
            {"3018", DeviceIcon.StereoMix},
            {"3019", DeviceIcon.Speakers},
            {"3020", DeviceIcon.Kinect},
            {"3021", DeviceIcon.DesktopMicrophone},
            {"3030", DeviceIcon.Speakers},
            {"3031", DeviceIcon.Headphones},
            {"3050", DeviceIcon.Speakers},
            {"3051", DeviceIcon.Headphones}
        };

        public static DeviceIcon GetIconByPath(string path)
        {
            var imageKey = path.Substring(path.IndexOf(",", StringComparison.InvariantCultureIgnoreCase) + 1).Replace("-", "");
            
            if (IconMap.TryGetValue(imageKey, out var value))
                return value;

            return DeviceIcon.Unknown;
        }
    }
}
