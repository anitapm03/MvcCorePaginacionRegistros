using Humanizer;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Controllers;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

ALTER PROCEDURE SP_GRUPO_EMP_OFICIO
(@POSICION INT, @OFICIO NVARCHAR(50))
AS
SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM (
	SELECT CAST(
	ROW_NUMBER() OVER (ORDER BY APELLIDO) AS INT) AS POSICION
	,EMP_NO, APELLIDO, OFICIO,SALARIO, DEPT_NO
	FROM EMP WHERE OFICIO = @OFICIO) AS QUERY
	WHERE QUERY.POSICION >= @POSICION AND 
	QUERY.POSICION < (@POSICION +3)
GO


CREATE PROCEDURE SP_GRUPO_EMP_OFICIO_OUT
(@POSICION INT, 
@OFICIO NVARCHAR(50),
@REGISTROS INT OUT)
AS
	SELECT @REGISTROS = COUNT(EMP_NO) FROM EMP
	WHERE OFICIO = @OFICIO
	SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM (
		SELECT CAST(
		ROW_NUMBER() OVER (ORDER BY APELLIDO) AS INT) AS POSICION
		,EMP_NO, APELLIDO, OFICIO,SALARIO, DEPT_NO
		FROM EMP WHERE OFICIO = @OFICIO) AS QUERY
		WHERE QUERY.POSICION >= @POSICION AND 
		QUERY.POSICION < (@POSICION +2)
GO


CREATE PROCEDURE SP_GRUPO_EMP_DEPT
(@POSICION INT, 
@DEPT INT,
@REGISTROS INT OUT)
AS
	SELECT @REGISTROS = COUNT(EMP_NO) FROM EMP
	WHERE DEPT_NO = @DEPT
	SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM (
		SELECT CAST(
		ROW_NUMBER() OVER (ORDER BY APELLIDO) AS INT) AS POSICION
		,EMP_NO, APELLIDO, OFICIO,SALARIO, DEPT_NO
		FROM EMP WHERE  DEPT_NO = @DEPT) AS QUERY
		WHERE QUERY.POSICION >= @POSICION AND 
		QUERY.POSICION < (@POSICION +2)
GO

create procedure SP_REGISTRO_EMPLEADO_DEPARTAMENTO
(@posicion int, @departamento int
, @registros int out)
as
select @registros = count(EMP_NO) from EMP
where DEPT_NO=@departamento
select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from 
    (select cast(
    ROW_NUMBER() OVER (ORDER BY APELLIDO) as int) AS POSICION
    , EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
    from EMP
    where DEPT_NO=@departamento) as QUERY
    where QUERY.POSICION = @posicion
go
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

        //METODO QUE DEVIELVE TBN EL NUMERO DE REGISTROS
        //el controller nos va a dar una posicion y un oficio
        //debemos devolver los empleados y el numero de registrosd 
        public async Task<ModelPaginacionEmpleado> GetGrupoEmpleadosOficioOutAsync
            (int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMP_OFICIO_OUT @POSICION, @OFICIO, "
                + " @REGISTROS out";
            SqlParameter pamPosicion = new SqlParameter("@POSICION", posicion);
            SqlParameter pamOficio = new SqlParameter("@OFICIO", oficio);
            SqlParameter pamRegistros = new SqlParameter("@REGISTROS", -1);
            pamRegistros.Direction = ParameterDirection.Output;
            var consulta =
                this.context.Empleados.FromSqlRaw
                (sql, pamPosicion, pamOficio, pamRegistros);
            //PRIMERO DEBEMOS EJECUTAR LA CONSULTA PARA PODER RECUPERAR 
            //LOS PARAMETROS DE SALIDA
            List<Empleado> empleados = await consulta.ToListAsync();
            int registros = (int)pamRegistros.Value;
            return new ModelPaginacionEmpleado
            {
                NumRegistros = registros,
                Empleados = empleados
            };
        }


        public async Task<ModelDeptEmp>
            GetEmpleadoDepartamentoAsync
            (int posicion, int iddepartamento)
        {
            string sql = "SP_REGISTRO_EMPLEADO_DEPARTAMENTO @posicion, @departamento, "
                + " @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamDepartamento =
                new SqlParameter("@departamento", iddepartamento);
            SqlParameter pamRegistros = new SqlParameter("@registros", -1);
            pamRegistros.Direction = ParameterDirection.Output;
            var consulta =
                this.context.Empleados.FromSqlRaw
                (sql, pamPosicion, pamDepartamento, pamRegistros);
            //PRIMERO DEBEMOS EJECUTAR LA CONSULTA PARA PODER RECUPERAR 
            //LOS PARAMETROS DE SALIDA
            var datos = await consulta.ToListAsync();
            Empleado empleado = datos.FirstOrDefault();

            int registros = (int)pamRegistros.Value;
            return new ModelDeptEmp
            {
                NumRegistros = registros,
                Empleado = empleado
            };
        }


        //METODO PARA SABER EL NUMERO DE EMPLEADOS POR OFICIO
        public async Task<int> GetNumEmpOficio(string oficio)
        {
            return await
                this.context.Empleados.
                Where(z => z.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpOficio
            (int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMP_OFICIO @POSICION, @OFICIO";
            SqlParameter ppos = new SqlParameter("@POSICION", posicion);
            SqlParameter pof = new SqlParameter("@OFICIO", oficio);

            var consulta = this.context.Empleados.FromSqlRaw
                (sql, ppos, pof);
            return await consulta.ToListAsync();
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

        public async Task<VistaEmpleado>
            GetVistaEmpleado(int posicion)
        {
            VistaEmpleado vista = await
                this.context.VistaEmpleados
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


        public async Task<List<Departamento>> GetDepartamentos()
        {
            var consulta = from datos in context.Departamentos
                           select datos;
            return await consulta.ToListAsync();    
        }

        public async Task<Departamento> FindDepto(int id)
        {
            var consulta = from datos in context.Departamentos
                           where datos.IdDepartamento == id
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<Empleado>> GetEmpleados()
        {
            var consulta = from datos in context.Empleados
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}
