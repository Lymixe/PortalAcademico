using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? minCreditos, int? maxCreditos, TimeOnly? horarioInicio, TimeOnly? horarioFin)
        {
            if (minCreditos.HasValue && minCreditos < 0)
            {
                ModelState.AddModelError("", "Los créditos mínimos no pueden ser negativos.");
            }
            if (horarioInicio.HasValue && horarioFin.HasValue && horarioFin < horarioInicio)
            {
                ModelState.AddModelError("", "El horario de fin no puede ser anterior al de inicio.");
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["MinCreditos"] = minCreditos;
            ViewData["MaxCreditos"] = maxCreditos;
            ViewData["HorarioInicio"] = horarioInicio;
            ViewData["HorarioFin"] = horarioFin;

            var cursos = from c in _context.Cursos
                         where c.Activo
                         select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                cursos = cursos.Where(s => s.Nombre.Contains(searchString));
            }

            if (minCreditos.HasValue)
            {
                cursos = cursos.Where(c => c.Creditos >= minCreditos);
            }

            if (maxCreditos.HasValue)
            {
                cursos = cursos.Where(c => c.Creditos <= maxCreditos);
            }

            if (horarioInicio.HasValue)
            {
                cursos = cursos.Where(c => c.HorarioInicio >= horarioInicio);
            }

            if (horarioFin.HasValue)
            {
                cursos = cursos.Where(c => c.HorarioFin <= horarioFin);
            }

            return View(await cursos.AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }
    }
}