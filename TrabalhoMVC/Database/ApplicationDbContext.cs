using Microsoft.EntityFrameworkCore;
using TrabalhoMVC.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TrabalhoMVC.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<ConsultaMedica> Consultas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração da herança - utilizar TPH (Table Per Hierarchy)
            modelBuilder.Entity<Pessoa>()
                .ToTable("Pessoas")
                .HasDiscriminator<string>("TipoPessoa")
                .HasValue<Paciente>("Paciente")
                .HasValue<Medico>("Medico");

            // Configuração dos relacionamentos
            modelBuilder.Entity<ConsultaMedica>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Consultas)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict); // Impede exclusão em cascata

            modelBuilder.Entity<ConsultaMedica>()
                .HasOne(c => c.Medico)
                .WithMany(m => m.Consultas)
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict); // Impede exclusão em cascata

            // Dados iniciais
            modelBuilder.Entity<Paciente>().HasData(
                new Paciente { Id = 1, Nome = "Ana Silva", CPF = "123.456.789-01", DataNascimento = new System.DateTime(1980, 5, 15), Telefone = "(11) 99999-1111" },
                new Paciente { Id = 2, Nome = "Carlos Oliveira", CPF = "234.567.890-12", DataNascimento = new System.DateTime(1975, 8, 22), Telefone = "(11) 99999-2222" },
                new Paciente { Id = 3, Nome = "Mariana Santos", CPF = "345.678.901-23", DataNascimento = new System.DateTime(1990, 3, 10), Telefone = "(11) 99999-3333" },
                new Paciente { Id = 4, Nome = "Paulo Rodrigues", CPF = "456.789.012-34", DataNascimento = new System.DateTime(1965, 12, 5), Telefone = "(11) 99999-4444" }
            );

            modelBuilder.Entity<Medico>().HasData(
                new Medico { Id = 5, Nome = "Dr. José Cardoso", CPF = "567.890.123-45", Telefone = "(11) 98888-1111", CRM = "12345-SP", Especialidade = "Cardiologia", Ativo = true },
                new Medico { Id = 6, Nome = "Dra. Patrícia Almeida", CPF = "678.901.234-56", Telefone = "(11) 98888-2222", CRM = "23456-SP", Especialidade = "Dermatologia", Ativo = true },
                new Medico { Id = 7, Nome = "Dr. Roberto Costa", CPF = "789.012.345-67", Telefone = "(11) 98888-3333", CRM = "34567-SP", Especialidade = "Ortopedia", Ativo = true },
                new Medico { Id = 8, Nome = "Dra. Fernanda Lima", CPF = "890.123.456-78", Telefone = "(11) 98888-4444", CRM = "45678-SP", Especialidade = "Pediatria", Ativo = false }
            );

            modelBuilder.Entity<ConsultaMedica>().HasData(
                new ConsultaMedica { Id = 1, PacienteId = 1, MedicoId = 5, DataConsulta = System.DateTime.Now.AddDays(-15), Status = StatusConsulta.Realizada, Valor = 150.00m, Observacoes = "Consulta de rotina" },
                new ConsultaMedica { Id = 2, PacienteId = 1, MedicoId = 6, DataConsulta = System.DateTime.Now.AddDays(-7), Status = StatusConsulta.Realizada, Valor = 200.00m, Observacoes = "Avaliação de manchas na pele" },
                new ConsultaMedica { Id = 3, PacienteId = 2, MedicoId = 5, DataConsulta = System.DateTime.Now.AddDays(-10), Status = StatusConsulta.Realizada, Valor = 150.00m, Observacoes = "Check-up cardiovascular" },
                new ConsultaMedica { Id = 4, PacienteId = 3, MedicoId = 7, DataConsulta = System.DateTime.Now.AddDays(-5), Status = StatusConsulta.Realizada, Valor = 180.00m, Observacoes = "Dor no joelho" },
                new ConsultaMedica { Id = 5, PacienteId = 4, MedicoId = 5, DataConsulta = System.DateTime.Now.AddDays(5), Status = StatusConsulta.Agendada, Valor = 150.00m, Observacoes = "Acompanhamento" },
                new ConsultaMedica { Id = 6, PacienteId = 2, MedicoId = 6, DataConsulta = System.DateTime.Now.AddDays(3), Status = StatusConsulta.Agendada, Valor = 200.00m, Observacoes = "Tratamento de acne" },
                new ConsultaMedica { Id = 7, PacienteId = 3, MedicoId = 5, DataConsulta = System.DateTime.Now.AddDays(-2), Status = StatusConsulta.Cancelada, Valor = 0.00m, Observacoes = "Paciente não compareceu" }
            );
        }
    }
}