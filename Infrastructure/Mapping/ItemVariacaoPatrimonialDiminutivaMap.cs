using Application.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Application.Mapping
{
    public class ItemVariacaoPatrimonialDiminutivaMap : EntityTypeConfiguration<ItemVariacaoPatrimonialDiminutiva>
    {
        public ItemVariacaoPatrimonialDiminutivaMap()
        {
            // Primary Key
            this.HasKey(t => t.VariacaoPatrimonialDiminutivaItemID);

            // Properties
            this.Property(t => t.TipoVPD)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ValorEstorno)
                .HasPrecision(15, 2);

            this.Property(t => t.ValorSolicitado)
                .HasPrecision(15, 2);

            // Table & Column Mappings
            this.ToTable("ItemVariacaoPatrimonialDiminutiva", "EXEFIN");
            this.Property(t => t.VariacaoPatrimonialDiminutivaItemID).HasColumnName("VariacaoPatrimonialDiminutivaItemID");
            this.Property(t => t.DocumentoID).HasColumnName("DocumentoID");
            this.Property(t => t.FonteRecursoID).HasColumnName("FonteRecursoID");
            this.Property(t => t.NaturezaDespesaID).HasColumnName("NaturezaDespesaID");
            this.Property(t => t.ValorSolicitado).HasColumnName("ValorSolicitado");
            this.Property(t => t.ValorEstorno).HasColumnName("ValorEstorno");
            this.Property(t => t.TipoVPD).HasColumnName("TipoVPD");
            this.Property(t => t.IdentificadorUsoCodigo).HasColumnName("IdentificadorUsoCodigo");
            this.Property(t => t.DominioIDTipoPatrimonial).HasColumnName("DominioIDTipoPatrimonial");
            this.Property(t => t.EmpenhoID).HasColumnName("EmpenhoID");

            // Relationships
            //this.HasRequired(t => t.Documento)
            //    .WithMany(t => t.ItemVariacaoPatrimonialDiminutiva)
            //    .HasForeignKey(d => d.DocumentoID)
            //    .WillCascadeOnDelete(false);

            //this.HasOptional(t => t.Documento1)
            //    .WithMany(t => t.ItemVariacaoPatrimonialDiminutiva1)
            //    .HasForeignKey(d => d.EmpenhoID);

            //this.HasOptional(t => t.NaturezaDespesa)
            //    .WithMany(t => t.ItemVariacaoPatrimonialDiminutiva)
            //    .HasForeignKey(d => d.NaturezaDespesaID);

        }
    }
}
