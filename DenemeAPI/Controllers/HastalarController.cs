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
    public class HastalarController : ApiController
    {
        private readonly GenericRepository _repo;

        public HastalarController()
        {
            _repo = new GenericRepository();
        }


        [HttpGet]
        public List<HASTALAR> HastaListele()
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;

            if (UYGULAMA == "ef83de23-3d98-4db2-a558-402d3a2d8831")
            {
                return _repo.Query<HASTALAR>("Select h.ID,h.ADSOYAD,b.BOLUM,h.KAYITKODU,h.AKTIF from HASTALAR h INNER JOIN BOLUMLER b on b.KAYITKODU=h.BOLUM  where AKTIF=1");
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public IHttpActionResult HastaEkle(HASTALAR hasta)
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
                if (!string.IsNullOrEmpty(hasta.ADSOYAD) && !string.IsNullOrEmpty(hasta.BOLUM))
                {
                    hasta.KAYITKODU = Guid.NewGuid().ToString();
                    hasta.AKTIF = 1;

                    string hastaEkle = "Insert into HASTALAR (ADSOYAD,BOLUM,KAYITKODU,AKTIF) values (@ADSOYAD,@BOLUM,@KAYITKODU,@AKTIF)";

                    object parametreler2 = new
                    {
                        @ADSOYAD = hasta.ADSOYAD,
                        @BOLUM = hasta.BOLUM,
                        @KAYITKODU = hasta.KAYITKODU,
                        @AKTIF = hasta.AKTIF
                    };

                    int ekle = _repo.Execute(hastaEkle, parametreler2);

                    if (ekle > 0)
                    {
                        return Ok("Hasta Başarıyla Eklendi!");
                    }
                    else
                    {
                        return BadRequest("Hasta Eklenirken Bir Hata Oluştu!");
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



        [HttpPut]
        public IHttpActionResult HastaSil(HASTALAR hasta)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07' AND UYGULAMA='ef83de23-3d98-4db2-a558-402d3a2d8831'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            KULLANICILAR erisim = _repo.QueryFirstOrDefault<KULLANICILAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(hasta.KAYITKODU))
                {
                    string hastaSil = "Update HASTALAR set AKTIF=0 where KAYITKODU=@KAYITKODU";

                    object parametreler2 = new
                    {
                        @KAYITKODU = hasta.KAYITKODU
                    };

                    int sil = _repo.Execute(hastaSil, parametreler2);

                    if (sil > 0)
                    {
                        return Ok("Hasta Başarıyla Silindi!");
                    }
                    else
                    {
                        return BadRequest("Hasta Silinirken Bir Hata Oluştu!");
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
        public IHttpActionResult HastaGetir(HASTALAR hasta)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;

            if (UYGULAMA == "ef83de23-3d98-4db2-a558-402d3a2d8831")
            {
                if (!string.IsNullOrEmpty(hasta.KAYITKODU))
                {
                    object parametreler = new
                    {
                        @KAYITKODU = hasta.KAYITKODU
                    };

                    string rolGetir = "Select h.ID,h.ADSOYAD,b.BOLUM,h.KAYITKODU,h.AKTIF from HASTALAR h INNER JOIN BOLUMLER b on b.KAYITKODU=h.BOLUM  where AKTIF=1 and h.KAYITKODU=@KAYITKODU";

                    HASTALAR getir = _repo.QueryFirstOrDefault<HASTALAR>(rolGetir, parametreler);

                    if (getir != null)
                    {
                        return Ok(getir);
                    }
                    else
                    {
                        return BadRequest("Hasta Bulunamadı!");
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
        public IHttpActionResult HastaGuncelle(HASTALAR hasta)
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
                if (!string.IsNullOrEmpty(hasta.ADSOYAD) && !string.IsNullOrEmpty(hasta.KAYITKODU) && !string.IsNullOrEmpty(hasta.BOLUM))
                {
                    string hastaGuncelle = "Update HASTALAR set ADSOYAD=@ADSOYAD,BOLUM=@BOLUM where KAYITKODU=@KAYITKODU";

                    object parametreler2 = new
                    {
                        @ADSOYAD=hasta.ADSOYAD,
                        @BOLUM=hasta.BOLUM,
                        @KAYITKODU=hasta.KAYITKODU
                    };

                    int guncelle = _repo.Execute(hastaGuncelle,parametreler2);

                    if(guncelle > 0)
                    {
                        return Ok("Hasta Başarıyla Güncellendi!");
                    }
                    else
                    {
                        return BadRequest("Hasta Güncellenirken Bir Hata Oluştu!");
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
