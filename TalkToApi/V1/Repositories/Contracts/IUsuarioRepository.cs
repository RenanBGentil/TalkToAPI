using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToApi.V1.Models;

namespace TalkToApi.Repositories.V1.Contracts
{
    public interface IUsuarioRepository
    {
        void Cadastrar(ApplicationUser usuario, string senha);

        ApplicationUser Obter(string email, string senha);
        ApplicationUser Obter(string id);
    }
}
