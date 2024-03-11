using Humanizer;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
/*CREATE PROCEDURE SP_GRUPO_DEPTOS
(@POSICION INT)
AS
    SELECT DEPT_NO, DNOMBRE, LOC 
	FROM V_DEPARTAMENTOS_INDIVIDUAL
	WHERE POSICION >= @POSICION AND
	POSICION < (@POSICION +2)
GO

 CREATE PROCEDURE SP_GRUPO_EMP
(@POSICION INT)
AS
	SELECT EMP_NO, APELLIDO, OFICIO,
	SALARIO, DEPT_NO
	FROM V_EMP_INDIVIDUAL
	WHERE POSICION >= @POSICION AND
	POSICION < (@POSICION + 3)
GO
 */
namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {

        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>>
            GetGrupoEmp(int posicion)
        {
            string sql = "SP_GRUPO_EMP @POSICION";
            SqlParameter ppos = new SqlParameter("POSICION", posicion);
            var consulta =
                this.context.Empleados.FromSqlRaw(sql, ppos);
            return await consulta.ToListAsync();
        }


        public async Task<List<Departamento>>
            GetGrupoDept(int posicion)
        {
            string sql = "SP_GRUPO_DEPTOS @POSICION";
            SqlParameter ppos = new SqlParameter("POSICION", posicion);
            var consulta =
                this.context.Departamentos.FromSqlRaw(sql, ppos);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetNumeroRegistrosVistaEmpleados()
        {
            return await
                this.context.VistaEmpleados.CountAsync();
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentos()
        {
            return await
                this.context.VistaDepartamentos.CountAsync();
        }

        public async Task<VistaDepartamento>
            GetVistaDepartamentoAsync(int posicion)
        {
            VistaDepartamento vista = await
                this.context.VistaDepartamentos
                .Where(z => z.Posicion == posicion)
                .FirstOrDefaultAsync();
            return vista;
        }


        public async Task<List<VistaDepartamento>>
            GetGrupoVistaDepartamentoAsync(int posicion)
        {
            //SELECT * FROM V_DEPARTAMENTOS_INDIVIDUAL
            //WHERE POSICION >= 1 AMD POSICION < (1 + 2)
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion
                           && datos.Posicion < (posicion +2)
                           select datos;
            return await consulta.ToListAsync();
        }


        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            return await this.context.Departamentos.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosDepartamentoAsync
            (int idDepartamento)
        {
            var empleados = this.context.Empleados
                .Where(x => x.IdDepartamento == idDepartamento);
            if (empleados.Count() == 0)
            {
                return null;
            }
            else
            {
                return await empleados.ToListAsync();
            }
        }

    }
}
