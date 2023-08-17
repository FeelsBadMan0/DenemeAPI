using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class KULLANICILAR
    {
        public int ID { get; set; }
        public string  KAYITKODU { get; set; }
        public string KULLANICIADI { get; set; }
        public string SIFRE { get; set; }
        public byte[] SIFRELIPAROLA { get; set; }
        public byte[] ANAHTARPAROLA { get; set; }
        public string UYGULAMA { get; set; }
        public UYGULAMALAR UYGULAMALAR { get; set; }
        public string ROL { get; set; }
        public ROLLER ROLLER { get; set; }
        public DateTime KAYITTARIHI { get; set; }
        public short AKTIF { get; set; }
    }
}
