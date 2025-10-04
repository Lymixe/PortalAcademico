using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CursosController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
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

            List<Curso> cursosList;
            string cacheKey = "ListaCursosActivos";
            var cachedCursos = await _cache.GetAsync(cacheKey);

            if (cachedCursos != null)
            {
                var serializedCursos = Encoding.UTF8.GetString(cachedCursos);
                cursosList = JsonSerializer.Deserialize<List<Curso>>(serializedCursos);
            }
            else
            {
                cursosList = await _context.Cursos.Where(c => c.Activo).AsNoTracking().ToListAsync();
                var serializedCursos = JsonSerializer.Serialize(cursosList);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                };
                await _cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(serializedCursos), cacheOptions);
            }
            
            var cursosQuery = cursosList.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                cursosQuery = cursosQuery.Where(s => s.Nombre.Contains(searchString));
            }

            if (minCreditos.HasValue)
            {
                cursosQuery = cursosQuery.Where(c => c.Creditos >= minCreditos.Value);
            }

            if (maxCreditos.HasValue)
            {
                cursosQuery = cursosQuery.Where(c => c.Creditos <= maxCreditos.Value);
            }

            if (horarioInicio.HasValue)
            {
                cursosQuery = cursosQuery.Where(c => c.HorarioInicio >= horarioInicio.Value);
            }

            if (horarioFin.HasValue)
            {
                cursosQuery = cursosQuery.Where(c => c.HorarioFin <= horarioFin.Value);
            }

            return View(cursosQuery.ToList());
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
            
            HttpContext.Session.SetInt32("LastCourseId", curso.Id);
            HttpContext.Session.SetString("LastCourseName", curso.Nombre);

            return View(curso);
        }
    }
}