using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TrabalhoMVC.Models;

namespace TrabalhoMVC.Database
{
    public static class DatabaseInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Cria o banco de dados se não existir, ignorando migrações
            context.Database.EnsureCreated();

            // Verifica se o banco já tem dados
            if (context.Pacientes.Any() || context.Medicos.Any())
            {
                return;
            }

            var sintomas = new Sintoma[] {
                new Sintoma { Nome = "Dor de cabeça" },
                new Sintoma { Nome = "Febre" },
                new Sintoma { Nome = "Tosse" },
                new Sintoma { Nome = "Dor abdominal" },
                new Sintoma { Nome = "Náusea" },
                new Sintoma { Nome = "Dor no peito" },
                new Sintoma { Nome = "Falta de ar" },
                new Sintoma { Nome = "Tontura" },
                new Sintoma { Nome = "Dor nas costas" },
                new Sintoma { Nome = "Dor no joelho" }
            };

            foreach (Sintoma sintoma in sintomas)
            {
                context.Sintomas.Add(sintoma);
            }
            context.SaveChanges();

            // Dados mockados - Pacientes
            var pacientes = new Paciente[] {
                new Paciente {
                    Nome = "Ana Silva",
                    CPF = "123.456.789-01",
                    DataNascimento = new DateTime(1980, 5, 15),
                    Telefone = "(11) 99999-1111"
                },
                new Paciente {
                    Nome = "Carlos Oliveira",
                    CPF = "234.567.890-12",
                    DataNascimento = new DateTime(1975, 8, 22),
                    Telefone = "(11) 99999-2222"
                },
                new Paciente {
                    Nome = "Mariana Santos",
                    CPF = "345.678.901-23",
                    DataNascimento = new DateTime(1990, 3, 10),
                    Telefone = "(11) 99999-3333"
                },
                new Paciente {
                    Nome = "Paulo Rodrigues",
                    CPF = "456.789.012-34",
                    DataNascimento = new DateTime(1965, 12, 5),
                    Telefone = "(11) 99999-4444"
                },
                new Paciente {
                    Nome = "Juliana Costa",
                    CPF = "567.890.123-45",
                    DataNascimento = new DateTime(1988, 7, 30),
                    Telefone = "(11) 99999-5555"
                }
            };

            foreach (Paciente paciente in pacientes)
            {
                context.Pacientes.Add(paciente);
            }
            context.SaveChanges();

            // Dados mockados - Médicos
            var medicos = new Medico[] {
                new Medico {
                    Nome = "Dr. José Cardoso",
                    CPF = "678.901.234-56",
                    Telefone = "(11) 98888-1111",
                    CRM = "12345-SP",
                    Especialidade = "Cardiologia",
                    Ativo = true
                },
                new Medico {
                    Nome = "Dra. Patrícia Almeida",
                    CPF = "789.012.345-67",
                    Telefone = "(11) 98888-2222",
                    CRM = "23456-SP",
                    Especialidade = "Dermatologia",
                    Ativo = true
                },
                new Medico {
                    Nome = "Dr. Roberto Costa",
                    CPF = "890.123.456-78",
                    Telefone = "(11) 98888-3333",
                    CRM = "34567-SP",
                    Especialidade = "Ortopedia",
                    Ativo = true
                },
                new Medico {
                    Nome = "Dra. Fernanda Lima",
                    CPF = "901.234.567-89",
                    Telefone = "(11) 98888-4444",
                    CRM = "45678-SP",
                    Especialidade = "Pediatria",
                    Ativo = true
                },
                new Medico {
                    Nome = "Dr. Ricardo Santos",
                    CPF = "012.345.678-90",
                    Telefone = "(11) 98888-5555",
                    CRM = "56789-SP",
                    Especialidade = "Neurologia",
                    Ativo = false
                }
            };

            foreach (Medico medico in medicos)
            {
                context.Medicos.Add(medico);
            }
            context.SaveChanges();

