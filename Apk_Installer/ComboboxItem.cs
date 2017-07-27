using AndroidCtrl;

namespace Apk_Installer
{
    class ComboboxItem
    {
        public string Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string CodeName { get; set; }
        public DataModelDevicesItem DataModel { get; set; }

        public override string ToString()
        {
            if (this.Brand != string.Empty)
            {
                if (this.Model != string.Empty)
                    return $"{this.Brand} {this.Model}";
                else if (this.CodeName != string.Empty)
                    return $"{this.Brand} {this.CodeName}";
                else
                    return this.Brand;
            }
            else if(this.Model != string.Empty)
            {
                if ((this.CodeName != string.Empty) && (this.Model != this.CodeName))
                    return $"{this.Model} ( {this.CodeName} )";
                else
                    return this.Model;
            }
            else
            {
                return this.Id;
            }
        }
    }
}
