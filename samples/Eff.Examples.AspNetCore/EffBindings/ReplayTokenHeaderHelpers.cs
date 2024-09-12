namespace Nessos.Effects.Examples.AspNetCore.EffBindings;

using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

/// <summary>
///  Helper methods for reading and writing the replay token header.
/// </summary>
public static class ReplayTokenHeaderHelpers
{
    public const string EffReplayTokenHeaderName = "eff-replay-token";

    public static string? GetReplayTokenHeader(this HttpRequest request)
    {
        if (request.Headers.TryGetValue(EffReplayTokenHeaderName, out var values) && values.Count > 0)
        {
            return values[0];
        }

        return null;
    }

    public static void AddReplayTokenHeader(this HttpResponse response, string replayToken)
    {
        response.Headers.Append(EffReplayTokenHeaderName, replayToken);
    }
}

/// <summary>
///   Documents the replay token header in swagger
/// </summary>
public class SwashBuckleReplayHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            In = ParameterLocation.Header,
            Name = ReplayTokenHeaderHelpers.EffReplayTokenHeaderName,
            Required = false,
        });
    }
}
