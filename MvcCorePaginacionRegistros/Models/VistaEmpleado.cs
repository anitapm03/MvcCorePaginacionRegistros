using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcCorePaginacionRegistros.Models
{
    [Table("V_EMP_INDIVIDUAL")]
    public class VistaEmpleado
    {
        [Key]
        [Column("EMP_NO")]
        public int IdEmpleado { get; set; }
        [Column("APELLIDO")]
        public string Apellido { get; set; }
        [Column("OFICIO")]
        public string Oficio { get; set; }
        [Column("SALARIO")]
        public int Salario { get; set; }
        [Column("DEPT_NO")]
        public int IdDepartamento { get; set; }
        [Column("POSICION")]
        public int Posicion { get; set; }
    }
}
/*CREATE VIEW V_EMP_INDIVIDUAL
AS
	SELECT CAST(
	ROW_NUMBER() OVER (ORDER BY EMP_NO) AS INT)
	AS POSICION,
	ISNULL(EMP_NO, 0) AS EMP_NO, APELLIDO, OFICIO,
	SALARIO, DEPT_NO 
	FROM EMP
GO*/