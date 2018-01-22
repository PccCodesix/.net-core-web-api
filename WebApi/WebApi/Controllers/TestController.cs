using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context;

namespace WebApi.Controllers
{
    //[Produces("application/json")]
    [Route("api/[Controller]")]
    public class TestController : Controller
    {
        private MyContext _context;

        public TestController(MyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

    }
}