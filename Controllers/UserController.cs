using BAMS.Databse;
using BAMS.Databse.Models;
using BAMS.Databse.ViewModels;
using BAMS.Services.Implementation;
using BAMS.Services.Interface;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BAMS.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICheck _checking;

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

                TaxPayerByPropertyTax taxPayerByPropertyTax = new TaxPayerByPropertyTax
                {
                    Vergi_Ödəyicisinin_Güzəşti_Barədə_Məlumat = taxpayer.Concession == "1" ? "Güzəştsiz" : "Güzəştli",
                    Güzəşt_Sənədinin_Nömrəsi = taxpayer.ConcesionNumber,
                    Güzəştin_Səbəbi = taxpayer.ConcessionCause,
                    Güzəşt_Sənədinin_Verilmə_Tarixi = taxpayer.ConcessionGiveDate,
                    Güzəşt_Sənədini_Verən_Təşkilat =taxpayer.ConcessionGiveOrgan,
                    Güzəşt_Sənədinin_Seriyası = taxpayer.ConcessionSeries,
                    Bələdiyyə_Kodu = taxpayer.Municipal_code,
                    Vergi_Ödəyicisinin_Statusu = taxpayer.Individual_Legal == "1" ? "Fiziki" : "Huquqi",
                    Bələdiyyə_Adı = taxpayer.MunicipalName,
                    Bələdiyyə_VOEN = taxpayer.VOEN,
                    YVOK = taxpayer.YVOK,
                    V_O_VOEN = taxpayer.VergiOdeyiciVoen,
                    PropertyTaxes = residentialPropertyTax.ToList(),
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
                                                                    "GivingDate1,unvan, Zonaname, TaxRate, GeneralArea, DiffGeneralArea, mebleg, N'Yaşayış sahəsi üzrə əmlak vergisi' VergiNov " +
                                                                    "from viewLivingProperty where ExitDate is null and " +
                                                                     $"TaxPayerId={taxpayer.TaxpayerID}");
                TaxPayerByLandTax taxPayerByLandTax = new TaxPayerByLandTax
                {
                    Concession = taxpayer.Concession,
                    ConcesionNumber = taxpayer.ConcesionNumber,
                    ConcessionCause = taxpayer.ConcessionCause,
                    ConcessionGiveDate = taxpayer.ConcessionGiveDate,
                    ConcessionGiveOrgan = taxpayer.ConcessionGiveOrgan,
                    ConcessionSeries = taxpayer.ConcessionSeries,
                    Municipal_code = taxpayer.Municipal_code,
                    Individual_Legal = taxpayer.Individual_Legal,
                    MunicipalName = taxpayer.MunicipalName,
                    TaxpayerID = taxpayer.TaxpayerID,
                    VOEN = taxpayer.VOEN,
                    YVOK = taxpayer.YVOK,
                    VergiOdeyiciVoen = taxpayer.VergiOdeyiciVoen,
                    LandTaxes = residentialLandTax.ToList(),
                };
                return Ok(taxPayerByLandTax);
            }
        }
    }
}