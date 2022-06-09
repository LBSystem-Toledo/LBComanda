using LBComandaAPI.Models;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IEntrega
    {
        Task<Entrega> ApontarEntregaAsync(string token, string id_prevenda, string cd_entregador);
        Task<bool> ConcluirEntregaAsync(string token, Entrega entrega);
    }
}
