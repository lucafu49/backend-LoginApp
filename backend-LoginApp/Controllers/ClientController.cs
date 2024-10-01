using backend_LoginApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend_LoginApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly LoginAppContext _context;
        private readonly IConfiguration _configuration;

        public ClientController(LoginAppContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Clients")]
        public async Task<IActionResult> getClient()
        {
            var listClient = await _context.Clients.ToListAsync();
            return Ok(listClient);
        }

        [HttpGet]
        [Route("{id:int}")] 
        public async Task<IActionResult> getClientById(int id)
        {
            var request = await _context.Clients.FindAsync(id);

            if (request == null) 
            {
                return BadRequest("No existe el cliente");
            }
            return Ok(request);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> registerClient([FromBody] Client request)
        {
            var existingMail = await _context.Clients.FirstOrDefaultAsync(c => c.Mail == request.Mail);

            if (existingMail != null)
            {
                return Conflict(new { message = "Correo ya existente" });
            }

            await _context.Clients.AddAsync(request);
            await _context.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Mail == loginDto.Mail && c.Password == loginDto.Password);

            if (existingClient != null)
            {
                var token = GenerateJwtToken(existingClient);

                return Ok(new
                {
                    message = "Login exitoso",
                    token = token, // Incluir el token en la respuesta
                    client = existingClient
                });
            }

            return Unauthorized(new { message = "Correo o contraseña incorrectos" });
        }

        private string GenerateJwtToken(Client client) 
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, client.Mail),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("clientId", client.IdClient.ToString())
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1), // El token expira en 1 hora
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
