using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ehs5
{
    public class JWTAction : Attribute, IAuthorizationFilter
    {
        private readonly string _allowedRoles;

        // Constructor that accepts the allowed roles as a comma-separated string
        public JWTAction(string allowedRoles = null)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Session.GetString("JwtToken");

            // Ensure the token is passed as a header if not already
            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                context.HttpContext.Request.Headers.Add("Authorization", token);
            }

            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (!ValidateToken(authToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check for the role if provided
            if (!string.IsNullOrEmpty(_allowedRoles) && !HasAnyRole(authToken, _allowedRoles))
            {
                context.Result = new ForbidResult(); // 403 Forbidden
                return;
            }
        }

        // Validate the JWT token
        private bool ValidateToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null || jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return false; // Token is expired or invalid
                }

                return true;
            }
            catch (Exception)
            {
                return false; // Token is invalid
            }
        }

        // Check if the token contains any of the allowed roles
        private bool HasAnyRole(string token, string allowedRoles)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                return false;
            }

            // Get the role claim from the token
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
            if (roleClaim == null)
            {
                return false; // No role claim in token
            }

            // Split allowed roles and check if the user's role matches any
            var roles = allowedRoles.Split(',').Select(r => r.Trim());

            return roles.Contains(roleClaim.Value);
        }
    }
}
