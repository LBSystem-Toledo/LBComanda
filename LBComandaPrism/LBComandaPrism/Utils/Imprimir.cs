using System;
using System.Collections.Generic;
using System.Text;

namespace LBComandaPrism.Utils
{
    public static class TImprimir
    {
        public static string ImprimirVale(string produto)
        {
            StringBuilder imp = new StringBuilder();
            imp.AppendLine("--------------------------------")
                .AppendLine(App.Garcom.Nm_empresa.RemoverCaracteres().Trim().ToUpper())
                .AppendLine(App.Garcom.Endereco_empresa.RemoverCaracteres().Trim().ToUpper())
                .AppendLine("--------------------------------")
                .AppendLine()
                .AppendLine("              VALE")
                .AppendLine()
                .AppendLine(produto.RemoverCaracteres().Trim().ToUpper())
                .AppendLine()
                .AppendLine("Data Vale: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"))
                .AppendLine("--------------------------------")
                .AppendLine()
                .AppendLine();
            return imp.ToString();
        }
    }
}
