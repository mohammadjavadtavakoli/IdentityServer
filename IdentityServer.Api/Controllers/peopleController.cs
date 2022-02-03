using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class peopleController : ControllerBase
    {
        
        [HttpGet]
        public IActionResult Get()
        {
            var people = new List<Person>()
            {
                  new Person
                {
                    FirstName="Mohammad",
                    LastName="Tavakoli",
                    Age=30
                },
                  new Person
                {
                    FirstName="Lelila",
                    LastName="Kefayati",
                    Age=28
               },
                  new Person
                {
                    FirstName="Alireza",
                    LastName="Tavakoli",
                    Age=25
               }
            };

            return new JsonResult(people);
        }
    }
}
