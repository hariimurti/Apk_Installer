using AndroidCtrl;

namespace Apk_Installer
{
    class ComboboxItem
    {
        public string Id { get; set; }
        public string Device { get; set; }
        public DataModelDevicesItem DataModel { get; set; }

        public override string ToString()
        {
            if (Device.Trim() != string.Empty)
            {
                return $"{Device} ( {Id} )";
            }
            else
            {
                return Id;
            }
        }
    }
}
