using LBComandaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IMesa
    {
        Task<IEnumerable<Mesa>> GetAsync(string token);
        Task<IEnumerable<Local>> GetLocalAsync(string token);
    }
}
