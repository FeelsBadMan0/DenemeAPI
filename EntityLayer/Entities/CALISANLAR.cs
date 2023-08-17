using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class CALISANLAR
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string ADSOYAD { get; set; }
        public string  DEPARTMAN{ get; set; }
        public DEPARTMANLAR DEPARTMANLAR { get; set; }
        public short AKTIF { get; set; }
    }
}
