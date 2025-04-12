using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabalhoMVC.Models;
using TrabalhoMVC.Database;

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
            var consultas = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .OrderByDescending(c => c.DataConsulta)
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

            var consultaMedica = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consultaMedica == null)
            {
                return NotFound();
            }

            return View(consultaMedica);
        }

        // GET: ConsultaMedica/Create
        public IActionResult Create()
        {
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "Nome");
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Where(m => m.Ativo), "Id", "Nome");

            // Status possíveis para uma nova consulta - usando Enum
            ViewData["StatusOptions"] = new SelectList(Enum.GetValues(typeof(StatusConsulta))
                .Cast<StatusConsulta>()
                .Where(s => s == StatusConsulta.Agendada || s == StatusConsulta.Confirmada)
                .Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name");

            // Data padrão = data atual + 1 dia
            var consulta = new ConsultaMedica
            {
                DataConsulta = DateTime.Now.AddDays(1).Date.AddHours(9), // 9:00 AM do dia seguinte
                Status = StatusConsulta.Agendada // Status padrão
            };

            return View(consulta);
        }

        // POST: ConsultaMedica/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PacienteId,MedicoId,DataConsulta,Status,Valor,Observacoes")] ConsultaMedica consultaMedica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consultaMedica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "Nome", consultaMedica.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Where(m => m.Ativo), "Id", "Nome", consultaMedica.MedicoId);
            ViewData["StatusOptions"] = new SelectList(Enum.GetValues(typeof(StatusConsulta))
                .Cast<StatusConsulta>()
                .Where(s => s == StatusConsulta.Agendada || s == StatusConsulta.Confirmada)
                .Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", consultaMedica.Status);

            return View(consultaMedica);
        }

        // GET: ConsultaMedica/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultaMedica = await _context.Consultas.FindAsync(id);
            if (consultaMedica == null)
            {
                return NotFound();
            }

            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "Nome", consultaMedica.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "Id", "Nome", consultaMedica.MedicoId);

            // Status possíveis para uma consulta em edição - usando Enum
            ViewData["StatusOptions"] = new SelectList(Enum.GetValues(typeof(StatusConsulta))
                .Cast<StatusConsulta>()
                .Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", consultaMedica.Status);

            return View(consultaMedica);
        }

        // POST: ConsultaMedica/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PacienteId,MedicoId,DataConsulta,Status,Valor,Observacoes")] ConsultaMedica consultaMedica)
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

            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "Nome", consultaMedica.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "Id", "Nome", consultaMedica.MedicoId);
            List<string> statusList = new List<string> { "Agendada", "Confirmada", "Realizada", "Cancelada" };
            ViewData["StatusOptions"] = new SelectList(statusList, consultaMedica.Status);

            return View(consultaMedica);
        }

        // GET: ConsultaMedica/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultaMedica = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.Id == id);

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
            var consultaMedica = await _context.Consultas.FindAsync(id);
            if (consultaMedica != null)
            {
                _context.Consultas.Remove(consultaMedica);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ConsultaMedica/Agenda
        public async Task<IActionResult> Agenda(DateTime? data)
        {
            // Se não for informada data, considera a data atual
            data = data ?? DateTime.Today;

            var consultas = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Where(c => c.DataConsulta.Date == data.Value.Date)
                .OrderBy(c => c.DataConsulta)
                .ToListAsync();

            ViewBag.Data = data;
            return View(consultas);
        }

        private bool ConsultaMedicaExists(int id)
        {
            return _context.Consultas.Any(c => c.Id == id);
        }
    }
}