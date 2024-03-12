using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryHospital repo;

        public EmpleadosController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> EmpleadosDepartamento(int idempleado)
        {
            List<Empleado> empleados =
               await this.repo.GetEmpleadosDepartamentoAsync(idempleado);
            return View(empleados);

        }
    }
}
