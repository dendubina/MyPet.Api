using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MyPet.Api.Extensions
{
    public static class RequestExtensions
    {
        public static string GetUserId(this HttpRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Headers["Authorization"]))
            {
                throw new UnauthorizedAccessException("Not Authorized");
            }

            return request.HttpContext
                .User
                .Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.UniqueName)?
                .Value;
        }
    }
}
