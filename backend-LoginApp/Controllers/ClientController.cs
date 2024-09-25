using backend_LoginApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_LoginApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly LoginAppContext _context;

        public ClientController(LoginAppContext context)
        {
            _context = context;
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
                return Ok(new {message = "Login exitoso", client = existingClient});
            }

            return Unauthorized(new { message = "Correo o contraseña incorrectos" });
        }
    }
}
