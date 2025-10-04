using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    [Authorize]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Inscribir(int cursoId)
        {
            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null || !curso.Activo)
            {
                TempData["ErrorMessage"] = "El curso seleccionado no existe o no está activo.";
                return RedirectToAction("Index", "Cursos");
            }

            var userId = _userManager.GetUserId(User);

            var yaInscrito = await _context.Matriculas
                .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == userId && m.Estado != EstadoMatricula.Cancelada);
            if (yaInscrito)
            {
                TempData["ErrorMessage"] = "Ya te encuentras matriculado en este curso.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            var matriculasActuales = await _context.Matriculas
                .CountAsync(m => m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);
            if (matriculasActuales >= curso.CupoMaximo)
            {
                TempData["ErrorMessage"] = "El curso ha alcanzado su cupo máximo.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            var cursosUsuario = await _context.Matriculas
                .Where(m => m.UsuarioId == userId && m.Estado == EstadoMatricula.Confirmada)
                .Select(m => m.Curso)
                .ToListAsync();

            foreach (var cursoInscrito in cursosUsuario)
            {
                bool haySolapamiento = curso.HorarioInicio < cursoInscrito.HorarioFin && curso.HorarioFin > cursoInscrito.HorarioInicio;
                if (haySolapamiento)
                {
                    TempData["ErrorMessage"] = $"El horario de este curso se solapa con el de '{cursoInscrito.Nombre}'.";
                    return RedirectToAction("Details", "Cursos", new { id = cursoId });
                }
            }
            
            var matricula = new Matricula
            {
                CursoId = cursoId,
                UsuarioId = userId,
                FechaRegistro = DateTime.UtcNow,
                Estado = EstadoMatricula.Pendiente
            };

            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Inscripción a '{curso.Nombre}' realizada con éxito. Tu solicitud está pendiente de confirmación.";
            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }
    }
}