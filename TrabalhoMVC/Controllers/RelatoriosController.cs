using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoMVC.Database;
using TrabalhoMVC.Models;

namespace TrabalhoMVC.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RelatoriosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Relatorios
        public IActionResult Index()
        {
            return View();
        }

        // Consulta 1: Usando dados das duas classes (relacionamento Paciente e Médico)
        // Lista todas as consultas com dados do paciente e medico
        public async Task<IActionResult> ConsultasDetalhadas()
        {
            // LINQ Query: Join entre Paciente, Medico e ConsultaMedica
            var query = from c in _context.Consultas
                        join p in _context.Pacientes on c.PacienteId equals p.Id
                        join m in _context.Medicos on c.MedicoId equals m.Id
                        select new
                        {
                            ConsultaId = c.Id,
                            DataConsulta = c.DataConsulta,
                            Status = c.Status.ToString(),
                            Valor = c.Valor,
                            PacienteNome = p.Nome,
                            PacienteCPF = p.CPF,
                            MedicoNome = m.Nome,
                            MedicoCRM = m.CRM,
                            Especialidade = m.Especialidade
                        };

            var resultado = await query.ToListAsync();
            return View(resultado);
        }

        // Consulta 2: Consulta usando funcoes grupo
        // Mostra estatisticas de consultas por medico
        public async Task<IActionResult> EstatisticasPorMedico()
        {
            // LINQ Query: Group by com funções de agregação
            var query = from c in _context.Consultas
                        join m in _context.Medicos on c.MedicoId equals m.Id
                        where c.Status == StatusConsulta.Realizada // Considerar apenas consultas realizadas
                        group c by new { m.Id, m.Nome, m.Especialidade } into g
                        select new
                        {
                            MedicoId = g.Key.Id,
                            MedicoNome = g.Key.Nome,
                            Especialidade = g.Key.Especialidade,
                            TotalConsultas = g.Count(),
                            ValorTotal = g.Sum(c => c.Valor),
                            ValorMedio = g.Average(c => c.Valor),
                            MaiorValor = g.Max(c => c.Valor),
                            MenorValor = g.Min(c => c.Valor)
                        };

            var resultado = await query.ToListAsync();
            return View(resultado);
        }

        // Consulta 3: Consulta com filtro principal (where) e filtro do grupo (having)
        // Retorna médicos ativos com pelo menos 2 consultas realizadas e valor médio acima de 150
        public async Task<IActionResult> MedicosDestaque()
        {
            // LINQ Query: Where e Having
            var query = from c in _context.Consultas
                        join m in _context.Medicos on c.MedicoId equals m.Id
                        where m.Ativo == true // Filtro principal: apenas médicos ativos
                        where c.Status == StatusConsulta.Realizada // Filtro principal: apenas consultas realizadas
                        group c by new { m.Id, m.Nome, m.Especialidade } into g
                        where g.Count() >= 1 // Having: pelo menos 1 consulta (ajustado para os novos dados)
                        where g.Average(c => c.Valor) >= 150m // Having: valor médio >= 150
                        select new
                        {
                            MedicoId = g.Key.Id,
                            MedicoNome = g.Key.Nome,
                            Especialidade = g.Key.Especialidade,
                            TotalConsultas = g.Count(),
                            ValorMedio = g.Average(c => c.Valor),
                            ValorTotal = g.Sum(c => c.Valor)
                        };

            var resultado = await query.OrderByDescending(r => r.TotalConsultas).ToListAsync();
            return View(resultado);
        }

        // Método adicional: Consultas por período
        public async Task<IActionResult> ConsultasPorPeriodo(DateTime? dataInicio, DateTime? dataFim)
        {
            // Valores default se não forem informados
            dataInicio = dataInicio ?? DateTime.Now.AddMonths(-1);
            dataFim = dataFim ?? DateTime.Now;

            // LINQ Query com filtro por período
            var query = from c in _context.Consultas
                        join p in _context.Pacientes on c.PacienteId equals p.Id
                        join m in _context.Medicos on c.MedicoId equals m.Id
                        where c.DataConsulta >= dataInicio && c.DataConsulta <= dataFim
                        orderby c.DataConsulta
                        select new
                        {
                            ConsultaId = c.Id,
                            DataConsulta = c.DataConsulta,
                            Status = c.Status.ToString(),
                            PacienteNome = p.Nome,
                            MedicoNome = m.Nome,
                            Especialidade = m.Especialidade,
                            Valor = c.Valor
                        };

            ViewBag.DataInicio = dataInicio;
            ViewBag.DataFim = dataFim;

            var resultado = await query.ToListAsync();
            return View(resultado);
        }
    }
}