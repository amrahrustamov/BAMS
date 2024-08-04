using BAMS.Databse.Models.Enums;

namespace BAMS.Databse.Models
{
    public class Taxpayer
    {
        public int TaxpayerID { get; set; }
        public string MunicipalName { get; set; }
        public string Municipal_code { get; set; }
        public string VOEN { get; set; }
        public string YVOK { get; set; }
        public string VergiOdeyiciVoen { get; set; }
        public string Individual_Legal { get; set; }
        public string Concession { get; set; }
        public string ConcessionGiveOrgan { get; set; }
        public string ConcessionCause { get; set; }
        public string ConcessionSeries { get; set; }
        public string ConcesionNumber{ get; set; }
        public DateTime? ConcessionGiveDate { get; set; }
    }
}
