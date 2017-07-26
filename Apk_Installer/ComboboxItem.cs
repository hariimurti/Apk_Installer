using AndroidCtrl;

namespace Apk_Installer
{
    class ComboboxItem
    {
        public string Id { get; set; }
        public string Device { get; set; }
        public string Model { get; set; }
        public DataModelDevicesItem DataModel { get; set; }

        public override string ToString()
        {
            if (this.Model != string.Empty)
            {
                if (this.Device != string.Empty)
                    return $"{this.Model} ( {this.Device} )";
                else
                    return this.Model;
            }
            else if (this.Device != string.Empty)
            {
                return this.Device;
            }
            else
            {
                return this.Id;
            }
        }
    }
}