            // Dados mockados - Consultas Médicas
            var dataAtual = DateTime.Now;
            var consultas = new ConsultaMedica[] {
                new ConsultaMedica {
                    PacienteId = 1, // Ana Silva
                    MedicoId = 1,   // Dr. José Cardoso
                    DataConsulta = dataAtual.AddDays(-15),
                    Status = StatusConsulta.Realizada,
                    Valor = 150.00m,
                    Observacoes = "Consulta de rotina para avaliação cardíaca"
                },
                new ConsultaMedica {
                    PacienteId = 1, // Ana Silva
                    MedicoId = 2,   // Dra. Patrícia Almeida
                    DataConsulta = dataAtual.AddDays(-7),
                    Status = StatusConsulta.Realizada,
                    Valor = 200.00m,
                    Observacoes = "Avaliação de manchas na pele"
                },
                new ConsultaMedica {
                    PacienteId = 2, // Carlos Oliveira
                    MedicoId = 1,   // Dr. José Cardoso
                    DataConsulta = dataAtual.AddDays(-10),
                    Status = StatusConsulta.Realizada,
                    Valor = 150.00m,
                    Observacoes = "Check-up cardiovascular anual"
                },
                new ConsultaMedica {
                    PacienteId = 3, // Mariana Santos
                    MedicoId = 3,   // Dr. Roberto Costa
                    DataConsulta = dataAtual.AddDays(-5),
                    Status = StatusConsulta.Realizada,
                    Valor = 180.00m,
                    Observacoes = "Consulta para tratamento de dor no joelho"
                },
                new ConsultaMedica {
                    PacienteId = 4, // Paulo Rodrigues
                    MedicoId = 1,   // Dr. José Cardoso
                    DataConsulta = dataAtual.AddDays(5),
                    Status = StatusConsulta.Agendada,
                    Valor = 150.00m,
                    Observacoes = "Acompanhamento de pressão arterial"
                },
                new ConsultaMedica {
                    PacienteId = 2, // Carlos Oliveira
                    MedicoId = 2,   // Dra. Patrícia Almeida
                    DataConsulta = dataAtual.AddDays(3),
                    Status = StatusConsulta.Confirmada,
                    Valor = 200.00m,
                    Observacoes = "Tratamento de acne"
                },
                new ConsultaMedica {
                    PacienteId = 3, // Mariana Santos
                    MedicoId = 1,   // Dr. José Cardoso
                    DataConsulta = dataAtual.AddDays(-2),
                    Status = StatusConsulta.Cancelada,
                    Valor = 0.00m,
                    Observacoes = "Consulta cancelada - paciente não compareceu"
                },
                new ConsultaMedica {
                    PacienteId = 5, // Juliana Costa
                    MedicoId = 4,   // Dra. Fernanda Lima
                    DataConsulta = dataAtual.AddDays(7),
                    Status = StatusConsulta.Agendada,
                    Valor = 180.00m,
                    Observacoes = "Consulta pediátrica para seu filho"
                },
                new ConsultaMedica {
                    PacienteId = 5, // Juliana Costa
                    MedicoId = 5,   // Dr. Ricardo Santos
                    DataConsulta = dataAtual.AddDays(12),
                    Status = StatusConsulta.Agendada,
                    Valor = 250.00m,
                    Observacoes = "Consulta neurológica para avaliação de enxaquecas"
                },
                new ConsultaMedica {
                    PacienteId = 4, // Paulo Rodrigues
                    MedicoId = 3,   // Dr. Roberto Costa
                    DataConsulta = dataAtual.AddDays(-20),
                    Status = StatusConsulta.Realizada,
                    Valor = 180.00m,
                    Observacoes = "Avaliação de dor nas costas"
                }
            };

            foreach (ConsultaMedica consulta in consultas)
            {
                context.Consultas.Add(consulta);
            }
            context.SaveChanges();

            // Relacionar sintomas com consultas (Relacionamento Muitos para Muitos)
            var consultasSalvas = context.Consultas.ToList();
            var sintomasSalvos = context.Sintomas.ToList();

            // Consulta 1 (Ana Silva - Cardiologia): Dor no peito, Falta de ar
            consultasSalvas[0].Sintomas = new List<Sintoma>
            {
                sintomasSalvos.First(s => s.Nome == "Dor no peito"),
                sintomasSalvos.First(s => s.Nome == "Falta de ar")
            };

            // Consulta 2 (Ana Silva - Dermatologia): sem sintomas específicos relatados
            
            // Consulta 3 (Carlos - Cardiologia): Dor de cabeça, Tontura
            consultasSalvas[2].Sintomas = new List<Sintoma>
            {
                sintomasSalvos.First(s => s.Nome == "Dor de cabeça"),
                sintomasSalvos.First(s => s.Nome == "Tontura")
            };

            // Consulta 4 (Mariana - Ortopedia): Dor no joelho
            consultasSalvas[3].Sintomas = new List<Sintoma>
            {
                sintomasSalvos.First(s => s.Nome == "Dor no joelho")
            };

            // Consulta 9 (Juliana - Neurologia): Dor de cabeça, Náusea
            consultasSalvas[8].Sintomas = new List<Sintoma>
            {
                sintomasSalvos.First(s => s.Nome == "Dor de cabeça"),
                sintomasSalvos.First(s => s.Nome == "Náusea")
            };

            // Consulta 10 (Paulo - Ortopedia): Dor nas costas
            consultasSalvas[9].Sintomas = new List<Sintoma>
            {
                sintomasSalvos.First(s => s.Nome == "Dor nas costas")
            };

            context.SaveChanges();
        }
    }
}