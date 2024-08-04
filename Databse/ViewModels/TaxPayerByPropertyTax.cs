using BAMS.Databse.Models;

namespace BAMS.Databse.ViewModels
{
    public class TaxPayerByPropertyTax : TaxPayerViewModel
    {
        public List<PropertyTax> PropertyTaxes { get; set; }
    }
    public class TaxPayerByLandTax : Taxpayer
    {
        public List<LandTax> LandTaxes { get; set; }
    }
}
