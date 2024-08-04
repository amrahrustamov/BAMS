using BAMS.Databse.Models;

namespace BAMS.Services.Interface
{
    public interface ICheck
    {
        public bool PinCode(string pincode);
        public bool TaxPayerIsNull(Taxpayer taxpayer);
    }
}
