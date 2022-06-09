using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly ICartao _cartaoDAO;
        public CartaoController(ICartao cartaoDAO) { _cartaoDAO = cartaoDAO; }

        [HttpGet, Route("ConsultarCartaoAbertoAsync")]
        public async Task<IActionResult> ConsultarCartaoAbertoAsync([FromQuery] int Nr_cartao)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _cartaoDAO.ConsultarCartaoAbertoAsync(Request.Headers["token"].ToString(), Nr_cartao);
                return Ok(ret);
            }
            catch(Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet, Route("AbrirCartaoAsync")]
        public async Task<IActionResult> AbrirCartaoAsync([FromQuery] int Nr_cartao,
                                                          [FromQuery] string Celular,
                                                          [FromQuery] string Nome,
                                                          [FromQuery] bool MenorIdade)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _cartaoDAO.AbrirCartaoAsync(Request.Headers["token"].ToString(), Nr_cartao, Celular, Nome, MenorIdade);
                return Ok(ret);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet, Route("ConsultaClienteAsync")]
        public async Task<IActionResult> ConsultaClienteAsync([FromQuery] string Celular)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _cartaoDAO.ConsultaClienteAsync(Request.Headers["token"].ToString(), Celular);
                return Ok(ret);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
