using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Accounts.DAO.Model
{
    public partial class AccountsControlContext : DbContext
    {
        public AccountsControlContext()
        {
        }

        public AccountsControlContext(DbContextOptions<AccountsControlContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cliente> Clientes { get; set; } = null!;
        public virtual DbSet<Cuenta> Cuentas { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Data Source=NB120\\SQLEXPRESS;Initial Catalog=DB_Accounts;Integrated Security=True;TrustServerCertificate=True");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Apellido).IsUnicode(false);

                entity.Property(e => e.Contraseña).HasMaxLength(50);

                entity.Property(e => e.Dni)
                    .HasMaxLength(10)
                    .HasColumnName("DNI")
                    .IsFixedLength();

                entity.Property(e => e.Nombre).IsUnicode(false);

                entity.Property(e => e.Usuario).HasMaxLength(50);
            });

            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FechaActualizacion).HasColumnType("datetime");

                entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

                entity.Property(e => e.NumeroCuenta).HasMaxLength(50);

                entity.Property(e => e.Saldo).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Cuenta)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK_Cuentas_Clientes");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
