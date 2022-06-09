using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RecVendaController : ControllerBase
    {
        private readonly IRecVenda _query;
        public RecVendaController(IRecVenda query) { _query = query; }
        [HttpPost, Route("ReceberVendaAsync")]
        public async Task<IActionResult> ReceberVendaAsync([FromBody] RecVenda rec)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var result = await _query.ReceberVendaAsync(Request.Headers["token"].ToString(), rec);
                return Ok(result);
            }
            catch(Exception ex) { return BadRequest(ex.Message.Trim()); }
        }
    }
}
