using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities
{
    public class ItemVariacaoPatrimonialDiminutiva
    {
        public ItemVariacaoPatrimonialDiminutiva()
        {
            this.ItemVPDCronogramaDesembolso = new List<ItemVPDCronogramaDesembolso>();
        }

        public int VariacaoPatrimonialDiminutivaItemID { get; set; }
        public int DocumentoID { get; set; }
        public Nullable<int> FonteRecursoID { get; set; }
        public Nullable<int> NaturezaDespesaID { get; set; }
        public decimal ValorSolicitado { get; set; }
        public decimal ValorEstorno { get; set; }
        public string TipoVPD { get; set; }
        public Nullable<int> IdentificadorUsoCodigo { get; set; }
        public Nullable<int> DominioIDTipoPatrimonial { get; set; }
        public Nullable<int> EmpenhoID { get; set; }
        //public virtual Documento Documento { get; set; }
        //public virtual Documento Documento1 { get; set; }
        public virtual ICollection<ItemVPDCronogramaDesembolso> ItemVPDCronogramaDesembolso { get; set; }
        //public virtual NaturezaDespesa NaturezaDespesa { get; set; }
    }
}
