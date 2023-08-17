using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class DEPARTMANLAR
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string DEPARTMAN { get; set; }

        public List<CALISANLAR> CALISANLARs { get; set; }
    }
}
