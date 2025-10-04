using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models
{
    public enum EstadoMatricula
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public class Matricula
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public string UsuarioId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public EstadoMatricula Estado { get; set; }

        public Curso Curso { get; set; }
        public IdentityUser Usuario { get; set; }
    }
}