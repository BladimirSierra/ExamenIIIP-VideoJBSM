using System;
using System.Collections.Generic;
using System.Text;

namespace ExamenIIIP.Models
{
    public class Notas
    {
        public string Id_Nota { get; set; }
        public string Descripcion { get; set; }
        public string Fecha { get; set; }
        public string PhotoRecord { get; set; }
        public Byte[] AudioRecord { get; set; }
     
    }
}
