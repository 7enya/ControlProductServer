using System.Configuration;
using System.Collections.Specialized;
using ServerWinForm.Data;
using ServerWinForm.Enums;
using System.Collections.Immutable;
using System.Diagnostics;

namespace ServerWinForm.Services
{
    public class ConfigService
    {
        public List<ClientProfile> profiles { get; private set; }
        public string? ServerAddress { get; private set; }
        
        public ConfigService() {
            profiles = new List<ClientProfile>();
            NameValueCollection configKeys = ConfigurationManager.AppSettings;
            if (configKeys.Count != 0)
            {
                string? deviceName = null, macAddress = null, deviceType = null, login = null, password = null;
                const int COUNT_DEVICE_PARAMS = 5;
                int counter = 0;
                foreach (var key in configKeys.AllKeys)
                {
                    if (string.IsNullOrEmpty(key)) { continue; } 
                    if (key.Contains("ServerAddress", StringComparison.OrdinalIgnoreCase))
                    { ServerAddress = configKeys[key]; continue; }
                    ++counter;
                    if (key.Contains("DeviceName", StringComparison.OrdinalIgnoreCase))
                    { deviceName = configKeys[key]; }
                    else if (key.Contains("MacAddress", StringComparison.OrdinalIgnoreCase))
                    { macAddress = configKeys[key]; }
                    else if (key.Contains("DeviceType", StringComparison.OrdinalIgnoreCase))
                    { deviceType = configKeys[key]; }
                    else if (key.Contains("Login", StringComparison.OrdinalIgnoreCase))
                    { login = configKeys[key]; }
                    else if (key.Contains("Password", StringComparison.OrdinalIgnoreCase))
                    { password = configKeys[key]; }
                    if (counter == COUNT_DEVICE_PARAMS)
                    {
                        counter = 0;
                        if (string.IsNullOrEmpty(deviceName?.Trim())) continue;
                        profiles.Add(
                            new ClientProfile(
                                deviceName,
                                macAddress,
                                DeviceType.Find(deviceType),
                                login,
                                password
                            )
                        );
                    }
                }
            }
            //foreach (var profile in profiles)
            //{
            //    Debug.WriteLine($"deviceName = {profile.deviceName}");
            //    Debug.WriteLine($"macAddress = {profile.deviceMacAddress}");
            //    Debug.WriteLine($"deviceType = {profile.deviceType}");
            //    Debug.WriteLine($"login = {profile.login}");
            //    Debug.WriteLine($"password = {profile.password}");
            //}
        }

        private static ImmutableHashSet<string> GetDeviceTypeList(NameValueCollection configKeys)
        {
            if (configKeys.Count != 0)
                return ImmutableHashSet<string>.Empty;
            ImmutableHashSet<string> set = ImmutableHashSet<string>.Empty;
            foreach (var key in configKeys.AllKeys)
            {
                if (key!.Contains("DeviceType", StringComparison.OrdinalIgnoreCase))
                { set.Add(configKeys[key]!); }
            }
            return set;
        }
    }
}
