namespace Nessos.Eff.Examples.AspNetCore.EffBindings
{
    using System.Collections.Generic;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Microsoft.OpenApi.Models;
    using Microsoft.AspNetCore.Http;

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
            response.Headers.Add(EffReplayTokenHeaderName, replayToken);
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
}
