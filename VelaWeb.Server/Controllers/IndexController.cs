using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using VelaWeb.Dtos;
using VelaWeb.Server.DBModels;
using VelaWeb.Dtos;
using Way.Lib;
using JMS.Token;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Xml.Linq;
using VelaLib;
using VelaWeb.Server.Infrastructures;
using System.IO.Compression;
using System.Text;

namespace VelaWeb.Server.Controllers
{
    [ApiController]
    [Route("/")]
    public class IndexController : AuthController
    {
        [HttpGet]
        public ContentResult Get()
        {
            var path = Path.Combine("wwwroot", "index.html");
            var htmlContent = System.IO.File.ReadAllText(path, Encoding.UTF8);
            return Content(htmlContent, "text/html");
        }
    }
}
