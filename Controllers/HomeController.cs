using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalAcademico.Models;
using System.Diagnostics;

namespace PortalAcademico.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // --- MÉTODO TEMPORAL PARA RESETEAR LA CONTRASEÑA ---
        public async Task<IActionResult> ResetCoordinatorPassword()
        {
            var user = await _userManager.FindByEmailAsync("coordinador@test.com");
            if (user == null)
            {
                return Content("Error: El usuario coordinador@test.com no fue encontrado en la base de datos.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, "Coordinador123!");

            if (result.Succeeded)
            {
                return Content("¡Éxito! La contraseña para 'coordinador@test.com' ha sido reseteada a 'Coordinador123!'. Ya puedes intentar iniciar sesión.");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Content($"Error al resetear la contraseña: {errors}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}