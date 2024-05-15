using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArmaController : ControllerBase
    {

        private readonly DataContext _context; // context é usado para visualizar uma variável global



        public ArmaController(DataContext context)
        {
            _context = context;

        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Arma> lista = await _context.TB_ARMAS.ToListAsync();
                return Ok(lista);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpPost]

        public async Task<IActionResult> Add(Arma novaArma)
        {
            try
            {
                    if(novaArma.Dano == 0)
                    throw new  Exception("O dano da arma não pode ser 0.");

                    Personagem p = await _context.TB_PERSONAGENS
                        .FirstOrDefaultAsync(p => p.Id == novaArma.PersonagemId);

                    if(p == null)
                            throw   new Exception("Não existe personagem com o ID informado");

                    Arma buscaArma = await _context.TB_ARMAS
                    .FirstOrDefaultAsync(A => A.PersonagemId == novaArma.PersonagemId);

                    if (buscaArma != null)
                    throw new Exception ("O personagem selecionado já contem uma arma atribuída a ele");

                    
                await _context.TB_ARMAS.AddAsync(novaArma);
                await _context.SaveChangesAsync();

                return Ok(novaArma.Id);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")] //Buscar pelo ID
        public async Task<IActionResult> GetSingle(int id)
        {
            try
            {

                Arma ar = await _context.TB_ARMAS
                        .FirstOrDefaultAsync(arBusca => arBusca.Id == id);

                return Ok(ar);

            }
            catch (System.Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpPut]
        public async Task<IActionResult> Uptade(Arma novaArma)
        {
            try
            {

                _context.TB_ARMAS.Update(novaArma);
                int linhasAfetadas = await _context.SaveChangesAsync();

                return Ok(linhasAfetadas);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            Arma aRemover = await _context.TB_ARMAS.FirstOrDefaultAsync(a => a.Id == id);

            _context.TB_ARMAS.Remove(aRemover);
            int linhasAfetadas = await _context.SaveChangesAsync();
            return Ok(linhasAfetadas);

        }

        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    }
}