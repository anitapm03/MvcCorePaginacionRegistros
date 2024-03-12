using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
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

        public async Task<IActionResult> EmpleadosOficioOut
            (int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                ModelPaginacionEmpleado model = await
                    this.repo.GetGrupoEmpleadosOficioOutAsync(posicion.Value, oficio);
                ViewData["REGISTROS"] = model.NumRegistros;
                ViewData["OFICIO"] = oficio;
                return View(model.Empleados);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficioOut
            (string oficio)
        {
            ModelPaginacionEmpleado model = await
                this.repo.GetGrupoEmpleadosOficioOutAsync(1, oficio);
            ViewData["REGISTROS"] = model.NumRegistros;
            ViewData["OFICIO"] = oficio;
            return View(model.Empleados);
        }


        public async Task<IActionResult> EmpleadosOficio
            (int? posicion, string oficio)
        {
            if(posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                List<Empleado> empleados = await
                     this.repo.GetGrupoEmpOficio(posicion.Value, oficio);
                int numeroRegistros = await
                this.repo.GetNumEmpOficio(oficio);

                ViewData["REGISTROS"] = numeroRegistros;
                ViewData["OFICIO"] = oficio;

                return View(empleados);
            }
        }
        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio
            (string oficio)
        {
            //cuando buscamos en que posicion empieza todo?
            List<Empleado> empleados = await
                this.repo.GetGrupoEmpOficio(1, oficio);
            int numeroRegistros = await
                this.repo.GetNumEmpOficio(oficio);

            ViewData["REGISTROS"] = numeroRegistros;
            ViewData["OFICIO"] = oficio;

            return View(empleados);
        }


        public async Task<IActionResult>
           PaginarGrupoEmp(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int numeroRegistros = await
                this.repo.GetNumeroRegistrosVistaEmpleados();

            ViewData["REGISTROS"] = numeroRegistros;

            List<Empleado> empleados = await
                this.repo.GetGrupoEmp(posicion.Value);

            return View(empleados);
        }

        public async Task<IActionResult>
           PaginarGrupoDept(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int numeroRegistros = await
                this.repo.GetNumeroRegistrosVistaDepartamentos();

            ViewData["REGISTROS"] = numeroRegistros;

            List<Departamento> departamentos = await
                this.repo.GetGrupoDept(posicion.Value);

            return View(departamentos);
        }


        public async Task<IActionResult>
            PaginarGrupo(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }
            
            int numeroRegistros = await
                this.repo.GetNumeroRegistrosVistaDepartamentos();

            ViewData["REGISTROS"] = numeroRegistros;

            List<VistaDepartamento> departamentos = await
                this.repo.GetGrupoVistaDepartamentoAsync(posicion.Value);

            return View(departamentos);
        }

        public async Task<IActionResult>
            PaginarRegistroVistaDepartamento(int? posicion)
        {
            if(posicion == null)
            {
                //ponemos la posicion en el primer registro
                posicion = 1;
            }
            
            int numeroRegistros = await
                this.repo.GetNumeroRegistrosVistaDepartamentos();
            int siguiente = posicion.Value + 1;

            if(siguiente > numeroRegistros)
            {
                //efecto optico 
                siguiente = numeroRegistros;
            }
            int anterior = posicion.Value - 1;

            if (anterior < 1)
            {
                anterior = 1;
            }

            VistaDepartamento vista = await 
                this.repo.GetVistaDepartamentoAsync(posicion.Value);

            ViewData["ULTIMO"] = numeroRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior; ;

            return View(vista);
        }

        public async Task<IActionResult>
            PaginarRegistroVistaEmpleado(int? posicion)
        {
            if (posicion == null)
            {
                //ponemos la posicion en el primer registro
                posicion = 1;
            }

            int numeroRegistros = await
                this.repo.GetNumeroRegistrosVistaEmpleados();
            int siguiente = posicion.Value + 1;

            if (siguiente > numeroRegistros)
            {
                //efecto optico 
                siguiente = numeroRegistros;
            }
            int anterior = posicion.Value - 1;

            if (anterior < 1)
            {
                anterior = 1;
            }

            VistaEmpleado vista = await
                this.repo.GetVistaEmpleado(posicion.Value);

            ViewData["ULTIMO"] = numeroRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior; ;

            return View(vista);
        }
    }
}
