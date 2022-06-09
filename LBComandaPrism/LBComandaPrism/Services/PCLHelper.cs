using PCLStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaPrism.Services
{
    public static class PCLHelper
    {
        public async static Task<bool> ArquivoExisteAsync(this string nomeArquivo, IFolder pastaRaiz = null)
        {
            IFolder pasta = pastaRaiz ?? FileSystem.Current.LocalStorage;
            ExistenceCheckResult pastaExiste = await pasta.CheckExistsAsync(nomeArquivo);
            return pastaExiste == ExistenceCheckResult.FileExists;
        }
        public async static Task<bool> PastaExisteAsync(this string nomePasta, IFolder pastaRaiz = null)
        {
            IFolder pasta = pastaRaiz ?? FileSystem.Current.LocalStorage;
            ExistenceCheckResult pastaExiste = await pasta.CheckExistsAsync(nomePasta);
            return pastaExiste == ExistenceCheckResult.FolderExists;
        }
        public async static Task<IFolder> CriarPasta(this string nomePasta, IFolder pastaRaiz = null)
        {
            IFolder pasta = pastaRaiz ?? FileSystem.Current.LocalStorage;
            return await pasta.CreateFolderAsync(nomePasta, CreationCollisionOption.ReplaceExisting);
        }
        public async static Task<IFile> CriarArquivo(this string nomeArquivo, IFolder pastaRaiz = null)
        {
            IFolder pasta = pastaRaiz ?? FileSystem.Current.LocalStorage;
            return await pasta.CreateFileAsync(nomeArquivo, CreationCollisionOption.ReplaceExisting);
        }
        public async static Task<bool> WriteTextAllAsync(this string nomeArquivo, string content = "", IFolder pastaRaiz = null)
        {
            IFile arquivo = await nomeArquivo.CriarArquivo(pastaRaiz);
            await arquivo.WriteAllTextAsync(content);
            return true;
        }
        public async static Task<string> ReadAllTextAsync(this string nomeArquivo, IFolder pastaRaiz = null)
        {
            string conteudo = string.Empty;
            IFolder pasta = pastaRaiz ?? FileSystem.Current.LocalStorage;
            if (await nomeArquivo.ArquivoExisteAsync(pasta))
            {
                try
                {
                    IFile arquivo = await pasta.GetFileAsync(nomeArquivo);
                    conteudo = await arquivo.ReadAllTextAsync();
                }
                catch { throw new System.Exception("Erro ao ler arquivo."); }
            }
            return conteudo;
        }
        public async static Task<bool> DeleteFileAsync(this string nomeArquivo, IFolder pastaRaiz = null)
        {
            IFolder pasta = pastaRaiz ?? FileSystem.Current.LocalStorage;
            if (await nomeArquivo.ArquivoExisteAsync(pastaRaiz))
            {
                IFile arquivo = await pasta.GetFileAsync(nomeArquivo);
                await arquivo.DeleteAsync();
                return true;
            }
            return false;
        }
    }
}
