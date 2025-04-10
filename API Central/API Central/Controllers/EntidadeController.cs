﻿using DataBase.Data;
using Microsoft.AspNetCore.Mvc;
using Modelos.EF.Entidade;
using Modelos.ModelosRequest.Entidade;
using System.ComponentModel.DataAnnotations;

namespace API_Central.Controllers
{
    /// <summary>
    /// Controlador para gerenciar entidades.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EntidadeController : ControllerBase
    {
        private readonly DAL<EntidadeModel> _dalEntidade;

        /// <summary>
        /// Construtor do controlador EntidadeController.
        /// </summary>
        /// <param name="dalEntidade">Dependência de acesso a dados para EntidadeModel.</param>
        public EntidadeController(DAL<EntidadeModel> dalEntidade)
        {
            _dalEntidade = dalEntidade;
        }

        /// <summary>
        /// Cria uma nova entidade.
        /// </summary>
        /// <param name="entidade">Dados da entidade a ser criada.</param>
        /// <returns>A entidade criada.</returns>
        [HttpPost]
        public async Task<ActionResult<EntidadeModel>> CriarEntidade([FromBody] EntidadeModel entidade)
        {
            try
            {
                // Adicionar a entidade a base de dados
                await _dalEntidade.AdicionarAsync(entidade);

                // Retorna o objeto adicionado
                return entidade;
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ValidationException ex)
            {
                // Retorna um erro de validação com o detalhe da mensagem
                return BadRequest($"Erro de validação: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Retorna um erro genérico com a mensagem da exceção
                return StatusCode(500, $"Erro ao tentar adicionar a entidade. {ex.Message}");
            }
        }

        /// <summary>
        /// Busca todas as entidades.
        /// </summary>
        /// <returns>Lista de entidades.</returns>
        [HttpGet("BuscarTodos")]
        public async Task<ActionResult<IEnumerable<EntidadeModel>>> BuscarTodos()
        {
            try
            {
                IEnumerable<EntidadeModel?> entidades = await _dalEntidade.ListarAsync();
                return Ok(entidades);
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Retorna um erro genérico com a mensagem da exceção
                return StatusCode(500, $"Erro ao tentar buscar as entidades. {ex.Message}");
            }
        }

        /// <summary>
        /// Busca uma entidade pelo ID.
        /// </summary>
        /// <param name="id">ID da entidade.</param>
        /// <returns>A entidade encontrada.</returns>
        [HttpGet("BuscarPorId/{id}")]
        public async Task<ActionResult<EntidadeModel>> BuscarPorId(int id)
        {
            try
            {
                EntidadeModel? entidade = await _dalEntidade.BuscarPorAsync(c => c.Id.Equals(id));

                // Retorna 404 Not Found se a entidade não for encontrada
                if (entidade is null) return NotFound($"Não foi encontrada nenhuma entidade com este ID {id}.");

                return Ok(entidade);
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Retorna um erro genérico com a mensagem da exceção
                return StatusCode(500, $"Erro ao tentar buscar a entidade. {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza uma entidade pelo ID.
        /// </summary>
        /// <param name="entidade">Dados da entidade a ser atualizada.</param>
        /// <param name="id">ID da entidade.</param>
        /// <returns>A entidade atualizada.</returns>
        [HttpPut("AtualizarPorId/{id}")]
        public async Task<ActionResult<EntidadeModel>> AtualizarPorId([FromBody] AtualizarEntidade atualizarEntidade, int id)
        {
            try
            {
                EntidadeModel? entidadeExistente = await _dalEntidade.RecuperarPorAsync(x => x.Id.Equals(id));
                if (entidadeExistente is null) return BadRequest($"Entidade não encontrada no banco de Dados!");

                // Atualizar os campos da entidade existente com os novos dados
                entidadeExistente.Nome = atualizarEntidade.Nome;
                entidadeExistente.Endereco = atualizarEntidade.Endereco;
                entidadeExistente.Telefone = atualizarEntidade.Telefone;
                entidadeExistente.Situacao = atualizarEntidade.Situacao;
                entidadeExistente.TipoEntidade = atualizarEntidade.Tipo_Entidade;

                // Chama o método DAL para atualizar a entidade no banco de dados
                await _dalEntidade.AtualizarAsync(entidadeExistente);

                // Retorna a entidade atualizada dentro de um Ok()
                return Ok(entidadeExistente);
            }
            catch (ValidationException ex)
            {
                // Retorna um erro de validação com o detalhe da mensagem
                return BadRequest($"Erro de validação: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Retorna um erro genérico com a mensagem da exceção
                return StatusCode(500, $"Erro ao tentar atualizar a entidade. {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza o tipo de uma entidade pelo ID.
        /// </summary>
        /// <param name="tipoEntidade">Dados para atualizar o tipo da entidade.</param>
        /// <param name="id">ID da entidade.</param>
        /// <returns>A entidade atualizada.</returns>
        [HttpPut("AtualizarTipo/{id}")]
        public async Task<ActionResult<EntidadeModel>> AtualizarTipo([FromBody] AtualizarTipoEntidade tipoEntidade, int id)
        {
            try
            {
                EntidadeModel? entidadeExistente = await _dalEntidade.RecuperarPorAsync(x => x.Id.Equals(tipoEntidade.Id));
                if (entidadeExistente is null) return BadRequest($"Entidade não encontrada no banco de Dados!");

                // Atualizar o tipo da entidade existente com o novo tipo
                entidadeExistente.TipoEntidade = tipoEntidade.TipoEntidade;

                // Chama o método DAL para atualizar a entidade no banco de dados
                await _dalEntidade.AtualizarAsync(entidadeExistente);

                // Retorna a entidade atualizada dentro de um Ok()
                return Ok(entidadeExistente);
            }
            catch (ValidationException ex)
            {
                // Retorna um erro de validação com o detalhe da mensagem
                return BadRequest($"Erro de validação: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Retorna um erro genérico com a mensagem da exceção
                return StatusCode(500, $"Erro ao tentar atualizar o tipo da entidade. {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza a situação de uma entidade pelo ID.
        /// </summary>
        /// <param name="atualizarSituacaoEntidade">dados para atualizar a situação da entidade.</param>
        /// <param name="id">ID da entidade.</param>
        /// <returns>A entidade atualizada.</returns
        [HttpPut("AtualizarSituacao/{id}")]
        public async Task<ActionResult<EntidadeModel>> AtualizarSituacao([FromBody] AtualizarSituacaoEntidade entidadeRequest, int id)
        {
            try
            {
                EntidadeModel? entidadeExistente = await _dalEntidade.RecuperarPorAsync(x => x.Id.Equals(entidadeRequest.Id));
                if (entidadeExistente is null) return BadRequest($"Entidade não encontrada no banco de Dados!");

                // Atualizar o tipo da entidade existente com o novo tipo
                entidadeExistente.Situacao = entidadeRequest.Situacao;

                // Chama o método DAL para atualizar a entidade no banco de dados
                await _dalEntidade.AtualizarAsync(entidadeExistente);

                // Retorna a entidade atualizada dentro de um Ok()
                return Ok(entidadeExistente);
            }
            catch (ValidationException ex)
            {
                // Retorna um erro de validação com o detalhe da mensagem
                return BadRequest($"Erro de validação: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Retorna um erro genérico com a mensagem da exceção
                return StatusCode(500, $"Erro ao tentar atualizar o tipo da entidade. {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza o nome de uma entidade pelo ID.
        /// </summary>
        /// <param name="entidadeRequest">Dados para atualizar o nome da entidade.</param>
        /// <param name="id">ID da entidade.</param>
        /// <returns>A entidade atualizada.</returns>
        [HttpPut("AtualizarNome/{id}")]
        public async Task<ActionResult<EntidadeModel>> AtualizarNome([FromBody] AtualizarNomeEntidade entidadeRequest, int id)
        {
            try
            {
                EntidadeModel? entidadeExistente = await _dalEntidade.RecuperarPorAsync(x => x.Nome.Equals(entidadeRequest.Id));
                if (entidadeExistente is null) return BadRequest($"Entidade não encontrada no banco de Dados!");

                // Atualizar o nome da entidade existente com o novo nome
                entidadeExistente.Nome = entidadeRequest.Nome;

                // Chama o método DAL para atualizar a entidade no banco de dados
                await _dalEntidade.AtualizarAsync(entidadeExistente);

                // Retorna a entidade atualizada dentro de um Ok()
                return Ok(entidadeExistente);
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Erro de validação: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao tentar atualizar o nome da entidade. {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza o endereço de uma entidade pelo ID.
        /// </summary>
        /// <param name="entidadeRequest">Dados para atualizar o endereço da entidade.</param>
        /// <param name="id">ID da entidade.</param>
        /// <returns>A entidade atualizada.</returns>
        [HttpPut("AtualizarEndereco/{id}")]
        public async Task<ActionResult<EntidadeModel>> AtualizarEndereco([FromBody] AtualizarEnderecoEntidade entidadeRequest, int id)
        {
            try
            {
                EntidadeModel? entidadeExistente = await _dalEntidade.RecuperarPorAsync(x => x.Id.Equals(entidadeRequest.Id));
                if (entidadeExistente is null) return BadRequest($"Entidade não encontrada no banco de Dados!");

                // Atualizar o endereço da entidade existente com o novo endereço
                entidadeExistente.Endereco = entidadeRequest.Endereco;

                // Chama o método DAL para atualizar a entidade no banco de dados
                await _dalEntidade.AtualizarAsync(entidadeExistente);

                // Retorna a entidade atualizada dentro de um Ok()
                return Ok(entidadeExistente);
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Erro de validação: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao tentar atualizar o endereço da entidade. {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza o telefone de uma entidade pelo ID.
        /// </summary>
        /// <param name="entidadeRequest">Dados para atualizar o telefone da entidade.</param>
        /// <param name="id">ID da entidade.</param>
        /// <returns>A entidade atualizada.</returns>
        [HttpPut("AtualizarTelefone/{id}")]
        public async Task<ActionResult<EntidadeModel>> AtualizarTelefone([FromBody] AtualizarTelefoneEntidade entidadeRequest, int id)
        {
            try
            {
                EntidadeModel? entidadeExistente = await _dalEntidade.RecuperarPorAsync(x => x.Id.Equals(entidadeRequest.Id));
                if (entidadeExistente is null) return BadRequest($"Entidade não encontrada no banco de Dados!");

                // Atualizar o telefone da entidade existente com o novo telefone
                entidadeExistente.Telefone = entidadeRequest.Telefone;

                // Chama o método DAL para atualizar a entidade no banco de dados
                await _dalEntidade.AtualizarAsync(entidadeExistente);

                // Retorna a entidade atualizada dentro de um Ok()
                return Ok(entidadeExistente);
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Erro de validação: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao tentar atualizar o telefone da entidade. {ex.Message}");
            }
        }

        /// <summary>
        /// Remove uma entidade pelo ID.
        /// </summary>
        /// <param name="id">ID da entidade.</param>
        /// <returns>Resultado da remoção.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Remover(int id)
        {
            try
            {
                // Primeiro, recupera a entidade existente pelo ID
                EntidadeModel? entidadeExistente = await _dalEntidade.RecuperarPorAsync(c => c.Id.Equals(id));

                // Retorna 404 Not Found se a entidade não existir
                if (entidadeExistente is null) return NotFound();

                try
                {
                    // Chama o método do DAL para remover a entidade
                    await _dalEntidade.DeletarAsync(entidadeExistente);

                    // Retorna 204 No Content se a remoção foi bem-sucedida
                    return NoContent();
                }
                catch (Exception ex)
                {
                    // Retorna um erro genérico ou detalhado se a remoção falhar
                    return StatusCode(500, $"Erro ao tentar remover a entidade. {ex.Message}");
                }
            }
            catch (ValidationException ex)
            {
                // Retorna um erro de validação com o detalhe da mensagem
                return BadRequest($"Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Retorna um erro genérico com a mensagem da exceção
                return StatusCode(500, $"Erro ao tentar remover a entidade. {ex.Message}");
            }
        }
    }
}
