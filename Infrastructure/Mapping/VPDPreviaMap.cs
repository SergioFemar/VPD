using Application.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;


namespace Application.Mapping
{
    public class VPDPreviadMap : EntityTypeConfiguration<VPDPrevia>
    {
        public VPDPreviadMap()
        {
            // Primary Key
            this.HasKey(t => new { t.DocumentoID, t.VariacaoPatrimonialDiminutivaItemID, t.NaturezaDespesaID, t.NaturezaDespesa, t.ValorSolicitado, t.ValorEstorno, t.TotalEstorno, t.TotalEmpenhado, t.TotalDevolvido });

            // Properties
            this.Property(t => t.DocumentoID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.VariacaoPatrimonialDiminutivaItemID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.NaturezaDespesaID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.FonteRecurso)
                .HasMaxLength(1015);

            this.Property(t => t.NaturezaDespesa)
                .IsRequired()
                .HasMaxLength(1511);

            this.Property(t => t.ValorSolicitado)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ValorEstorno)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TotalEstorno)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TotalEmpenhado)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TotalDevolvido)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("VPDPreviaSubGrid", "EXEFIN");
            this.Property(t => t.DocumentoID).HasColumnName("DocumentoID");
            this.Property(t => t.DocumentoOriginalID).HasColumnName("DocumentoOriginalID");
            this.Property(t => t.VariacaoPatrimonialDiminutivaItemID).HasColumnName("VariacaoPatrimonialDiminutivaItemID");
            this.Property(t => t.NaturezaDespesaID).HasColumnName("NaturezaDespesaID");
            this.Property(t => t.FonteRecurso).HasColumnName("FonteRecurso");
            this.Property(t => t.NaturezaDespesa).HasColumnName("NaturezaDespesa");
            this.Property(t => t.ValorSolicitado).HasColumnName("ValorSolicitado");
            this.Property(t => t.ValorEstorno).HasColumnName("ValorEstorno");
            this.Property(t => t.TotalEstorno).HasColumnName("TotalEstorno");
            this.Property(t => t.TotalEmpenhado).HasColumnName("TotalEmpenhado");
            this.Property(t => t.TotalDevolvido).HasColumnName("TotalDevolvido");
        }
    }
}
