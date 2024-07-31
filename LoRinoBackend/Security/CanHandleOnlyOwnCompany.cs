using LoRinoBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace LoRinoBackend.Security
{
    public class CanHandleOnlyOwnCompany : AuthorizationHandler<CompanyClaimsRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor;


        public CanHandleOnlyOwnCompany(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CompanyClaimsRequirement requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if (authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            string loggedInUserId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var testEmail = authFilterContext.HttpContext.Request.Query["userId"];

            //var currentUser = requirement.company;

            //string test = currentUser.Email;

            

            if (context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
                testEmail == "1"
                )
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
