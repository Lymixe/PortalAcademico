using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalAcademico.Models
{
    public class Curso
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int Creditos { get; set; }
        public int CupoMaximo { get; set; }
        public TimeOnly HorarioInicio { get; set; }
        public TimeOnly HorarioFin { get; set; }
        public bool Activo { get; set; }
    }
}