using AccessControl.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AccessApi.Controllers
{
	[Route("api/auth")]
	public class AuthController : Controller
	{
		private readonly IJwtManager _jwtManager;
		public AuthController(IJwtManager jwtManager) => _jwtManager = jwtManager;

		[HttpGet("00")]
		public IActionResult Login()
		{
			var token = _jwtManager.Create("alex@wataba.com");
			return Json(token);
		}

		[HttpGet("01")]
		public IActionResult SayHello() => Json("Cool! You has been authorized!");

		[HttpGet("02")]
		public IActionResult Logout()
		{
			_jwtManager.Revoke();
			return Ok();
		}
	}
}