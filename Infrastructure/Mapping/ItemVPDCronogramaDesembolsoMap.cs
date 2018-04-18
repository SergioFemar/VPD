using Application.Entities;
using System.Data.Entity.ModelConfiguration;


namespace Application.Mapping
{
    public class ItemVPDCronogramaDesembolsoMap : EntityTypeConfiguration<ItemVPDCronogramaDesembolso>
    {
        public ItemVPDCronogramaDesembolsoMap()
        {
            // Primary Key
            this.HasKey(t => t.ItemVPDCronogramaDesembolsoID);

            // Properties
            this.Property(t => t.Devolucao)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Valor)
                .HasPrecision(15, 2);

            // Table & Column Mappings
            this.ToTable("ItemVPDCronogramaDesembolso", "EXEFIN");
            this.Property(t => t.ItemVPDCronogramaDesembolsoID).HasColumnName("ItemVPDCronogramaDesembolsoID");
            this.Property(t => t.CronogramaDesembolsoID).HasColumnName("CronogramaDesembolsoID");
            this.Property(t => t.VariacaoPatrimonialDiminutivaItemID).HasColumnName("VariacaoPatrimonialDiminutivaItemID");
            this.Property(t => t.Valor).HasColumnName("Valor");
            this.Property(t => t.Devolucao).HasColumnName("Devolucao");

            // Relationships
            //this.HasRequired(t => t.CronogramaDesembolso)
            //    .WithMany(t => t.ItemVPDCronogramaDesembolso)
            //    .HasForeignKey(d => d.CronogramaDesembolsoID)
            //    .WillCascadeOnDelete(false);

            //this.HasRequired(t => t.ItemVariacaoPatrimonialDiminutiva)
            //    .WithMany(t => t.ItemVPDCronogramaDesembolso)
            //    .HasForeignKey(d => d.VariacaoPatrimonialDiminutivaItemID)
            //    .WillCascadeOnDelete(false);

        }
    }
}
