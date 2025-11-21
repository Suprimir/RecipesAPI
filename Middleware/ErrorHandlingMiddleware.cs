using System.Net;
using System.Text.Json;
using Npgsql;
using Microsoft.EntityFrameworkCore;

namespace RecipesAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;
            var errors = new List<string>();

            switch (exception)
            {
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    errors.Add("No tienes autorización para realizar esta acción");
                    break;

                case KeyNotFoundException:
                    code = HttpStatusCode.NotFound;
                    errors.Add(exception.Message ?? "Recurso no encontrado");
                    break;

                case ArgumentNullException:
                    code = HttpStatusCode.BadRequest;
                    errors.Add(exception.Message ?? "Parámetro requerido no proporcionado");
                    break;

                case ArgumentException:
                    code = HttpStatusCode.BadRequest;
                    errors.Add(exception.Message ?? "Solicitud inválida");
                    break;

                case InvalidOperationException:
                    code = HttpStatusCode.BadRequest;
                    errors.Add(exception.Message ?? "Operación no válida");
                    break;

                case Npgsql.PostgresException pgEx:
                    code = HttpStatusCode.BadRequest;
                    errors.Add(HandlePostgresException(pgEx));
                    break;

                case DbUpdateException dbEx when dbEx.InnerException is Npgsql.PostgresException pgInnerEx:
                    code = HttpStatusCode.BadRequest;
                    errors.Add(HandlePostgresException(pgInnerEx));
                    break;

                case TimeoutException:
                    code = HttpStatusCode.RequestTimeout;
                    errors.Add("La operación ha excedido el tiempo límite");
                    break;

                default:
                    errors.Add("Ha ocurrido un error interno en el servidor");
                    break;
            }

            var response = new ErrorResponse
            {
                Success = false,
                Message = errors.First(),
                Errors = errors,
                StatusCode = (int)code,
                Timestamp = DateTime.UtcNow
            };

            result = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }

        private static string HandlePostgresException(PostgresException pgEx)
        {
            return pgEx.SqlState switch
            {
                // Violación de clave única
                "23505" => "Ya existe un registro con esos datos",

                // Violación de clave foránea
                "23503" => "No se puede completar la operación debido a referencias existentes",

                // Violación de restricción NOT NULL
                "23502" => "Faltan datos requeridos",

                // Violación de restricción CHECK
                "23514" => "Los datos no cumplen con las restricciones de validación",

                // Tabla no existe
                "42P01" => "Recurso no encontrado en la base de datos",

                // Columna no existe
                "42703" => "Campo no encontrado",

                // División por cero
                "22012" => "Error en cálculo matemático",

                // Tipo de dato inválido
                "22P02" => "Formato de datos inválido",

                // Violación de restricción de integridad
                "23000" => "Error de integridad de datos",

                _ => $"Error de base de datos: {pgEx.MessageText}"
            };
        }
    }

    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
