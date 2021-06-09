using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreLib.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataServer.API.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        [HttpGet("data")]
        public IActionResult Index()
        {
            string id = HttpContext.User.FindFirstValue("id");
            string email = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            string username = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            return Ok(new DataResponse
            {
                Value = "123"
            });
        }
    }
}
