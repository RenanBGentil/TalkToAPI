using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToApi.DataBase;
using TalkToApi.V1.Models;
using TalkToApi.V1.Repositories.Contracts;

namespace TalkToApi.V1.Repositories
{
    public class MensagemRepository : IMensagemRepository
    {
        private readonly TalkToContext _banco;
        public MensagemRepository(TalkToContext banco) 
        {
            _banco = banco;
        }

        public List<Mensagem> ObterMensagens(string UsuarioUmId, string UsuarioDoisId)
        {
            return _banco.Mensagem.Where(a=> (a.DeId == UsuarioUmId || a.DeId == UsuarioDoisId)
            && (a.ParaId == UsuarioUmId || a.ParaId == UsuarioDoisId)).ToList();
        }

        public void Cadastrar(Mensagem mensagem)
        {
            _banco.Add(mensagem);
            _banco.SaveChanges();
        }

        public void Atualizar(Mensagem mensagem)
        {
            _banco.Update(mensagem);
            _banco.SaveChanges();
        }

        public Mensagem Obter(int id)
        {
            return _banco.Mensagem.Find(id);
        }
    }
}
