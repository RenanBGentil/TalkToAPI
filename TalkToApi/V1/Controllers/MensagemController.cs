using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TalkToApi.Helpers.Constants;
using TalkToApi.V1.Models;
using TalkToApi.V1.Models.DTO;
using TalkToApi.V1.Repositories.Contracts;

namespace TalkToApi.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [DisableCors]
    public class MensagemController : ControllerBase
    {
        private readonly IMensagemRepository _mensagemRepository;
        private readonly IMapper _mapper;
        public MensagemController(IMensagemRepository mensagemRepository,IMapper mapper)
        {
            _mensagemRepository = mensagemRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("{usuarioUmId}/{usuarioDoisId}", Name ="MensagemObterTodos")]
        public ActionResult Obter(string usuarioUmId, string usuarioDoisId, [FromHeader(Name ="Accept")]string mediaType)
        {
            if(usuarioUmId == usuarioDoisId)
            {
                return UnprocessableEntity();
            }

            var mensagens = _mensagemRepository.ObterMensagens(usuarioUmId, usuarioDoisId);

            if (mediaType == CustomMediaType.Hetoas)
            {
               
                var listaMsg = _mapper.Map<List<Mensagem>, List<MensagemDTO>>(mensagens);
                var lista = new ListaDTO<MensagemDTO>() { Lista = listaMsg };
                lista.Links.Add(new LinkDTO("_self", Url.Link("MensagemObterTodos", new { usuarioUmId = usuarioUmId, usuarioDoisId = usuarioDoisId }), "GET"));

                return Ok(lista);
            }
            else
            {
                return Ok(mensagens);
            }
            
        }
        [Authorize]
        [HttpPost("", Name = "MensagemCadastrar")]
        public ActionResult Cadastrar([FromBody]Mensagem mensagem, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (mediaType == CustomMediaType.Hetoas)
                    {
                        var mensagemDto = _mapper.Map<Mensagem, MensagemDTO>(mensagem);
                        _mensagemRepository.Cadastrar(mensagem);
                        mensagemDto.Links.Add(new LinkDTO("_self", Url.Link("MensagemCadastrar", null), "POST"));
                        mensagemDto.Links.Add(new LinkDTO("_mensagemAtualizacaoParcial", Url.Link("MensagemAtualizacaoParcial", new { id = mensagem.Id }), "PATCH"));
                        return Ok(mensagemDto);
                    }
                    else
                    {
                        return Ok(mensagem);
                    }
                }
                catch (Exception e)
                {
                    return UnprocessableEntity(e);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        [Authorize]
        [HttpPatch("{id}", Name = "MensagemAtualizacaoParcial")]
        public ActionResult AtualizacaoParcial(int id, [FromBody]JsonPatchDocument<Mensagem> jsonPatch, [FromHeader(Name = "Accept")] string mediaType)
        {
            if(jsonPatch == null)
            {
                return BadRequest();
            }

           var mensagem = _mensagemRepository.Obter(id);

            jsonPatch.ApplyTo(mensagem);
            mensagem.Atualizado = DateTime.UtcNow;
            _mensagemRepository.Atualizar(mensagem);
            var mensagemDto = _mapper.Map<Mensagem, MensagemDTO>(mensagem);

            if (mediaType == CustomMediaType.Hetoas)
            {
                mensagemDto.Links.Add(new LinkDTO("_self", Url.Link("MensagemAtualizacaoParcial", new { id = mensagem.Id }), "PATCH"));
                return Ok(mensagemDto);
            }
            else
            {
                return Ok(mensagem);
            }
          
        }
    }
}
