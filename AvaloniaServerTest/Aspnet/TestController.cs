using System;
using Microsoft.AspNetCore.Mvc;

namespace AvaloniaServerTest.Aspnet;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return Ok($"Hello from ASP.NET Core at {DateTime.Now}!");
    }
}