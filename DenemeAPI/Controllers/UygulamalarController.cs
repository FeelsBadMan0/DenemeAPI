using DataAccessLayer.GenericRepository;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace DenemeAPI.Controllers
{
    public class UygulamalarController : ApiController
    {
        private readonly GenericRepository _repo;

        public UygulamalarController()
        {
            _repo = new GenericRepository();
        }


        [HttpGet]
        public List<UYGULAMALAR> UygulamaListele()
        {
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;
            string ROL = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13" && ROL == "e19d09bc-2568-4855-b91f-1ec37b8eee07")
            {
                return _repo.Query<UYGULAMALAR>("Select * from UYGULAMALAR");
            }
            else
            {
                return null;
            }

        }


        [HttpPost]
        public IHttpActionResult UygulamaEkle(UYGULAMALAR uygulama)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07' AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler2 = new
            {
                KULLANICIADI = KULLANICIADI
            };

            ROLLER erisim = _repo.QueryFirstOrDefault<ROLLER>(kontrol, parametreler2);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(uygulama.UYGULAMAADI))
                {
                    uygulama.KAYITKODU = Guid.NewGuid().ToString();
                    object parametreler = new
                    {
                        @KAYITKODU = uygulama.KAYITKODU,
                        @UYGULAMAADI = uygulama.UYGULAMAADI
                    };
                    string sql = "INSERT INTO UYGULAMALAR (UYGULAMAADI,KAYITKODU) values (@UYGULAMAADI,@KAYITKODU)";
                    int ekle = _repo.Execute(sql, parametreler);
                    if (ekle > 0)
                    {
                        return Ok("Uygulama Ekleme Başarılı");
                    }
                    else
                    {
                        return BadRequest("Uygulama Eklenirken Bir Hata Oluştu");
                    }
                }
                else
                {
                    return BadRequest("Parametreler Boş Geçilemez!");
                }
            
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet]
        public IHttpActionResult UygulamaGetir(UYGULAMALAR uygulama)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;
            string ROL = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13" && ROL== "e19d09bc-2568-4855-b91f-1ec37b8eee07")
            {
                if (!string.IsNullOrEmpty(uygulama.KAYITKODU))
                {
                    object parametreler = new
                    {
                        @KAYITKODU = uygulama.KAYITKODU
                    };

                    string rolGetir = "Select * from UYGULAMALAR where KAYITKODU=@KAYITKODU";

                    UYGULAMALAR getir = _repo.QueryFirstOrDefault<UYGULAMALAR>(rolGetir, parametreler);

                    if (getir != null)
                    {
                        return Ok(getir);
                    }
                    else
                    {
                        return BadRequest("Uygulama Bulunamadı!");
                    }
                }
                else
                {
                    return BadRequest("KAYIT KODU Boş Geçilemez!");
                }

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut]
        public IHttpActionResult UygulamaGuncelle(UYGULAMALAR uygulama)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07' AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            UYGULAMALAR erisim = _repo.QueryFirstOrDefault<UYGULAMALAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(uygulama.UYGULAMAADI) && !string.IsNullOrEmpty(uygulama.KAYITKODU))
                {

                    string guncelle = "Update UYGULAMALAR set UYGULAMAADI=@UYGULAMAADI where KAYITKODU=@KAYITKODU";

                    object parametreler2 = new
                    {
                        @UYGULAMAADI = uygulama.UYGULAMAADI,
                        @KAYITKODU = uygulama.KAYITKODU
                    };

                    int rolGuncelle = _repo.Execute(guncelle, parametreler2);

                    if (rolGuncelle > 0)
                    {
                        return Ok("Uygulama Başarıyla Güncellendi!");
                    }
                    else
                    {
                        return BadRequest("Uygulama Güncellenirken Bir Hata Oluştu!");
                    }
                }
                else
                {
                    return BadRequest("Parametreler Boş Geçilemez!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
