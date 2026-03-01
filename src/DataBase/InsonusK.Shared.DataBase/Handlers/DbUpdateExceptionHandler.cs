using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.DataBase.Handlers;

public static class DbUpdateExceptionHandler
{
    public static Result<TResponse> ToResult<TResponse>(this DbUpdateException ex, ILogger _logger, UXConfig[] uxConfigs)
    {
        _logger.LogError(ex, "Database update error occurred.");

        // Обработка исключений PostgreSQL
        if (ex.InnerException is PostgresException postgresException)
        {
            // Ошибка нарушения уникального индекса
            if (postgresException.SqlState == "23505")
            {
                var constraintName = postgresException.ConstraintName;
                foreach (var uxConfig in uxConfigs)
                {
                    if (constraintName == uxConfig.UX_name)
                    {
                        return Result<TResponse>.Conflict(
                            $"A record with this identifier ({string.Join(',', uxConfig.UX_fields)}) already exists."
                        );
                    }
                }
            }
        }
        else if (ex is DbUpdateConcurrencyException)
        {
            return Result<TResponse>.Conflict("The record has been modified by another user.");
        }
        throw ex;
    }
}