using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using VehiculosApi.Models;
using System;

namespace VehiculosApi.Data
{
    public class VehiculoData
    {
        private readonly string conexion;

        public VehiculoData(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<Vehiculo>> ObtenerTodos()
        {
            List<Vehiculo> lista = new List<Vehiculo>();

            using (var conn = new MySqlConnection(conexion))
            {
                await conn.OpenAsync();

                var query = "SELECT Id, Marca, Modelo, Año, Color FROM vehiculo";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Vehiculo
                        {
                            Id = reader.GetInt32(0),
                            Marca = reader.GetString(1),
                            Modelo = reader.GetString(2),
                            Año = reader.GetInt32(3),
                            Color = reader.GetString(4)
                        });
                    }
                }
            }

            return lista;
        }

        public async Task<Vehiculo> ObtenerPorId(int id)
        {
            Vehiculo vehiculo = null;

            using (var conn = new MySqlConnection(conexion))
            {
                await conn.OpenAsync();

                var query = "SELECT Id, Marca, Modelo, Año, Color FROM vehiculo WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            vehiculo = new Vehiculo
                            {
                                Id = reader.GetInt32(0),
                                Marca = reader.GetString(1),
                                Modelo = reader.GetString(2),
                                Año = reader.GetInt32(3),
                                Color = reader.GetString(4)
                            };
                        }
                    }
                }
            }

            return vehiculo;
        }

        public async Task<int> Crear(Vehiculo vehiculo)
        {
            using (var conn = new MySqlConnection(conexion))
            {
                await conn.OpenAsync();

                // Consulta para obtener el ID máximo actual
                var queryMaxId = "SELECT MAX(Id) FROM vehiculo";
                int maxId = 0;
                using (var cmdMaxId = new MySqlCommand(queryMaxId, conn))
                {
                    var result = await cmdMaxId.ExecuteScalarAsync();
                    if (result != DBNull.Value)
                    {
                        maxId = Convert.ToInt32(result);
                    }
                }

                // Incrementar el ID en 1
                vehiculo.Id = maxId + 1;

                // Insertar el nuevo vehículo con el ID incrementado
                var query = "INSERT INTO vehiculo (Id, Marca, Modelo, Año, Color) VALUES (@Id, @Marca, @Modelo, @Año, @Color)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", vehiculo.Id);
                    cmd.Parameters.AddWithValue("@Marca", vehiculo.Marca);
                    cmd.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
                    cmd.Parameters.AddWithValue("@Año", vehiculo.Año);
                    cmd.Parameters.AddWithValue("@Color", vehiculo.Color);

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> Actualizar(Vehiculo vehiculo)
        {
            using (var conn = new MySqlConnection(conexion))
            {
                await conn.OpenAsync();

                var query = "UPDATE vehiculo SET Marca = @Marca, Modelo = @Modelo, Año = @Año, Color = @Color WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", vehiculo.Id);
                    cmd.Parameters.AddWithValue("@Marca", vehiculo.Marca);
                    cmd.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
                    cmd.Parameters.AddWithValue("@Año", vehiculo.Año);
                    cmd.Parameters.AddWithValue("@Color", vehiculo.Color);

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> Eliminar(int id)
        {
            using (var conn = new MySqlConnection(conexion))
            {
                await conn.OpenAsync();

                var query = "DELETE FROM vehiculo WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
