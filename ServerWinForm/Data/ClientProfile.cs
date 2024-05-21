using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerWinForm.Enums;

namespace ServerWinForm.Data
{
    public class ClientProfile
    {
        public string deviceName { get; set; }
        public string? deviceMacAddress { get; set; }
        public DeviceType deviceType { get; set; }
        public string? login { get; set; }
        public string? password { get; set; }
       
        public ClientProfile (string name, string? macAddress, DeviceType type, string? login, string? password)
        {
            deviceName = name;
            deviceMacAddress = macAddress;
            deviceType = type;
            this.login = login;
            this.password = password;
        }
    }
}