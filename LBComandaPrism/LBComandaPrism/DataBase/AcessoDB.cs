using LBComandaPrism.Models;
using LiteDB;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LBComandaPrism.DataBase
{
    public class AcessoDB
    {
        readonly LiteDatabase _litedb;
        public AcessoDB(string dbPath)
        {
            _litedb = new LiteDatabase(Path.Combine(dbPath, "Banco.db"));
            _litedb
                .Mapper
                .Entity<Entrega>()
                .Id(p => p.Id_prevenda);
        }
        public async Task<List<Entrega>> GetEntregasAsync(bool Entregue = false)
        {
            List<Entrega> retorno = await Task.Run(() =>
             {
                 return _litedb.GetCollection<Entrega>().Query().Where(p=> p.Entregue == Entregue).ToList();
             });
            return retorno;
        }
        public async Task<bool> GravarEntregaAsync(Entrega entrega)
        {
            bool ret = await Task.Run(() =>
            {
                return _litedb.GetCollection<Entrega>().Upsert(entrega);
            });
            return ret;
        }
        public async Task<bool> ExcluirEntregaAsync(Entrega entrega)
        {
            bool ret = await Task.Run(() =>
            {
                return _litedb.GetCollection<Entrega>().Delete(entrega.Id_prevenda);
            });
            return ret;
        }
    }
}
