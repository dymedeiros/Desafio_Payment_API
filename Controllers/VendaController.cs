using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesafioPottencial.Context;
using DesafioPottencial.Models;
using Microsoft.AspNetCore.Mvc;

namespace DesafioPottencial.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendaController : ControllerBase
    {
        private readonly PagamentosContext _context;

        public VendaController(PagamentosContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var venda = _context.Vendas.Find(id);
            if(venda == null)
                return NotFound();

            var vendedor = _context.Vendedores.Find(venda.VendedorID);
            if(vendedor == null)
                return NotFound();

            var itens = _context.Itens.Where(i => i.VendaID == venda.Id).ToList();


            Facade.Venda.Response.Venda fVenda = new Facade.Venda.Response.Venda()
            {
                VendaID = venda.Id,
                Data = venda.Data,
                Status = venda.Status,
                Vendedor = new Facade.Venda.Vendedor()
                {
                    Nome = vendedor.Nome,
                    CPF = vendedor.CPF,
                    Email = vendedor.Email,
                    Telefone = vendedor.Telefone
                }
            };

            foreach(Item item in itens)
            {
                fVenda.Itens.Add(new Facade.Venda.Item()
                {
                    Nome = item.Nome,
                    Quantidade = item.Quantidade,
                    Valor = item.Valor
                });
            }
            

            return Ok(fVenda);
        }

        [HttpPost]
        public IActionResult Criar(Facade.Venda.Request.CriarVenda venda)
        {
            
            List<string> erros = new List<string>();
            if(venda != null){
                if(venda.Vendedor != null 
                    && venda.Vendedor.Nome.Length > 0 
                    && venda.Vendedor.CPF.Length > 0 
                    && venda.Vendedor.Telefone.Length > 0)
                {
                    if(venda.Itens.Count == 0)
                    {
                        erros.Add("Informe no mínimo um item vendido."); 
                    }
                }else{ 
                    erros.Add("Informe os dados do vendedor: nome, CPF, e-mail e telefone."); 
                }
            }else{ 
                erros.Add("Parâmetros não informados."); 
            }


            if(erros.Count > 0)
            {
                return BadRequest(new { Erro = string.Join(", ", erros) });
            }else{
                
                Venda vendaBanco = new Venda()
                {
                    Data = venda.Data,
                    Status = "Aguardando pagamento",
                    Vendedor = new Vendedor()
                    {
                        Nome = venda.Vendedor.Nome,
                        CPF = venda.Vendedor.CPF.Replace(" ","").Replace(".","").Replace("-",""),
                        Email = venda.Vendedor.Email,
                        Telefone = venda.Vendedor.Telefone
                    }
                };

                _context.Vendas.Add(vendaBanco);
                _context.SaveChanges();

                foreach(Facade.Venda.Item item in venda.Itens)
                {
                    _context.Itens.Add(new Item()
                    {
                        Nome = item.Nome,
                        Quantidade = item.Quantidade,
                        Valor = item.Valor,
                        VendaID = vendaBanco.Id
                    });
                }
                _context.SaveChanges();

                return Ok(vendaBanco.Id);
            }
        }

        [HttpPost("AlterarStatus")]
        public IActionResult AlterarStatus(Facade.Venda.Request.AlterarStatus req)
        {
            var venda = _context.Vendas.Find(req.VendaID);
            if(venda == null)
                return NotFound("Id não encontrado");

            switch(req.Status)
            {
                case "Pagamento aprovado":
                    if(!venda.Status.Equals("Aguardando pagamento"))
                    {
                        return BadRequest(new { Erro = "Sua venda precisa estar no status [Aguardando pagamento] para poder ser alterada para este novo status"});
                    }else{
                        venda.Status = req.Status;
                    }
                break;
                case "Enviado para a transportadora":
                    if(!venda.Status.Equals("Pagamento aprovado"))
                    {
                        return BadRequest(new { Erro = "Sua venda precisa estar no status [Pagamento aprovado] para poder ser alterada para este novo status"});
                    }else{
                        venda.Status = req.Status;
                    }
                break;
                case "Entregue":
                    if(!venda.Status.Equals("Enviado para a transportadora"))
                    {
                        return BadRequest(new { Erro = "Sua venda precisa estar no status [Enviado para a transportadora] para poder ser alterada para este novo status"});
                    }else{
                        venda.Status = req.Status;
                    }
                break;
                case "Cancelada":
                    if(venda.Status.Equals("Enviado para a transportadora"))
                    {
                        return BadRequest(new { Erro = "Como esta venda já está na transportadora, ela não pode ser cancelada"});
                    }else{
                        venda.Status = req.Status;
                    }
                break;
                default:
                    return BadRequest(new { Erro = "Informe um estatus válido: [Pagamento aprovado, Enviado para a transportadora, Entregue ou Cancelada]"});
            }

            _context.Vendas.Update(venda);
            _context.SaveChanges();

            return Ok(venda);
        }

    }
}