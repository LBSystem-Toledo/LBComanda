using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class EntregaController : ControllerBase
    {
        private readonly IEntrega _entregaDAO;
        public EntregaController(IEntrega entregaDAO) { _entregaDAO = entregaDAO; }
        [HttpPost, Route("ApontarEntregaAsync")]
        public async Task<IActionResult> ApontarEntregaAsync([FromQuery]string Id_prevenda,
                                                             [FromQuery]string Cd_entregador)
        {
            if (!Request.Headers.ContainsKey("token"))
                return Unauthorized();
            try
            {
                var ret = await _entregaDAO.ApontarEntregaAsync(Request.Headers["token"].ToString(),
                                                                Id_prevenda,
                                                                Cd_entregador);
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
        [HttpPost, Route("ConcluirEntregaAsync")]
        public async Task<IActionResult> ConcluirEntregaAsync([FromBody]Entrega entrega)
        {
            if (!Request.Headers.ContainsKey("token"))
                return Unauthorized();
            try
            {
                var ret = await _entregaDAO.ConcluirEntregaAsync(Request.Headers["token"].ToString(), entrega);
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
    }
}
