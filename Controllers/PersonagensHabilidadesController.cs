using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [Route("[controller]")]
    public class PersonagemHabilidadeController : Controller
    {
    private readonly DataContext _context;

    public PersonagemHabilidadeController(DataContext context) 
    {
        _context = context;
    }  

    [HttpPost]

    public async Task<IActionResult> AddPersonagemHabilidadeAsync(PersonagemHabilidade novoPersonagemHabilidade)
    {
        try
        {
                Personagem personagem = await _context.TB_PERSONAGENS
                    .Include(p => p.Arma)
                    .Include(p => p.PersonagemHabilidades).ThenInclude(ps => ps.Habilidade)
                    .FirstOrDefaultAsync(p => p.Id == novoPersonagemHabilidade.PersonagemId);

                    if (personagem == null)
                    throw new System.Exception("Personagem não encontrado para o Id Informado");

                Habilidade habilidade = await _context.TB_HABILIDADES
                                    .FirstOrDefaultAsync(h => h.Id == novoPersonagemHabilidade.HabilidadeId);
                if(habilidade == null)
                    throw new System.Exception("Habilidade não encontrado. ");


                PersonagemHabilidade ph = new PersonagemHabilidade();
                ph.Personagem = personagem;
                ph.Habilidade = habilidade;

                await _context.TB_PERSONAGENS_HABILIDADES.AddAsync(ph);
                int linhasAfetadas = await _context.SaveChangesAsync();

                return Ok(linhasAfetadas);                     
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("ListarPorPersonagem/{personagemId}")]
        public async Task<IActionResult> ListarPorPersonagem(int personagemId)
        {
            try
            {
                var habilidadesDoPersonagem = await _context.TB_PERSONAGENS_HABILIDADES
                    .Include(ph => ph.Habilidade)
                    .Where(ph => ph.PersonagemId == personagemId)
                    .ToListAsync();

                return Ok(habilidadesDoPersonagem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ListarPorHabilidade/{HabilidadeId}")]
        public async Task<IActionResult> ListarPorHabilidade(int HabilidadeId)
        {
            try
            {
                var habilidadesDoPersonagem = await _context.TB_PERSONAGENS_HABILIDADES
                    .Include(ph => ph.Habilidade)
                    .Where(ph => ph.HabilidadeId == HabilidadeId)
                    .ToListAsync();

                return Ok(habilidadesDoPersonagem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("DeletePersonagemHabilidade")]
        public async Task<IActionResult> DeletePersonagemHabilidade(PersonagemHabilidadeDTO personagemHabilidadeDto)
        {
            try
            {
                // Encontra a PersonagemHabilidade a ser removida
                var personagemHabilidade = await _context.TB_PERSONAGENS_HABILIDADES
                    .FirstOrDefaultAsync(ph => ph.PersonagemId == personagemHabilidadeDto.PersonagemId &&
                                                ph.HabilidadeId == personagemHabilidadeDto.HabilidadeId);

                if (personagemHabilidade == null)
                    return NotFound("PersonagemHabilidade não encontrada");

                _context.TB_PERSONAGENS_HABILIDADES.Remove(personagemHabilidade);
                await _context.SaveChangesAsync();

                return Ok("PersonagemHabilidade removida com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}