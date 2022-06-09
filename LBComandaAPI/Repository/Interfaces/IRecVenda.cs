using LBComandaAPI.Models;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IRecVenda
    {
        Task<bool> ReceberVendaAsync(string token, RecVenda rec);
    }
}
