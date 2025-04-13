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

        // GET: ConsultaMedica
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

        // GET: ConsultaMedica/Details/5
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

        // GET: ConsultaMedica/Create
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

        // POST: ConsultaMedica/Create
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

        // GET: ConsultaMedica/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultaMedica = await (from c in _context.Consultas where c.Id == id select c).FirstOrDefaultAsync();
            if (consultaMedica == null)
            {
                return NotFound();
            }

            var pacientes = (from p in _context.Pacientes orderby p.Nome select p).ToList();
            var medicos = (from m in _context.Medicos orderby m.Nome select m).ToList();
            var statusOptions = (from s in Enum.GetValues(typeof(StatusConsulta)).Cast<StatusConsulta>()
                                 select s).ToList();

            ViewBag.PacienteList = pacientes;
            ViewBag.MedicoList = medicos;
            ViewBag.StatusList = statusOptions;

            return View(consultaMedica);
        }

        // POST: ConsultaMedica/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConsultaMedica consultaMedica)
        {
            if (id != consultaMedica.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consultaMedica);
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

            var pacientes = (from p in _context.Pacientes orderby p.Nome select p).ToList();
            var medicos = (from m in _context.Medicos orderby m.Nome select m).ToList();
            var statusOptions = (from s in Enum.GetValues(typeof(StatusConsulta)).Cast<StatusConsulta>()
                                 select s).ToList();

            ViewBag.PacienteList = pacientes;
            ViewBag.MedicoList = medicos;
            ViewBag.StatusList = statusOptions;

            return View(consultaMedica);
        }

        // GET: ConsultaMedica/Delete/5
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

        // POST: ConsultaMedica/Delete/5
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

        // GET: ConsultaMedica/Agenda
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
                                                      && c.Status == StatusConsulta.Realizada
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

    // Classes auxiliares para agrupamento de dados
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