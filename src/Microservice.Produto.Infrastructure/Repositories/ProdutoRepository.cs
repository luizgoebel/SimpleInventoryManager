using Microservice.Produto.Application.Interfaces;
using Microservice.Produto.Domain.Entities;
using Microservice.Produto.Infrastructure.Data.Context;

namespace Microservice.Produto.Infrastructure.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(ProdutoDbContext ctx) : base(ctx) { }
}
