using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities
{
    public partial class ItemVPDCronogramaDesembolso
    {
        public int ItemVPDCronogramaDesembolsoID { get; set; }
        public int CronogramaDesembolsoID { get; set; }
        public int VariacaoPatrimonialDiminutivaItemID { get; set; }
        public decimal Valor { get; set; }
        public string Devolucao { get; set; }
        //public virtual CronogramaDesembolso CronogramaDesembolso { get; set; }
        //public virtual ItemVariacaoPatrimonialDiminutiva ItemVariacaoPatrimonialDiminutiva { get; set; }
    }
}
