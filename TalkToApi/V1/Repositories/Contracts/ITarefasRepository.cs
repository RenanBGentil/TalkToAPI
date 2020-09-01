using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToApi.V1.Models;

namespace TalkToApi.Repositories.V1.Contracts
{
    public interface ITarefasRepository
    {
        List<Tarefa> Sincronizacao(List<Tarefa> tarefas);

        List<Tarefa> Resturacao(ApplicationUser usuario,DateTime dataUltimaSincronizacao);
    }
}
