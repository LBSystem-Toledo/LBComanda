using LBComandaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IItemVenda
    {
        Task<IEnumerable<ItemVenda>> GetAsync(string token, string Id_local, string Id_mesa, string Nr_cartao);
        Task<bool> GravarItensAsync(string token, List<ItemVenda> items);
        Task<bool> GravarItensAsync(string token, string Id_local, string Id_mesa, List<ItemVenda> items);
        Task<bool> GravarItensAsync(string token, string Nr_cartao, List<ItemVenda> items);
        Task<bool> GravarItensBalcaoAsync(string token, string ClienteBalcao, List<ItemVenda> items);
    }
}
