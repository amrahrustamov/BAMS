using BAMS.Databse;
using BAMS.Databse.Models;
using BAMS.Databse.ViewModels;
using BAMS.Services.Implementation;
using BAMS.Services.Interface;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace BAMS.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICheck _checking;
        public List<PropertyTaxViewModel> _properties = new List<PropertyTaxViewModel>();
        public List<LandTaxViewModel> _lands = new List<LandTaxViewModel>();

        public UserController(IConfiguration configuration, ICheck check)
        {
            _configuration = configuration;
            _checking = check;
        }

        [HttpGet("get-residential-property-tax/{pincode}")]
        public async Task<IActionResult> GetResidentialPropertyTax(string pincode)
        {
            if(!_checking.PinCode(pincode)) return BadRequest(new {message = " Yanlış format! FIN kod 7 rəqəm vəya hərfdən ibarət olmalıdır."});

            string connectionString = _configuration.GetConnectionString("DefaultConnectionMSSQL");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<Taxpayer>("Select t.TaxpayerID,m.MunicipalName,m.Municipal_code,m.VOEN,t.YVOK,t.VOEN VergiOdeyiciVoen, " +
                                                                    "t.Individual_Legal,t.Concession,t.ConcessionGiveOrgan,t.ConcessionCause,t.ConcessionSeries," +
                                                                    "t.ConcessionNumber,t.ConcessionGiveDate from List_classification_Municipal m inner join Taxpayer t" +
                                                                    " on t.MunicipalID=m.MunicipalID where m.ServiceCode=1 and " +
                                                                    $"Pincode='{pincode}'");
                Taxpayer taxpayer = result.FirstOrDefault();

                if (_checking.TaxPayerIsNull(taxpayer)) return NotFound(new { message = "Bu FİN kod ilə vergi ödəyicisi tapılmadı!" });

                var residentialPropertyTax = await connection.QueryAsync<PropertyTax>("Select ROW_NUMBER() over (order by livingAreaid) sn,RegistrDate1, DocumentNumber, RegisterDocumentNumber, " +
                                                                    "GivingDate1,unvan, Zonaname, TaxRate, GeneralArea, DiffGeneralArea, mebleg, N'Yaşayış sahəsi üzrə əmlak vergisi' VergiNov " +
                                                                    "from viewLivingProperty where ExitDate is null and " +
                                                                    $"TaxPayerId={taxpayer.TaxpayerID}");

                List<PropertyTax> properties = residentialPropertyTax.ToList();
                foreach (var item in properties)
                {
                    PropertyTaxViewModel propertyTaxViewModel = new PropertyTaxViewModel
                    {
                         S_N = item.sn,
                         Sənədin_Reyestr_Nömrəsi= item.DocumentNumber,
                         Mülkiyyət_Sənədinin_Verilmə_Tarixi = item.GivingDate1,
                         Vergiyə_Cəlb_Edilən_Sahə = item.DiffGeneralArea,
                         Məbləğ = item.mebleg,
                         Ümumi_Sahə = item.GeneralArea,
                         Qeydiyyat_Nömrəsi = item.RegisterDocumentNumber,
                         Verginin_Hesablanma_Tarixi = item.RegistrDate1,
                         Vergi_Dərəcəsi = item.TaxRate,
                         Vergi_Növü = item.VergiNov,
                         Yerləşdiyi_Ünvan = item.unvan,
                         Zona_Əmsalı = item.Zonaname,
                    };
                    _properties.Add(propertyTaxViewModel);
                }

                TaxPayerByPropertyTax taxPayerByPropertyTax = new TaxPayerByPropertyTax
                {
                    Vergi_Ödəyicisinin_Güzəşti_Barədə_Məlumat = taxpayer.Concession,
                    Güzəşt_Sənədinin_Nömrəsi = taxpayer.ConcesionNumber,
                    Güzəştin_Səbəbi = taxpayer.ConcessionCause,
                    Güzəşt_Sənədinin_Verilmə_Tarixi = taxpayer.ConcessionGiveDate,
                    Güzəşt_Sənədini_Verən_Təşkilat =taxpayer.ConcessionGiveOrgan,
                    Güzəşt_Sənədinin_Seriyası = taxpayer.ConcessionSeries,
                    Bələdiyyə_Kodu = taxpayer.Municipal_code,
                    Vergi_Ödəyicisinin_Statusu = taxpayer.Individual_Legal,
                    Bələdiyyə_Adı = taxpayer.MunicipalName,
                    Bələdiyyə_VOEN = taxpayer.VOEN,
                    YVOK = taxpayer.YVOK,
                    V_O_VOEN = taxpayer.VergiOdeyiciVoen,
                    PropertyTaxes = _properties.ToList(),
                };

                return Ok(taxPayerByPropertyTax);
            }
        }

        [HttpGet("get-residential-land-tax/{pincode}")]
        public async Task<IActionResult> GetResidentialLandTax(string pincode)
        {
            if (!_checking.PinCode(pincode)) return BadRequest(new { message = " Yanlış format! FIN kod 7 rəqəm vəya hərfdən ibarət olmalıdır." });

            string connectionString = _configuration.GetConnectionString("DefaultConnectionMSSQL");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<Taxpayer>("Select t.TaxpayerID,m.MunicipalName,m.Municipal_code,m.VOEN,t.YVOK,t.VOEN VergiOdeyiciVoen, " +
                                                                    "t.Individual_Legal,t.Concession,t.ConcessionGiveOrgan,t.ConcessionCause,t.ConcessionSeries," +
                                                                    "t.ConcessionNumber,t.ConcessionGiveDate from List_classification_Municipal m inner join Taxpayer t" +
                                                                    " on t.MunicipalID=m.MunicipalID where m.ServiceCode=1 and " +
                                                                    $"Pincode='{pincode}'");

                Taxpayer taxpayer = result.FirstOrDefault();

                if (_checking.TaxPayerIsNull(taxpayer)) return NotFound(new { message = "Bu FİN kod ilə vergi ödəyicisi tapılmadı!" });

                var residentialLandTax = await connection.QueryAsync<LandTax>("Select ROW_NUMBER() over (order by livingAreaid) sn,RegistrDate1, DocumentNumber, RegisterDocumentNumber, " +
                                                                    "GivingDate1,unvan, TaxRate, GeneralArea, TypeUseLand,TypeUseLandFrom, Mebleg, N'Yaşayış sahəsi üzrə torpaq vergisi' VergiNov " +
                                                                    "from viewLivingLand where ExitDate is null and " +
                                                                     $"TaxPayerId{taxpayer.TaxpayerID}");

                List<LandTax> landTax = residentialLandTax.ToList();

                foreach (var item in landTax)
                {
                    LandTaxViewModel land = new LandTaxViewModel
                    {
                        S_N = item.sn,
                        Mülkiyyət_Sənədinin_Verilmə_Tarixi = item.GivingDate1,
                        Məbləğ = item.Mebleg,
                        İcarə_verən = item.TypeUseLandFrom,
                        Vergi_Dərəcəsi = item.TaxRate,
                        Sənədin_Reyestr_Nömrəsi = item.DocumentNumber,
                        Qeydiyyat_Nömrəsi = item.RegisterDocumentNumber,
                        Yerləşdiyi_Ünvan = item.unvan,
                        Verginin_Hesablanma_Tarixi = item.RegistrDate1,
                        Ümumi_Sahə = item.GeneralArea,
                        Torpağın_İstifadə_Növü = item.TypeUseLand,
                        Vergi_Növü = item.VergiNov
                    };
                    _lands.Add(land);
                };

                TaxPayerByLandTax taxPayerByLandTax = new TaxPayerByLandTax
                {
                    Vergi_Ödəyicisinin_Güzəşti_Barədə_Məlumat = taxpayer.Concession,
                    Güzəşt_Sənədinin_Nömrəsi = taxpayer.ConcesionNumber,
                    Güzəştin_Səbəbi = taxpayer.ConcessionCause,
                    Güzəşt_Sənədinin_Verilmə_Tarixi = taxpayer.ConcessionGiveDate,
                    Güzəşt_Sənədini_Verən_Təşkilat = taxpayer.ConcessionGiveOrgan,
                    Güzəşt_Sənədinin_Seriyası = taxpayer.ConcessionSeries,
                    Bələdiyyə_Kodu = taxpayer.Municipal_code,
                    Vergi_Ödəyicisinin_Statusu = taxpayer.Individual_Legal,
                    Bələdiyyə_Adı = taxpayer.MunicipalName,
                    Bələdiyyə_VOEN = taxpayer.VOEN,
                    YVOK = taxpayer.YVOK,
                    V_O_VOEN = taxpayer.VergiOdeyiciVoen,
                    LandTaxes = _lands.ToList(),
                };
                return Ok(taxPayerByLandTax);
            }
        }
    }
}