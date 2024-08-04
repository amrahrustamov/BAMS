using BAMS.Databse.Models;

namespace BAMS.Databse.ViewModels
{
    public class TaxPayerByPropertyTax : TaxPayerViewModel
    {
        public List<PropertyTaxViewModel> PropertyTaxes { get; set; }
    }
}
