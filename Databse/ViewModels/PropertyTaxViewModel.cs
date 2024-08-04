namespace BAMS.Databse.ViewModels
{
    public class PropertyTaxViewModel
    {
        public int S_N { get; set; }
        public DateTime Verginin_Hesablanma_Tarixi { get; set; }
        public string Sənədin_Reyestr_Nömrəsi { get; set; }
        public string Qeydiyyat_Nömrəsi { get; set; }
        public DateTime Mülkiyyət_Sənədinin_Verilmə_Tarixi { get; set; }
        public string Yerləşdiyi_Ünvan { get; set; }
        public string Zona_Əmsalı { get; set; }
        public float Vergi_Dərəcəsi { get; set; }
        public float Ümumi_Sahə { get; set; }
        public float Vergiyə_Cəlb_Edilən_Sahə { get; set; }
        public float Məbləğ { get; set; }
        public string Vergi_Növü { get; set; }
    }
}
