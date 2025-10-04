using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Caching.Distributed;


namespace PortalAcademico.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CoordinadorController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Cursos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("ListaCursosActivos");
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                    await _cache.RemoveAsync("ListaCursosActivos");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExists(curso.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
            }
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("ListaCursosActivos");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> VerMatriculas(int cursoId)
        {
            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null) return NotFound();

            ViewBag.CursoNombre = curso.Nombre;

            var matriculas = await _context.Matriculas
                .Include(m => m.Usuario)
                .Where(m => m.CursoId == cursoId)
                .ToListAsync();

            return View(matriculas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarMatricula(int matriculaId)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if (matricula == null) return NotFound();
            
            matricula.Estado = EstadoMatricula.Confirmada;
            _context.Update(matricula);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("VerMatriculas", new { cursoId = matricula.CursoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarMatricula(int matriculaId)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if (matricula == null) return NotFound();
            
            matricula.Estado = EstadoMatricula.Cancelada;
            _context.Update(matricula);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("VerMatriculas", new { cursoId = matricula.CursoId });
        }
        
        private bool CursoExists(int id)
        {
            return _context.Cursos.Any(e => e.Id == id);
        }
    }
}