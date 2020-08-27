using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace InstituicaoEnsino.Models
{
    public partial class InstituicaoEnsinoDBContext : DbContext
    {
        public InstituicaoEnsinoDBContext()
        {
        }

        public InstituicaoEnsinoDBContext(DbContextOptions<InstituicaoEnsinoDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Aluno> Aluno { get; set; }
        public virtual DbSet<Professor> Professor { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("appsettings.json")
                   .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aluno>(entity =>
            {
                entity.HasKey(e => e.IdAluno)
                    .HasName("PK__Aluno__06D5E476E0D93A77");

                entity.Property(e => e.IdAluno).HasColumnName("ID_Aluno");

                entity.Property(e => e.DataVencimento).HasColumnType("datetime");

                entity.Property(e => e.IdProfessor).HasColumnName("ID_Professor");

                entity.Property(e => e.Mensalidade).HasColumnType("money");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(254);

                entity.HasOne(d => d.IdProfessorNavigation)
                    .WithMany(p => p.Aluno)
                    .HasForeignKey(d => d.IdProfessor)
                    .HasConstraintName("FK__Aluno__ID_Profes__38996AB5");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.IdProfessor)
                    .HasName("PK__Professo__3CFEF7CA8831359F");

                entity.Property(e => e.IdProfessor).HasColumnName("ID_Professor");

                entity.Property(e => e.DataUltimaImportacao).HasColumnType("datetime");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(254);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
