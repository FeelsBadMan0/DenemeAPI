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
    public class DepartmanlarController : ApiController
    {
        private readonly GenericRepository _repo;

        public DepartmanlarController()
        {
            _repo = new GenericRepository();
        }


        [HttpGet]
        public List<DEPARTMANLAR> DepartmanListele()
        {

            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;
            string ROL = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13")
            {
                return _repo.Query<DEPARTMANLAR>("Select * from DEPARTMANLAR");
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public IHttpActionResult DepartmanEkle(DEPARTMANLAR departmanlar)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            DEPARTMANLAR erisim = _repo.QueryFirstOrDefault<DEPARTMANLAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(departmanlar.DEPARTMAN))
                {
                    string departmanEkle = "Insert into DEPARTMANLAR (DEPARTMAN,KAYITKODU) values (@DEPARTMAN,@KAYITKODU)";

                    departmanlar.KAYITKODU = Guid.NewGuid().ToString();

                    object parametreler2 = new
                    {
                        @DEPARTMAN = departmanlar.DEPARTMAN,
                        @KAYITKODU = departmanlar.KAYITKODU

                    };

                    int ekle = _repo.Execute(departmanEkle, parametreler2);

                    if (ekle > 0)
                    {
                        return Ok("Departman Ekleme İşlemi Başarılı!");
                    }
                    else
                    {
                        return BadRequest("Departman Ekleme İşlemi Sırasında Bir Hata Oluştu!");
                    }
                }
                else
                {
                    return BadRequest("Parametreer Boş Geçilemez!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public IHttpActionResult DepartmanGetir(DEPARTMANLAR departman)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;
            string ROL = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13")
            {
                if (!string.IsNullOrEmpty(departman.KAYITKODU))
                {
                    object parametreler = new
                    {
                        @KAYITKODU = departman.KAYITKODU
                    };

                    string rolGetir = "Select * from DEPARTMANLAR where KAYITKODU=@KAYITKODU";

                    DEPARTMANLAR getir = _repo.QueryFirstOrDefault<DEPARTMANLAR>(rolGetir, parametreler);

                    if (getir != null)
                    {
                        return Ok(getir);
                    }
                    else
                    {
                        return BadRequest("Departman Bulunamadı!");
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
        public IHttpActionResult DepartmanGuncelle(DEPARTMANLAR departmanlar)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            DEPARTMANLAR erisim = _repo.QueryFirstOrDefault<DEPARTMANLAR>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(departmanlar.DEPARTMAN) && !string.IsNullOrEmpty(departmanlar.KAYITKODU))
                {

                    string departmanGuncelle = "Update DEPARTMANLAR set DEPARTMAN=@DEPARTMAN where KAYITKODU=@KAYITKODU";

                    object parametreler2 = new
                    {
                        @DEPARTMAN = departmanlar.DEPARTMAN,
                        @KAYITKODU = departmanlar.KAYITKODU
                    };

                    int guncelle = _repo.Execute(departmanGuncelle,parametreler2);

                    if(guncelle> 0)
                    {
                        return Ok("Güncelleme İşlemi Başarılı!");
                    }
                    else
                    {
                        return BadRequest("Güncelleme İşlemi Gerçekleşirken Bir Hata Oluştu!");
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
