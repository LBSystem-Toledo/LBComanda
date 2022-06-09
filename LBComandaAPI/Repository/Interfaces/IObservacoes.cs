using LBComandaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IObservacoes
    {
        Task<IEnumerable<Observacoes>> GetAsync(string token, string cd_produto);
    }
}
