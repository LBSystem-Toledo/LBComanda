using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemVendaController : ControllerBase
    {
        private readonly IItemVenda _itemDAO;

        public ItemVendaController(IItemVenda itemDAO)
        { 
            _itemDAO = itemDAO;
        }
        [HttpGet, Route("GetExtratoMesaAsync")]
        public async Task<IActionResult> GetExtratoMesaAsync([FromQuery] string Id_local, 
                                                             [FromQuery] string Id_mesa)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _itemDAO.GetAsync(Request.Headers["token"].ToString(), Id_local, Id_mesa, string.Empty);
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
        [HttpGet, Route("GetExtratoCartaoAsync")]
        public async Task<IActionResult> GetExtratoCartaoAsync([FromQuery] string Nr_cartao)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _itemDAO.GetAsync(Request.Headers["token"].ToString(), string.Empty, string.Empty, Nr_cartao);
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
        [HttpPost, Route("GravarComandaMesaAsync")]
        public async Task<IActionResult> GravarItensAsync(string Id_local,
                                                          string Id_mesa,
                                                          [FromBody] List<ItemVenda> items)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _itemDAO.GravarItensAsync(Request.Headers["token"].ToString(), Id_local, Id_mesa, items);
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
        [HttpPost, Route("GravarComandaCartaoAsync")]
        public async Task<IActionResult> GravarComandaCartaoAsync([FromQuery] string Nr_cartao,
                                                                  [FromBody] List<ItemVenda> items)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _itemDAO.GravarItensAsync(Request.Headers["token"].ToString(), Nr_cartao, items);
                return Ok(ret);
            }
            catch(Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
