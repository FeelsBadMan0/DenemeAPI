using DataAccessLayer.GenericRepository;
using DenemeAPI.Hash;
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
    [Authorize]
    public class KullanicilarController : ApiController
    {
        private readonly GenericRepository _repo;
        private readonly ParolaSifreleme _sifrele;

        public KullanicilarController()
        {
            _repo = new GenericRepository();
            _sifrele = new ParolaSifreleme();
        }

        [HttpGet]
        public List<KULLANICILAR> KullaniciListele()
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13")
            {
                string listele = "Select k.ID,k.KAYITKODU,k.KULLANICIADI,u.UYGULAMAADI AS 'UYGULAMA',r.ROL,k.KAYITTARIHI,k.AKTIF from KULLANICILAR k INNER JOIN UYGULAMALAR u on u.KAYITKODU=k.UYGULAMA INNER JOIN ROLLER r on r.KAYITKODU=k.ROL WHERE K.UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13' and k.AKTIF=1";
                return _repo.Query<KULLANICILAR>(listele);
            }
            else
            {
                return null;
            }


        }


        [HttpPost]
        public IHttpActionResult KullaniciEkle(KULLANICILAR kullanici)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler2 = new
            {
                KULLANICIADI = KULLANICIADI
            };

            KULLANICILAR erisim = _repo.QueryFirstOrDefault<KULLANICILAR>(kontrol, parametreler2);

            if (erisim != null)
            {

                if (!string.IsNullOrEmpty(kullanici.KULLANICIADI) && !string.IsNullOrEmpty(kullanici.SIFRE) && !string.IsNullOrEmpty(kullanici.ROL))
                {
                    string kullaniciAdiKontrol = "Select * from KULLANICILAR where KULLANICIADI=@KULLANICIADI";

                    object parametreler3 = new
                    {
                        @KULLANICIADI = kullanici.KULLANICIADI
                    };

                    KULLANICILAR kAdiKontrol = _repo.QueryFirstOrDefault<KULLANICILAR>(kullaniciAdiKontrol, parametreler3);

                    if (kAdiKontrol == null)
                    {

                        kullanici.KAYITKODU = Guid.NewGuid().ToString();
                        kullanici.KAYITTARIHI = DateTime.Now;
                        kullanici.AKTIF = 1;
                        _sifrele.ParolaSifrele(kullanici.SIFRE, out byte[] sifreliparola, out byte[] anahtarparola);
                        object parametreler = new
                        {
                            @KAYITKODU = kullanici.KAYITKODU,
                            @KULLANICIADI = kullanici.KULLANICIADI,
                            @SIFRELIPAROLA = sifreliparola,
                            @ANAHTARPAROLA = anahtarparola,
                            @UYGULAMA = kullanici.UYGULAMA,
                            @ROL = kullanici.ROL,
                            @KAYITTARIHI = kullanici.KAYITTARIHI,
                            @AKTIF = kullanici.AKTIF
                        };
                        string sql = "INSERT INTO KULLANICILAR (KAYITKODU,KULLANICIADI,SIFRELIPAROLA,ANAHTARPAROLA,UYGULAMA,ROL,KAYITTARIHI,AKTIF) values (@KAYITKODU,@KULLANICIADI,@SIFRELIPAROLA,@ANAHTARPAROLA,@UYGULAMA,@ROL,@KAYITTARIHI,@AKTIF)";
                        int ekle = _repo.Execute(sql, parametreler);
                        if (ekle > 0)
                        {
                            return Ok("Kullanıcı Ekleme Başarılı");
                        }
                        else
                        {
                            return BadRequest("Kullanıcı Eklenirken Bir Hata Oluştu");
                        }
                    }
                    else
                    {
                        return BadRequest("Kullanıcı Adı Sistemde Mevcut!");
                    }

                }
                else
                {
                    return BadRequest("Kullanıcı Parametreleri Boş Geçilemez!");
                }

            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPut]
        public IHttpActionResult KullaniciSil(KULLANICILAR kullanici)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07' AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            KULLANICILAR erisim = _repo.QueryFirstOrDefault<KULLANICILAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(kullanici.KAYITKODU))
                {
                    string kullaniciSil = "Update KULLANICILAR set AKTIF=0 where KAYITKODU=@KAYITKODU";

                    object parametreler2 = new
                    {
                        @KAYITKODU = kullanici.KAYITKODU
                    };

                    int sil = _repo.Execute(kullaniciSil, parametreler2);

                    if (sil > 0)
                    {
                        return Ok("Kullanıcı Başarıyla Silindi!");
                    }
                    else
                    {
                        return BadRequest("Kullanıcı Silinirken Bir Hata Oluştu!");
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
        public IHttpActionResult KullaniciGetir(KULLANICILAR kullanici)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13")
            {
                if (!string.IsNullOrEmpty(kullanici.KAYITKODU))
                {
                    string kullaniciGetir = "Select k.ID,k.KAYITKODU,k.KULLANICIADI,u.UYGULAMAADI AS 'UYGULAMA',r.ROL,k.KAYITTARIHI,k.AKTIF from KULLANICILAR k INNER JOIN UYGULAMALAR u on u.KAYITKODU=k.UYGULAMA INNER JOIN ROLLER r on r.KAYITKODU=k.ROL WHERE K.UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13' and k.AKTIF=1 AND k.KAYITKODU=@KAYITKODU";
                    object parametreler = new
                    {
                        @KAYITKODU = kullanici.KAYITKODU
                    };

                    KULLANICILAR getir = _repo.QueryFirstOrDefault<KULLANICILAR>(kullaniciGetir, parametreler);

                    if (getir != null)
                    {
                        return Ok(getir);
                    }
                    else
                    {
                        return BadRequest("Kullanıcı Bulunamadı!");
                    }
                }
                else
                {
                    return BadRequest("KAYIT KODU Boş Bırakılamaz!");
                }

            }
            else
            {
                return Unauthorized();
            }
        }



        [HttpPut]
        public IHttpActionResult KullaniciGuncelle(KULLANICILAR kullanici)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler2 = new
            {
                KULLANICIADI = KULLANICIADI
            };

            KULLANICILAR erisim = _repo.QueryFirstOrDefault<KULLANICILAR>(kontrol, parametreler2);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(kullanici.KULLANICIADI) && !string.IsNullOrEmpty(kullanici.SIFRE) && !string.IsNullOrEmpty(kullanici.ROL) && !string.IsNullOrEmpty(kullanici.KAYITKODU))
                {
                    string kullaniciAdiKontrol = "Select * from KULLANICILAR where KULLANICIADI=@KULLANICIADI";

                    object parametreler3 = new
                    {
                        @KULLANICIADI = kullanici.KULLANICIADI
                    };

                    KULLANICILAR kAdiKontrol = _repo.QueryFirstOrDefault<KULLANICILAR>(kullaniciAdiKontrol, parametreler3);

                    if (kAdiKontrol == null)
                    {

                        string guncelle = "Update KULLANICILAR set KULLANICIADI=@KULLANICIADI,SIFRELIPAROLA=@SIFRELIPAROLA,ANAHTARPAROLA=@ANAHTARPAROLA, ROL=@ROL WHERE KAYITKODU=@KAYITKODU";
                        _sifrele.ParolaSifrele(kullanici.SIFRE, out byte[] sifreliparola, out byte[] anahtarparola);

                        object parametreler = new
                        {
                            @KULLANICIADI = kullanici.KULLANICIADI,
                            @SIFRELIPAROLA = sifreliparola,
                            @ANAHTARPAROLA = anahtarparola,
                            @ROL = kullanici.ROL,
                            @KAYITKODU = kullanici.KAYITKODU
                        };

                        int kGuncelle = _repo.Execute(guncelle, parametreler);

                        if (kGuncelle > 0)
                        {
                            return Ok("Kullanıcı Başarıyla Güncellendi!");
                        }
                        else
                        {
                            return BadRequest("Kullanıcı Eklenirken Bir Hata Oluştu!");
                        }
                    }
                    else
                    {
                        return BadRequest("Kullanıcı Adı Sistemde Mevcut!");
                    }
                }
                else
                {
                    return BadRequest("Kullanıcı Bilgileri Boş Geçilemez!");
                }


            }
            else
            {
                return Unauthorized();
            }
        }
    }
}