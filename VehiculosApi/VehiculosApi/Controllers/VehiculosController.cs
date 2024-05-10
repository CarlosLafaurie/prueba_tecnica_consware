using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehiculosApi.Data;
using VehiculosApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VehiculosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {
        private readonly VehiculoData _vehiculoData;

        public VehiculosController(VehiculoData vehiculoData)
        {
            _vehiculoData = vehiculoData;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            List<Vehiculo> vehiculos = await _vehiculoData.ObtenerTodos();
            return StatusCode(StatusCodes.Status200OK, vehiculos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var vehiculo = await _vehiculoData.ObtenerPorId(id);
            if (vehiculo == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Vehículo con ID {id} no encontrado.");

            return StatusCode(StatusCodes.Status200OK, vehiculo);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] Vehiculo vehiculo)
        {
            if (vehiculo == null)
                return StatusCode(StatusCodes.Status400BadRequest, "El vehículo enviado es nulo.");

            await _vehiculoData.Crear(vehiculo);
            return StatusCode(StatusCodes.Status201Created, vehiculo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Vehiculo vehiculo)
        {
            var vehiculoExistente = await _vehiculoData.ObtenerPorId(id);
            if (vehiculoExistente == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Vehículo con ID {id} no encontrado.");

            vehiculo.Id = id; // Asegurarse de que el ID del vehículo coincida con el ID proporcionado en la solicitud
            await _vehiculoData.Actualizar(vehiculo);
            return StatusCode(StatusCodes.Status200OK, vehiculo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var vehiculoExistente = await _vehiculoData.ObtenerPorId(id);
            if (vehiculoExistente == null)
                return NotFound($"Vehículo con ID {id} no encontrado.");

            await _vehiculoData.Eliminar(id);
            return Ok(new { message = $"Vehículo con ID {id} eliminado correctamente." });
        }
    }
}
