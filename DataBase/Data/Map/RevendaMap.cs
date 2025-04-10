﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modelos.EF.Revenda;

namespace DataBase.Data.Map
{
    public class RevendaMap : IEntityTypeConfiguration<RevendaModel>
    {
        public void Configure(EntityTypeBuilder<RevendaModel> bld)
        {
            bld.HasKey(x => x.Id);

            // Define um relacionamento obrigatório com a Entidade
            bld.HasOne(x => x.Entidade)
                .WithMany()
                .HasForeignKey(x => x.EntidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            bld.Property(x => x.Situacao).IsRequired();
            bld.Property(x => x.DataCriacao).IsRequired();
        }
    }
}
