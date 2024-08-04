using BAMS.Databse.Models;
using BAMS.Services.Interface;
using System.Text.RegularExpressions;

namespace BAMS.Services.Implementation
{
    public class Check : ICheck
    {
        public bool PinCode(string pincode)
        {
            string pattern = @"^[A-Za-z0-9]{7}$";

            bool isMatch = Regex.IsMatch(pincode, pattern);

            if (isMatch) return true;

            return false;
        }
        public bool TaxPayerIsNull(Taxpayer taxpayer)
        {
            if (taxpayer == null) return true;
            return false;
        }
    }
}
