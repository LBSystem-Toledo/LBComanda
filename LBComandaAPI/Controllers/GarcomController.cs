using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class GarcomController : ControllerBase
    {
        private readonly IGarcom _garcomDAO;
        public GarcomController(IGarcom garcomDAO) { _garcomDAO = garcomDAO; }
        
        [HttpGet, Route("ValidarGarcomAsync")]
        public async Task<IActionResult> ValidarGarcomAsync()
        {
            if (!Request.Headers.ContainsKey("cnpj"))
                return StatusCode(500, "Obrigatório informar CNPJ");
            if (!Request.Headers.ContainsKey("login"))
                return StatusCode(500, "Obrigatório informar LOGIN");
            if (!Request.Headers.ContainsKey("senha"))
                return StatusCode(500, "Obrigatório informar SENHA");
            try
            {
                var ret = await _garcomDAO.ValidarGarcomAsync(Request.Headers["login"].ToString(), Request.Headers["senha"].ToString(), Request.Headers["cnpj"].ToString());
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
        [HttpGet, Route("ValidarTokenAsync")]
        public async Task<IActionResult> ValidarTokenAsync()
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            if (!Request.Headers.ContainsKey("garcom"))
                return StatusCode(500, "Obrigatório informar GARÇOM");
            try
            {
                var ret = await _garcomDAO.ValidarTokenAsync(Request.Headers["token"].ToString(), Request.Headers["garcom"].ToString());
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
        [HttpPost, Route("SolicitarTokenAsync")]
        public async Task<IActionResult> SolicitarTokenAsync()
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            if (!Request.Headers.ContainsKey("garcom"))
                return StatusCode(500, "Obrigatório informar GARÇOM");
            try
            {
                var ret = await _garcomDAO.SolicitarToken(Request.Headers["token"].ToString(), Request.Headers["garcom"].ToString());
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
    }
}
