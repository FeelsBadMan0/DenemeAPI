using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class UYGULAMALAR
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string UYGULAMAADI { get; set; }
        public List<KULLANICILAR>  KULLANICILARs{ get; set; }
    }
}
