using System;
using System.Runtime.InteropServices;

namespace CoreAudioApi.ExtendedConfig
{
    public class PolicyConfig
    {
        public static void SetDefaultEndpoint(string devId, ERole eRole)
        {
            object o = (object) null;
            try
            {
                o = GetPolicyConfig();
                IPolicyConfigX policyConfigX = o as IPolicyConfigX;
                IPolicyConfig policyConfig = o as IPolicyConfig;
                IPolicyConfigVista policyConfigVista = o as IPolicyConfigVista;
                if (policyConfig != null)
                    policyConfig.SetDefaultEndpoint(devId, eRole);
                else if (policyConfigVista != null)
                    policyConfigVista.SetDefaultEndpoint(devId, eRole);
                else
                    policyConfigX?.SetDefaultEndpoint(devId, eRole);
            }
            finally
            {
                if (o != null && Marshal.IsComObject(o))
                    Marshal.FinalReleaseComObject(o);
                GC.Collect();
            }
        }

        public static object GetPolicyConfig()
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")));
        }
    }
}
