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

namespace DenemeAPI.Controllers
{
    public class CalisanlarController : ApiController
    {
        private readonly GenericRepository _repo;

        public CalisanlarController()
        {
            _repo = new GenericRepository();
        }


        [HttpGet]
        public List<CALISANLAR> CalisanListele()
        {

            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;
            string ROL = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13" && ROL == "e19d09bc-2568-4855-b91f-1ec37b8eee07" || ROL == "9aaba3cf-d130-4763-b039-94c5a839fcf4")
            {
                return _repo.Query<CALISANLAR>("Select * from CALISANLAR where AKTIF=1");
            }
            else
            {
                return null;
            }
        }


        [HttpPost]
        public IHttpActionResult CalisanEkle(CALISANLAR calisan)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            CALISANLAR erisim = _repo.QueryFirstOrDefault<CALISANLAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(calisan.ADSOYAD) && !string.IsNullOrEmpty(calisan.DEPARTMAN))
                {
                    calisan.KAYITKODU = Guid.NewGuid().ToString();
                    calisan.AKTIF = 1;
                    string uygulamaEkle = "Insert into CALISANLAR (KAYITKODU,ADSOYAD,DEPARTMAN,AKTIF) values (@KAYITKODU,@ADSOYAD,@DEPARTMAN,@AKTIF)";

                    object parametreler2 = new
                    {
                        @KAYITKODU = calisan.KAYITKODU,
                        @ADSOYAD = calisan.ADSOYAD,
                        @DEPARTMAN = calisan.DEPARTMAN,
                        @AKTIF = calisan.AKTIF
                    };

                    int ekle = _repo.Execute(uygulamaEkle, parametreler2);

                    if (ekle > 0)
                    {
                        return Ok("Çalışsan Başarıyla Eklendi!");
                    }
                    else
                    {
                        return BadRequest("Çalışan Eklenirken Bir Hata Oluştu!");
                    }
                }
                else
                {
                    return BadRequest("Parametreler Boş Bırakılamaz!");
                }

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut]
        public IHttpActionResult CalisanSil(CALISANLAR calisan)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            CALISANLAR erisim = _repo.QueryFirstOrDefault<CALISANLAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(calisan.KAYITKODU))
                {
                    string calisanSil = "Update CALISANLAR set AKTIF=0 where KAYITKODU=@KAYITKODU";

                    object parametreler2 = new
                    {
                        @KAYITKODU = calisan.KAYITKODU
                    };

                    int sil = _repo.Execute(calisanSil, parametreler2);

                    if (sil > 0)
                    {
                        return Ok("Silme İşlemi Başarılı!");
                    }
                    else
                    {
                        return BadRequest("Silme İşlemi Gerçekleştirilirken Bir Hata Oluştu!");
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

        [HttpGet]
        public IHttpActionResult CalisanGetir(CALISANLAR calisan)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;
            string ROL = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13" && ROL == "e19d09bc-2568-4855-b91f-1ec37b8eee07" || ROL == "9aaba3cf-d130-4763-b039-94c5a839fcf4")
            {
                if (!string.IsNullOrEmpty(calisan.KAYITKODU))
                {
                    object parametreler = new
                    {
                        @KAYITKODU = calisan.KAYITKODU
                    };

                    string rolGetir = "Select * from CALISANLAR where KAYITKODU=@KAYITKODU";

                    UYGULAMALAR getir = _repo.QueryFirstOrDefault<UYGULAMALAR>(rolGetir, parametreler);

                    if (getir != null)
                    {
                        return Ok(getir);
                    }
                    else
                    {
                        return BadRequest("Çalışan Bulunamadı!");
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
        public IHttpActionResult CalisanGuncelle(CALISANLAR calisan)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            CALISANLAR erisim = _repo.QueryFirstOrDefault<CALISANLAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(calisan.ADSOYAD) && !string.IsNullOrEmpty(calisan.DEPARTMAN) && !string.IsNullOrEmpty(calisan.KAYITKODU))
                {
                    string calisanGuncelle = "Update CALISANLAR set ADSOYAD=@ADSOYAD,DEPARTMAN=@DEPARTMAN where KAYITKODU=@KAYITKODU";

                    object parametreler2 = new
                    {
                        @ADSOYAD = calisan.ADSOYAD,
                        @DEPARTMAN = calisan.DEPARTMAN,
                        @KAYITKODU = calisan.KAYITKODU
                    };

                    int guncelle = _repo.Execute(calisanGuncelle, parametreler2);

                    if (guncelle > 0)
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
