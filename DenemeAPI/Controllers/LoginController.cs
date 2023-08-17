using DataAccessLayer.GenericRepository;
using DenemeAPI.Hash;
using DenemeAPI.JWT;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace DenemeAPI.Controllers
{
    [AllowAnonymous]
    public class LoginController : ApiController
    {
        private readonly GenericRepository _repo;
        private readonly ParolaSifreleme _sifrele;

        public LoginController()
        {
            _repo = new GenericRepository();
            _sifrele = new ParolaSifreleme();
        }

        [HttpPost]
        public IHttpActionResult KullaniciGirisi(KULLANICILAR kullanici)
        {
            object parametreler = new
            {
                @KULLANICIADI=kullanici.KULLANICIADI
            };
            string sql = "Select * from KULLANICILAR WHERE KULLANICIADI=@KULLANICIADI AND AKTIF=1";

           KULLANICILAR getir= _repo.QueryFirstOrDefault<KULLANICILAR>(sql, parametreler);
            if(getir != null)
            {
                if (_sifrele.SifreDogrulama(kullanici.SIFRE, getir.SIFRELIPAROLA, getir.ANAHTARPAROLA))
                {

                    var token = JwtManeger.GetToken(getir.KULLANICIADI, getir.ROL, getir.UYGULAMA, getir.AKTIF);
                    return Ok(new { kullanici.KULLANICIADI, token });
                }
                else
                {
                    return BadRequest("Şifreniz Yanlış!");
                }
            }
            else
            {
                return BadRequest("Kullanıcı Bulunamadı!");
            }
           
        }
    }
}
