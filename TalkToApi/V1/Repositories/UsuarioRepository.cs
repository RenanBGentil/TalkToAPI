using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkToApi.Repositories.V1.Contracts;
using TalkToApi.V1.Models;

namespace TalkToApi.V1.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuarioRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser Obter(string email, string senha)
        {
           var usuario = _userManager.FindByEmailAsync(email).Result;
            if (_userManager.CheckPasswordAsync(usuario, senha).Result)
            {
                return usuario;
            }
            else
            {
                throw new Exception("Usuário não localizado!");
            }
        }

        public void Cadastrar(ApplicationUser usuario, string senha)
        {
           var result = _userManager.CreateAsync(usuario, senha).Result;

            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in result.Errors) 
                {
                    sb.Append(error.Description);
                }
                
                throw new Exception($"Usuário não localizado! {sb.ToString()}");
            }
        }

        public ApplicationUser Obter(string id)
        {
            return _userManager.FindByIdAsync(id).Result;
        }
    }
}
