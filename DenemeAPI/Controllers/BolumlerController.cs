using DataAccessLayer.GenericRepository;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Policy;
using System.Web;
using System.Web.Http;

namespace DenemeAPI.Controllers
{
    public class BolumlerController : ApiController
    {
        private readonly GenericRepository _repo;

        public BolumlerController()
        {
            _repo = new GenericRepository();
        }


        [HttpGet]
        public IHttpActionResult BolumListele()
        {
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;
            if (UYGULAMA == "ef83de23-3d98-4db2-a558-402d3a2d8831")
            {
                List<BOLUMLER> listele = _repo.Query<BOLUMLER>("Select * from BOLUMLER");
                return Ok(listele);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public IHttpActionResult BolumEkle(BOLUMLER bolumler)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='ef83de23-3d98-4db2-a558-402d3a2d8831'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                @KULLANICIADI = KULLANICIADI
            };

            KULLANICILAR erisim = _repo.QueryFirstOrDefault<KULLANICILAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(bolumler.BOLUM))
                {
                    string bolumEkle = "Insert into BOLUMLER (BOLUM,KAYITKODU) values (@BOLUM,@KAYITKODU)";
                    bolumler.KAYITKODU = Guid.NewGuid().ToString();
                    object parametreler2 = new
                    {
                        @BOLUM = bolumler.BOLUM,
                        @KAYITKODU = bolumler.KAYITKODU
                    };

                    int ekle = _repo.Execute(bolumEkle, parametreler2);

                    if (ekle > 0)
                    {
                        return Ok("Bölüm Başarıyla Eklendi!");
                    }
                    else
                    {
                        return BadRequest("Bölüm Eklenirken Bir Hata Oluştu!");
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
        public IHttpActionResult BolumGetir(BOLUMLER bolumler)
        {

            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;

            if (UYGULAMA == "ef83de23-3d98-4db2-a558-402d3a2d8831")
            {
                if (!string.IsNullOrEmpty(bolumler.KAYITKODU))
                {
                    object parametreler = new
                    {
                        @KAYITKODU = bolumler.KAYITKODU
                    };

                    string rolGetir = "Select * from BOLUMLER where KAYITKODU=@KAYITKODU";

                    BOLUMLER getir = _repo.QueryFirstOrDefault<BOLUMLER>(rolGetir, parametreler);

                    if (getir != null)
                    {
                        return Ok(getir);
                    }
                    else
                    {
                        return BadRequest("Bölüm Bulunamadı!");
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
        public IHttpActionResult BolumGuncelle(BOLUMLER bolumler)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='ef83de23-3d98-4db2-a558-402d3a2d8831'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            HASTALAR erisim = _repo.QueryFirstOrDefault<HASTALAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(bolumler.BOLUM) && !string.IsNullOrEmpty(bolumler.KAYITKODU))
                {
                    string bolumGuncelle = "Update BOLUMLER set BOLUM=@BOLUM where KAYITKODU=@KAYITKODU";

                    object parametreler2 = new
                    {
                        @BOLUM=bolumler.BOLUM,
                        @KAYITKODU=bolumler.KAYITKODU
                    };

                    int guncelle = _repo.Execute(bolumGuncelle, parametreler2);

                    if(guncelle > 0)
                    {
                        return Ok("Güncelleme İşlemi Başarılı!");
                    }
                    else
                    {
                        return BadRequest("Güncelleme İşlemi Gerçekleştirilirken Bir Hata Oluştu!");
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