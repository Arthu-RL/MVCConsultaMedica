using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoMVC.Database;
using TrabalhoMVC.Models;

namespace TrabalhoMVC.Controllers
{
    public class MedicoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Medico
        public async Task<IActionResult> Index()
        {
            return View(await _context.Medicos.ToListAsync());
        }

        // GET: Medico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos
                .Include(m => m.Consultas)
                .ThenInclude(c => c.Paciente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null)
            {
                return NotFound();
            }

            return View(medico);
        }

        // GET: Medico/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medico/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,CPF,Telefone,CRM,Especialidade,Ativo")] Medico medico)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                return View(medico);
            }

            _context.Add(medico);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Medico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null)
            {
                return NotFound();
            }
            return View(medico);
        }

        // POST: Medico/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,CPF,Telefone,CRM,Especialidade,Ativo")] Medico medico)
        {
            if (id != medico.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                return View(medico);
            }

            try
            {
                _context.Update(medico);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicoExists(medico.Id))
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

        // GET: Medico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medico == null)
            {
                return NotFound();
            }

            return View(medico);
        }

        // POST: Medico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);
            if (medico != null)
            {
                var consultas = await _context.Consultas.Where(m => m.MedicoId == medico.Id).FirstOrDefaultAsync();

                if (consultas != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                _context.Medicos.Remove(medico);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Medico/AgendaMedico/5
        public async Task<IActionResult> AgendaMedico(int? id, DateTime? data)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Se não for informada data, considera a data atual
            data = data ?? DateTime.Today;

            var medico = await _context.Medicos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null)
            {
                return NotFound();
            }

            // Consultas do médico na data informada
            var consultas = await _context.Consultas
                .Include(c => c.Paciente)
                .Where(c => c.MedicoId == id && c.DataConsulta.Date == data.Value.Date)
                .OrderBy(c => c.DataConsulta)
                .ToListAsync();

            ViewBag.Medico = medico;
            ViewBag.Data = data;

            return View(consultas);
        }

        private bool MedicoExists(int id)
        {
            return _context.Medicos.Any(m => m.Id == id);
        }
    }
}