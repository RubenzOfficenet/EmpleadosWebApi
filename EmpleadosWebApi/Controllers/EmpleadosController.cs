using EmpleadosWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EmpleadosWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private readonly string filePath = "Data/empleados.json";

        #region Private methods

        // Leer empleados del archivo JSON
        private List<Empleado> LeerEmpleados()
        {
            var jsonData = System.IO.File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true 
            };

            var empleados = JsonSerializer.Deserialize<Dictionary<string, List<Empleado>>>(jsonData, options);
            return empleados?["empleados"] ?? new List<Empleado>();
        }

        // Guardar empleados en el archivo JSON
        private void GuardarEmpleados(List<Empleado> empleados)
        {
            var empleadosData = new Dictionary<string, List<Empleado>> { { "empleados", empleados } };
            var jsonData = JsonSerializer.Serialize(empleadosData);
            System.IO.File.WriteAllText(filePath, jsonData);
        }

        #endregion

        #region Public methods

        // GET: api/empleados/{numeroEmp?}
        [HttpGet]
        public ActionResult<List<Empleado>> ObtenerEmpleados()
        {
            var empleados = LeerEmpleados();

            return Ok(empleados);
        }

        // GET: api/empleados/{numeroEmp?}
        [HttpGet("{numeroEmp}")]
        public IActionResult ObtenerEmpleadosPorId(int numeroEmp)
        {
            var empleados = LeerEmpleados();

            
            var empleado = empleados.FirstOrDefault(e => e.NumeroEmp == numeroEmp);
            if (empleado == null)
            {
                return NotFound("Empleado no encontrado.");
            }
            return Ok(empleado);
            
        }

        // DELETE: api/empleados/{numeroEmp}
        [HttpDelete("{numeroEmp}")]
        public IActionResult EliminarEmpleado(int numeroEmp)
        {
            var empleados = LeerEmpleados();
            var empleado = empleados.FirstOrDefault(e => e.NumeroEmp == numeroEmp);

            if (empleado == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            empleados.Remove(empleado);
            GuardarEmpleados(empleados);
            return Ok($"Empleado {numeroEmp} eliminado.");
        }

        // PUT: api/empleados/{numeroEmp}
        [HttpPut("{numeroEmp}")]
        public IActionResult ActualizarEmpleado(int numeroEmp, [FromBody] Empleado empleadoActualizado)
        {
            var empleados = LeerEmpleados();
            var empleado = empleados.FirstOrDefault(e => e.NumeroEmp == numeroEmp);

            if (empleado == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            if (!string.IsNullOrEmpty(empleadoActualizado.Nombre))
            {
                empleado.Nombre = empleadoActualizado.Nombre;
            }

            if (!string.IsNullOrEmpty(empleadoActualizado.Apellidos))
            {
                empleado.Apellidos = empleadoActualizado.Apellidos;
            }

            GuardarEmpleados(empleados);
            return Ok(empleado);
        }

        // POST: api/empleados
        [HttpPost]
        public IActionResult AgregarEmpleado([FromBody] Empleado nuevoEmpleado)
        {
            if (nuevoEmpleado == null || string.IsNullOrEmpty(nuevoEmpleado.Nombre) || string.IsNullOrEmpty(nuevoEmpleado.Apellidos) || nuevoEmpleado.NumeroEmp <= 0)
            {
                return BadRequest("Todos los campos son requeridos y el número de empleado debe ser mayor a 0.");
            }

            var empleados = LeerEmpleados();
            if (empleados.Any(e => e.NumeroEmp == nuevoEmpleado.NumeroEmp))
            {
                return BadRequest("El número de empleado ya existe.");
            }

            empleados.Add(nuevoEmpleado);
            GuardarEmpleados(empleados);

            return CreatedAtAction(nameof(ObtenerEmpleadosPorId), new { numeroEmp = nuevoEmpleado.NumeroEmp }, nuevoEmpleado);
        }

        #endregion

    }
}
