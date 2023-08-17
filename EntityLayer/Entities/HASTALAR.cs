using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class HASTALAR
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string ADSOYAD { get; set; }
        public string BOLUM { get; set; }
        public BOLUMLER BOLUMLER { get; set; }
        public short AKTIF { get; set; }
    }
}
