using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class ROLLER
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string ROL { get; set; }

        public List<KULLANICILAR> kULLANICILARs { get; set; }
    }
}
