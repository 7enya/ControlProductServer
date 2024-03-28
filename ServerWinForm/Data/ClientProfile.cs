﻿using System;
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
        public string? deviceName { get; set; }
        public string? deviceMacAddress { get; set; }
        public DeviceType deviceType { get; set; }
        public string? login { get; set; }
        public string? password { get; set; }
        
        public ClientProfile (string? name, string? macAddress, DeviceType type, string? login, string? password)
        {
            deviceName = name;
            deviceMacAddress = macAddress;
            deviceType = type;
            this.login = login;
            this.password = password;
        }

        public override bool Equals(object? obj)
        {
            return obj is ClientProfile profile &&
                   deviceName == profile.deviceName &&
                   deviceMacAddress == profile.deviceMacAddress &&
                   EqualityComparer<DeviceType>.Default.Equals(deviceType, profile.deviceType) &&
                   login == profile.login &&
                   password == profile.password;
        }

        //public string? GetDeviceName()
        //{
        //    return deviceName;
        //}

        //public DeviceType? GetDeviceType()
        //{
        //    return deviceType;
        //}

        //public string? GetDeviceMacAddress()
        //{
        //    return deviceMacAddress;
        //}

        //public string? GetLogin()
        //{
        //    return login;
        //}

        //public string? GetPassword()
        //{
        //    return password;
        //}
    }
}