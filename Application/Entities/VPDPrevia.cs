using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities
{
    public class VPDPrevia
    {
        public int DocumentoID { get; set; }
        public Nullable<int> DocumentoOriginalID { get; set; }
        public int VariacaoPatrimonialDiminutivaItemID { get; set; }
        public int NaturezaDespesaID { get; set; }
        public string FonteRecurso { get; set; }
        public string NaturezaDespesa { get; set; }
        public decimal ValorSolicitado { get; set; }
        public decimal ValorEstorno { get; set; }
        public decimal TotalEstorno { get; set; }
        public decimal TotalEmpenhado { get; set; }
        public decimal TotalDevolvido { get; set; }
    }
}
