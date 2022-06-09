using System.Text.RegularExpressions;

namespace LBComandaPrism.Utils
{
    public static class Extensoes
    {
        public static string SoNumero(this object valor)
        {
            if (valor == null)
                return string.Empty;
            string ret = string.Empty;
            foreach (char c in valor.ToString().ToCharArray())
                if (char.IsNumber(c))
                    ret += c;
            return ret;
        }
        public static string RemoverCaracteres(this object valor)
        {
            if (valor == null)
                return string.Empty;
            string retorno = valor.ToString();
            //Remover acentos do (a) minusculo
            retorno = Regex.Replace(retorno, "[äáâàã]", "a");
            //Remover acentos do (A) maiusculo
            retorno = Regex.Replace(retorno, "[ÄÅÁÂÀÃ]", "A");
            //Remover acentos do (e) minusculo
            retorno = Regex.Replace(retorno, "[éêëè]", "e");
            //Remover acentos do (E) maiusculo
            retorno = Regex.Replace(retorno, "[ÉÊËÈ]", "E");
            //Remover acentos do (i) minusculo
            retorno = Regex.Replace(retorno, "[íîïì]", "i");
            //Remover acentos do (I) maiusculo
            retorno = Regex.Replace(retorno, "[ÍÎÏÌ]", "I");
            //Remover acentos do (o) minusculo
            retorno = Regex.Replace(retorno, "[öóôòõ]", "o");
            //Remover acentos do (O) maiusculo
            retorno = Regex.Replace(retorno, "[ÖÓÔÒÕ]", "O");
            //Remover acentos do (u) minusculo
            retorno = Regex.Replace(retorno, "[üúûù]", "u");
            //Remover acentos do (U) maiusculo
            retorno = Regex.Replace(retorno, "[ÜÚÛ]", "U");
            //Remover acentos do (ç) minusculo
            retorno = Regex.Replace(retorno, "[ç]", "c");
            //Remover acentos do (Ç) maiusculo
            retorno = Regex.Replace(retorno, "[Ç]", "C");
            //Remover caracter especial
            retorno = Regex.Replace(retorno, "[º]", "");
            //Remover acentos do (nº) minusculo
            retorno = Regex.Replace(retorno, "[nº]", "n");
            //Remover acentos do (Nº) maiusculo
            retorno = Regex.Replace(retorno, "[Nº]", "N");
            //Remover acentos do (nª) minusculo
            retorno = Regex.Replace(retorno, "[nª]", "n");
            //Remover acentos do (Nª) maiusculo
            retorno = Regex.Replace(retorno, "[Nª]", "N");

            return retorno;
        }
    }
}
