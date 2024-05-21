
namespace ServerWinForm.Enums
{
    public class DeviceType
    {
        private DeviceType(string value) { Value = value; }
        public string Value { get; private set; }
        public static DeviceType SCANNER { get { return new DeviceType("Scanner"); } }
        public static DeviceType SMART_PHONE { get { return new DeviceType("Smartphone"); } }
        public static DeviceType UNDEFINED { get { return new DeviceType("Undefined"); } }
        public override string ToString()
        {
            return Value;
        }

        public static DeviceType Find(string? type)
        { 
            if (type == null) return UNDEFINED;
            if (type.Equals(SCANNER.Value)) { return SCANNER; }
            if (type.Equals(SMART_PHONE.Value)) { return SMART_PHONE; }
            return UNDEFINED;
        }
    }
}