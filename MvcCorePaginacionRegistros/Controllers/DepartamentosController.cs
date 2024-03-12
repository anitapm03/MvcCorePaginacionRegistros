using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class DepartamentosController : Controller
    {
        private RepositoryHospital repo;

        public DepartamentosController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> ListaDeptos()
        {
            List<Departamento> departamentos = await
                this.repo.GetDepartamentos();

            return View(departamentos);
        }

        public async Task<IActionResult> Detalles(int id, int? posicion)
        {
            if (posicion == null)
            {
                //POSICION PARA EL EMPLEADO
                posicion = 1;
            }

            ModelDeptEmp model = await
                this.repo.GetEmpleadoDepartamentoAsync(posicion.Value, id);

            ViewData["REGISTROS"] = model.NumRegistros;
            ViewData["DEPARTAMENTO"] = id;

            Departamento dept = await
                this.repo.FindDepto(id);

            model.departamento = dept;

            int siguiente = posicion.Value + 1;

            if (siguiente > model.NumRegistros)
            {
                //efecto optico 
                siguiente = model.NumRegistros;
            }
            int anterior = posicion.Value - 1;

            if (anterior < 1)
            {
                anterior = 1;
            }

            ViewData["ULTIMO"] = model.NumRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["POSICION"] = posicion; 

            return View(model);
        }



        public async Task<IActionResult> _EmpleadosPartial()
        {
            List<Empleado> empleados = await this.repo.GetEmpleados();
            return PartialView("_EmpleadosPartial");
        }
    }
}
