using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoMVC.Models;
using TrabalhoMVC.Database;
using System.Collections.Generic;

namespace TrabalhoMVC.Controllers
{
    public class ConsultaMedicaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultaMedicaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var consultas = await (from c in _context.Consultas
                                   join p in _context.Pacientes on c.PacienteId equals p.Id
                                   join m in _context.Medicos on c.MedicoId equals m.Id
                                   orderby c.DataConsulta descending
                                   select c).Include(c => c.Paciente)
                                           .Include(c => c.Medico)
                                           .ToListAsync();

            return View(consultas);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultaMedica = await (from c in _context.Consultas
                                        where c.Id == id
                                        select c).Include(c => c.Paciente)
                                                .Include(c => c.Medico)
                                                .FirstOrDefaultAsync();

            if (consultaMedica == null)
            {
                return NotFound();
            }

            return View(consultaMedica);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ConsultaMedica
            {
                DataConsulta = DateTime.Now.AddDays(1).Date.AddHours(9),
                Status = StatusConsulta.Agendada
            };

            var pacientes = (from p in _context.Pacientes orderby p.Nome select p).ToList();
            var medicos = (from m in _context.Medicos where m.Ativo orderby m.Nome select m).ToList();
            var statusOptions = (from s in Enum.GetValues(typeof(StatusConsulta)).Cast<StatusConsulta>()
                                 where s == StatusConsulta.Agendada || s == StatusConsulta.Confirmada
                                 select s).ToList();

            ViewBag.PacienteList = pacientes;
            ViewBag.MedicoList = medicos;
            ViewBag.StatusList = statusOptions;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsultaMedica consultaMedica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consultaMedica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var pacientes = (from p in _context.Pacientes orderby p.Nome select p).ToList();
            var medicos = (from m in _context.Medicos where m.Ativo orderby m.Nome select m).ToList();
            var statusOptions = (from s in Enum.GetValues(typeof(StatusConsulta)).Cast<StatusConsulta>()
                                 where s == StatusConsulta.Agendada || s == StatusConsulta.Confirmada
                                 select s).ToList();

            ViewBag.PacienteList = pacientes;
            ViewBag.MedicoList = medicos;
            ViewBag.StatusList = statusOptions;

            return View(consultaMedica);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultaMedica = await _context.Consultas
                .Include(c => c.Sintomas)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (consultaMedica == null)
            {
                return NotFound();
            }

            var pacientes = _context.Pacientes.OrderBy(p => p.Nome).ToList();
            var medicos = _context.Medicos.OrderBy(m => m.Nome).ToList();
            var statusOptions = Enum.GetValues(typeof(StatusConsulta)).Cast<StatusConsulta>().ToList();
            var todosSintomas = await _context.Sintomas.OrderBy(s => s.Nome).ToListAsync();

            ViewBag.PacienteList = pacientes;
            ViewBag.MedicoList = medicos;
            ViewBag.StatusList = statusOptions;
            ViewBag.TodosSintomas = todosSintomas;

