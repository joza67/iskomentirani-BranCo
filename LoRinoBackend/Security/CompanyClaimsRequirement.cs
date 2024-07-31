using LoRinoBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.Security
{
    public class CompanyClaimsRequirement : IAuthorizationRequirement
    {
    }
}
