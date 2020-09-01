using System;
using System.Collections.Generic;
using System.Linq;
using TalkToApi.DataBase;
using TalkToApi.Repositories.V1.Contracts;
using TalkToApi.V1.Models;

namespace TalkToApi.V1.Repositories
{
    public class TarefasRepository : ITarefasRepository
    {
        private readonly TalkToContext _banco;

        public TarefasRepository(TalkToContext banco)
        {
            _banco = banco;
        }

        public List<Tarefa> Resturacao(ApplicationUser usuario, DateTime dataUltimaSincronizacao)
        {
            var query = _banco.Tarefas.Where(a=> a.UsuarioId == usuario.Id).AsQueryable();
            if (dataUltimaSincronizacao != null)
            {
                query.Where(a=> a.Criado >= dataUltimaSincronizacao || a.Atualizado >= dataUltimaSincronizacao);
            }

            return query.ToList<Tarefa>();
        }

        public List<Tarefa> Sincronizacao(List<Tarefa> tarefas)
        {
            var tarefasNovas = tarefas.Where(a=> a.IdTarefaApi == 0).ToList();
            var tarefasExcluidasAtualizadas = tarefas.Where(a => a.IdTarefaApi != 0).ToList();

            if (tarefasNovas.Count() > 0)
            {
                foreach (var tarefa in tarefasNovas)
                {
                    _banco.Tarefas.Add(tarefa);
                }
            }


            if (tarefasExcluidasAtualizadas.Count() > 0)
            {
                foreach (var tarefa in tarefasExcluidasAtualizadas)
                {
                    _banco.Tarefas.Update(tarefa);
                }
            }
            _banco.SaveChanges();

            return tarefasNovas.ToList() ; 
        }

    }
}