            return View(consultaMedica);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConsultaMedica consultaMedica, List<int> sintomasIds)
        {
            if (id != consultaMedica.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar a consulta com sintomas
                    var consultaExistente = await _context.Consultas
                        .Include(c => c.Sintomas)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (consultaExistente == null)
                    {
                        return NotFound();
                    }

                    // Atualizar propriedades básicas
                    consultaExistente.PacienteId = consultaMedica.PacienteId;
                    consultaExistente.MedicoId = consultaMedica.MedicoId;
                    consultaExistente.DataConsulta = consultaMedica.DataConsulta;
                    consultaExistente.Status = consultaMedica.Status;
                    consultaExistente.Valor = consultaMedica.Valor;
                    consultaExistente.Observacoes = consultaMedica.Observacoes;

                    // Limpar sintomas existentes
                    consultaExistente.Sintomas.Clear();

                    // Adicionar novos sintomas
                    if (sintomasIds != null && sintomasIds.Any())
                    {
                        var sintomas = await _context.Sintomas
                            .Where(s => sintomasIds.Contains(s.Id))
                            .ToListAsync();
                        
                        foreach (var sintoma in sintomas)
                        {
                            consultaExistente.Sintomas.Add(sintoma);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultaMedicaExists(consultaMedica.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Recarregar dados se houver erro
            var pacientes = _context.Pacientes.OrderBy(p => p.Nome).ToList();
            var medicos = _context.Medicos.OrderBy(m => m.Nome).ToList();
            var statusOptions = Enum.GetValues(typeof(StatusConsulta)).Cast<StatusConsulta>().ToList();
            var todosSintomas = await _context.Sintomas.OrderBy(s => s.Nome).ToListAsync();

            ViewBag.PacienteList = pacientes;
            ViewBag.MedicoList = medicos;
            ViewBag.StatusList = statusOptions;
            ViewBag.TodosSintomas = todosSintomas;

            return View(consultaMedica);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultaMedica = await (from c in _context.Consultas
                                        where c.Id == id
                                        select c).Include(c => c.Paciente)
                                                .Include(c => c.Medico)
                                                .FirstOrDefaultAsync();

            if (consultaMedica == null)
            {
                return NotFound();
            }

            return View(consultaMedica);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consultaMedica = await (from c in _context.Consultas where c.Id == id select c).FirstOrDefaultAsync();
            if (consultaMedica != null)
            {
                _context.Consultas.Remove(consultaMedica);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Agenda(DateTime? data, int? minimoConsultas)
        {
            data = data ?? DateTime.Today;
            minimoConsultas = minimoConsultas ?? 1;

            var consultas = await (from c in _context.Consultas
                                   where c.DataConsulta.Date == data.Value.Date
                                   orderby c.DataConsulta
                                   select c).Include(c => c.Paciente)
                                           .Include(c => c.Medico)
                                           .ToListAsync();

            var estatisticasPorMedico = await (from c in _context.Consultas
                                               join m in _context.Medicos on c.MedicoId equals m.Id
                                               where c.DataConsulta.Month == data.Value.Month
                                                  && c.DataConsulta.Year == data.Value.Year
                                               group c by new { m.Id, m.Nome, m.Especialidade } into grp
                                               select new MedicoEstatisticas
                                               {
                                                   MedicoId = grp.Key.Id,
                                                   MedicoNome = grp.Key.Nome,
                                                   Especialidade = grp.Key.Especialidade,
                                                   TotalConsultas = grp.Count(),
                                                   ValorTotal = grp.Sum(c => c.Valor),
                                                   ValorMedio = grp.Average(c => c.Valor)
                                               }).ToListAsync();

            var medicosComMuitasConsultas = await (from c in _context.Consultas
                                                   join m in _context.Medicos on c.MedicoId equals m.Id
                                                   where c.DataConsulta.Month == data.Value.Month
                                                      && c.DataConsulta.Year == data.Value.Year
                                                      && c.Status == StatusConsulta.Realizada || c.Status == StatusConsulta.Cancelada
                                                   group c by new { m.Id, m.Nome, m.Especialidade } into grp
                                                   where grp.Count() >= minimoConsultas
                                                   orderby grp.Count() descending
                                                   select new MedicoComMuitasConsultas
                                                   {
                                                       MedicoId = grp.Key.Id,
                                                       MedicoNome = grp.Key.Nome,
                                                       Especialidade = grp.Key.Especialidade,
                                                       TotalConsultas = grp.Count(),
                                                       ConsultasRealizadas = grp.Count(c => c.Status == StatusConsulta.Realizada),
                                                       ConsultasCanceladas = grp.Count(c => c.Status == StatusConsulta.Cancelada),
                                                       PorcentagemCanceladas = grp.Count(c => c.Status == StatusConsulta.Cancelada) * 100.0m / grp.Count()
                                                   }).ToListAsync();

            ViewBag.Data = data;
            ViewBag.MinimoConsultas = minimoConsultas;
            ViewBag.EstatisticasPorMedico = estatisticasPorMedico;
            ViewBag.MedicosComMuitasConsultas = medicosComMuitasConsultas;

            return View(consultas);
        }

        private bool ConsultaMedicaExists(int id)
        {
            return (from c in _context.Consultas where c.Id == id select c).Any();
        }
    }

    public class MedicoEstatisticas
    {
        public int MedicoId { get; set; }
        public string MedicoNome { get; set; }
        public string Especialidade { get; set; }
        public int TotalConsultas { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorMedio { get; set; }
    }

    public class MedicoComMuitasConsultas
    {
        public int MedicoId { get; set; }
        public string MedicoNome { get; set; }
        public string Especialidade { get; set; }
        public int TotalConsultas { get; set; }
        public int ConsultasRealizadas { get; set; }
        public int ConsultasCanceladas { get; set; }
        public decimal PorcentagemCanceladas { get; set; }
    }
}
