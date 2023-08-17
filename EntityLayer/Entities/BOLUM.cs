using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class BOLUMLER
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string BOLUM { get; set; }

        public List<HASTALAR> HASTALARs { get; set; }
    }
}
