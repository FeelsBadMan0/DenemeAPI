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
    public class RollerController : ApiController
    {
        private readonly GenericRepository _repo;

        public RollerController()
        {
            _repo = new GenericRepository();
        }

        [HttpGet]
        public List<ROLLER> RolListele()
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13")
            {
                return _repo.Query<ROLLER>("Select * from ROLLER");
            }
            else
            {
                return null;
            }
        }


        [HttpPost]
        public IHttpActionResult RolEkle(ROLLER roller)
        {

            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler2 = new
            {
                KULLANICIADI = KULLANICIADI
            };

            ROLLER erisim = _repo.QueryFirstOrDefault<ROLLER>(kontrol, parametreler2);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(roller.ROL))
                {
                    roller.KAYITKODU = Guid.NewGuid().ToString();
                    object parametreler = new
                    {
                        @KAYITKODU = roller.KAYITKODU,
                        @ROL = roller.ROL
                    };
                    string sql = "INSERT INTO ROLLER (KAYITKODU,ROL) values (@KAYITKODU,@ROL)";
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
        public IHttpActionResult RolGetir(ROLLER roller)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string UYGULAMA = claimsIdentity.FindFirst("UYGULAMA")?.Value;

            if (UYGULAMA == "787ca10d-98b2-4102-8bce-9907b075fb13")
            {
                if (!string.IsNullOrEmpty(roller.KAYITKODU))
                {
                    object parametreler = new
                    {
                        @KAYITKODU = roller.KAYITKODU
                    };

                    string rolGetir = "Select * from ROLLER where KAYITKODU=@KAYITKODU";

                    ROLLER getir = _repo.QueryFirstOrDefault<ROLLER>(rolGetir, parametreler);

                    if (getir != null)
                    {
                        return Ok(getir);
                    }
                    else
                    {
                        return BadRequest("Rol Bulunamadı!");
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
        public IHttpActionResult RolGuncelle(ROLLER roller)
        {
            //Kullanıcı bilgilerini JWT ile Claim kullanarak alıyoruz
            var claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            string KULLANICIADI = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            string kontrol = "Select * from KULLANICILAR where (ROL='9aaba3cf-d130-4763-b039-94c5a839fcf4'OR ROL='e19d09bc-2568-4855-b91f-1ec37b8eee07') AND UYGULAMA='787ca10d-98b2-4102-8bce-9907b075fb13'AND KULLANICIADI=@KULLANICIADI AND AKTIF=1";

            object parametreler = new
            {
                KULLANICIADI = KULLANICIADI
            };

            ROLLER erisim = _repo.QueryFirstOrDefault<ROLLER>(kontrol, parametreler);

            if (erisim != null)
            {
                if (!string.IsNullOrEmpty(roller.ROL) && !string.IsNullOrEmpty(roller.KAYITKODU))
                {
                    string rolKontrol = "Select * from ROLLER where ROL=@ROL";

                    object parametreler2 = new
                    {
                        @ROL = roller.ROL
                    };

                    ROLLER rolAdKontrol = _repo.QueryFirstOrDefault<ROLLER>(rolKontrol, parametreler2);

                    if (rolAdKontrol == null)
                    {

                        string guncelle = "Update ROLLER set ROL=@ROL where KAYITKODU=@KAYITKODU";

                        object parametreler3 = new
                        {
                            @ROL = roller.ROL,
                            @KAYITKODU = roller.KAYITKODU
                        };

                        int rolGuncelle = _repo.Execute(guncelle, parametreler3);

                        if (rolGuncelle > 0)
                        {
                            return Ok("Rol Başarıyla Güncellendi!");
                        }
                        else
                        {
                            return BadRequest("Rol Güncellenirken Bir Hata Oluştu!");
                        }
                    }
                    else
                    {
                        return BadRequest("Rol Sistemde Zaten Mevcut!");
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
    }
}