using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult>
            PaginarRegistroVistaDepartamento(int? posicion)
        {
            if(posicion == null)
            {
                //ponemos la posicion en el primer registro
                posicion = 1;
            }
            else
            {

            }
            int numeroRegistros = await
                this.repo.GetNumeroRegistrosVistaDepartamentos();

            return View();
        }
    }
}
