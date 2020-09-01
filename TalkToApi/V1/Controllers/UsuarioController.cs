using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TalkToApi.Helpers.Constants;
using TalkToApi.Repositories.V1.Contracts;
using TalkToApi.V1.Models;
using TalkToApi.V1.Models.DTO;
using TalkToApi.V1.Repositories.Contracts;

namespace TalkToApi.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [EnableCors("AnyOrigin")]
    public class UsuarioController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuarioController(IUsuarioRepository usuarioRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            ITokenRepository tokenRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("", Name = "UsuarioObterTodos")]
        public ActionResult Obter([FromHeader(Name = "Accept")] string mediaType)
        {
            var usuariosAppUser = _userManager.Users.ToList();

            if (mediaType == CustomMediaType.Hetoas)
            {
                var listaUsuarioDTO = _mapper.Map<List<ApplicationUser>, List<UsuarioDTO>>(usuariosAppUser);

                foreach (var usuarioDTO in listaUsuarioDTO)
                {
                    usuarioDTO.Links.Add(new LinkDTO("_self",
                        Url.Link("UsuarioObterUsuario", new { id = usuarioDTO.Id }),
                       "GET"));
                }

                var lista = new ListaDTO<UsuarioDTO> { Lista = listaUsuarioDTO };
                lista.Links.Add(new LinkDTO("_self",
                        Url.Link("UsuarioObterTodos", null),
                       "GET"));

                return Ok(lista);
            }
            else
            {
                var usuarioResult =_mapper.Map<List<ApplicationUser>, List<UsuarioDtoSemHiperLink>>(usuariosAppUser);
                return Ok(usuariosAppUser);
            }
        }

        [Authorize]
        [HttpGet("{id}", Name = "UsuarioObter")]
        [DisableCors]

        public ActionResult ObterUsuario(string id, [FromHeader(Name = "Accept")] string mediaType)
        {
            var usuario = _userManager.FindByIdAsync(id).Result;
            if (usuario == null)
                return NotFound();

            if (mediaType == CustomMediaType.Hetoas) {
                var usuarioDtoDb = _mapper.Map<ApplicationUser, UsuarioDTO>(usuario);

                usuarioDtoDb.Links.Add(new LinkDTO("_self", Url.Link("UsuarioObter", new { id = usuario.Id }), "GET"));
                usuarioDtoDb.Links.Add(new LinkDTO("_atualizar", Url.Link("UsuarioAtualizar", new { id = usuario.Id }), "PUT"));

                return Ok(usuarioDtoDb);
            }
            else
            {
                 var usuarioResult = _mapper.Map<ApplicationUser, UsuarioDtoSemHiperLink>(usuario);
                return Ok(usuario);
            }
        }

        

        [HttpPost("", Name = "UsuarioCadastrar")]
        public ActionResult Cadastrar([FromBody] UsuarioDTO usuarioDTO, [FromHeader(Name = "Accept")] string mediaType)
        {
            
            if (ModelState.IsValid)
            {
                ApplicationUser usuario = _mapper.Map<UsuarioDTO, ApplicationUser>(usuarioDTO);
                

                var usuarioDtoDb = _mapper.Map<ApplicationUser, UsuarioDTO>(usuario);
                var resultado = _userManager.CreateAsync(usuario, usuarioDTO.Senha).Result;

                if (!resultado.Succeeded)
                {
                    List<string> erros = new List<string>();
                    foreach (var erro in resultado.Errors)
                    {
                        erros.Add(erro.Description);

                    }
                    return UnprocessableEntity(erros);
                }
                else
                {
                    if (mediaType == CustomMediaType.Hetoas)
                    {
                        usuarioDtoDb.Links.Add(new LinkDTO("_self", Url.Link("UsuarioCadastrar", new { id = usuario.Id }), "POST"));
                        usuarioDtoDb.Links.Add(new LinkDTO("_obter", Url.Link("UsuarioObter", new { id = usuario.Id }), "GET"));
                        usuarioDtoDb.Links.Add(new LinkDTO("_atualizar", Url.Link("UsuarioAtualizar", new { id = usuario.Id }), "PUT"));

                        return Ok(usuarioDtoDb);
                    }
                    else
                    {
                        var usuarioResult = _mapper.Map<ApplicationUser, UsuarioDtoSemHiperLink>(usuario);
                        return Ok(usuario);
                    }
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        
        [HttpPut("{id}", Name = "UsuarioAtualizar")]
        [Authorize]
        public ActionResult Atualizar(string id, [FromBody] UsuarioDTO usuarioDTO, [FromHeader(Name = "Accept")] string mediaType)
        {
            ApplicationUser usuario = _userManager.GetUserAsync(HttpContext.User).Result;
            var usuarioDtoDb = _mapper.Map<ApplicationUser, UsuarioDTO>(usuario);

            if (usuario.Id != id)
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                
                usuario.FullName = usuarioDTO.Nome;
                usuario.Email = usuarioDTO.Email;
                usuario.UserName = usuarioDTO.Email;
                usuario.Slogan = usuarioDTO.Slogan;

                var resultado = _userManager.UpdateAsync(usuario).Result;
                _userManager.RemovePasswordAsync(usuario);
                _userManager.AddPasswordAsync(usuario, usuarioDTO.Senha);

                if (!resultado.Succeeded)
                {
                    List<string> erros = new List<string>();
                    foreach (var erro in resultado.Errors)
                    {
                        erros.Add(erro.Description);

                    }
                    return UnprocessableEntity(erros);
                }
                else
                {
                    if (mediaType == CustomMediaType.Hetoas)
                    {
                        usuarioDtoDb.Links.Add(new LinkDTO("_obter", Url.Link("UsuarioObter", new { id = usuario.Id }), "GET"));
                        usuarioDtoDb.Links.Add(new LinkDTO("_atualizar", Url.Link("UsuarioAtualizar", new { id = usuario.Id }), "PUT"));

                        return Ok(usuarioDtoDb);
                    }
                    else
                    {
                        var usuarioResult = _mapper.Map<ApplicationUser, UsuarioDtoSemHiperLink>(usuario);
                        return Ok(usuario);
                    }
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }


        [HttpPost("login")]
        public ActionResult Login([FromBody] UsuarioDTO usuarioDTO)
        {

            ModelState.Remove("Nome");
            ModelState.Remove("ConfimacaoSenha");

            if (ModelState.IsValid)
            {
                ApplicationUser usuario = _usuarioRepository.Obter(usuarioDTO.Email, usuarioDTO.Senha);

                if (usuario != null)
                {
                    return GerarToken(usuario);
                }
                else
                {
                    return NotFound("Usuario não localizado");
                }

            }
            else
            {
                return UnprocessableEntity(ModelState);
            }

        }


        [HttpPost("renovar")]
        public ActionResult Renovar([FromBody] TokenDTO tokenDTO)
        {
            var refreshTokenDb = _tokenRepository.Obter(tokenDTO.RefreshToken);

            if (refreshTokenDb == null)
                return NotFound();


            refreshTokenDb.Atualizado = DateTime.Now;
            refreshTokenDb.Utilizado = true;
            _tokenRepository.Atualizar(refreshTokenDb);

            var usuario = _usuarioRepository.Obter(refreshTokenDb.UsuarioId);

            return GerarToken(usuario);
        }

        private TokenDTO BuildToken(ApplicationUser usuario)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("chave-api-jwt-talkto");
            var exp = DateTime.UtcNow.AddHours(1);

            var claims = new[]
             {
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id),
            };

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var refreshToken = Guid.NewGuid().ToString();

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var expRefreshToken = DateTime.UtcNow.AddHours(2);

            var tokenDTO = new TokenDTO { Token = tokenString, Expiration = exp, ExpirationRefreshToken = expRefreshToken, RefreshToken = refreshToken };

            return tokenDTO;
        }
        private ActionResult GerarToken(ApplicationUser usuario)
        {
            var token = BuildToken(usuario);

            var tokenModel = new Token()
            {
                RefreshToken = token.RefreshToken,
                ExpirationToken = token.Expiration,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                Usuario = usuario,
                Criado = DateTime.Now,
                Utilizado = false,
            };
            _tokenRepository.Cadastrar(tokenModel);
            return Ok(token);
        }
    }
}
