using LBComandaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IAdicional
    {
        Task<IEnumerable<Adicional>> GetAsync(string token, string Cd_produto);
    }
}
