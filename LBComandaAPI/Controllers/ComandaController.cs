using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ComandaController : ControllerBase
    {
        private readonly IComanda _comandaDAO;
        public ComandaController(IComanda comandaDAO) { _comandaDAO = comandaDAO; }
        [HttpGet, Route("GetAsync")]
        public async Task<IActionResult> GetAsync()
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _comandaDAO.GetAsync(Request.Headers["token"].ToString());
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
        [HttpPost, Route("GravarImpressoAsync")]
        public async Task<IActionResult> GravarImpressoAsync([FromBody] List<Comanda> itens)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                await _comandaDAO.GravarImpressoAsync(Request.Headers["token"].ToString(), itens);
                return Ok();
            }
            catch { return BadRequest(); }
        }
    }
}
