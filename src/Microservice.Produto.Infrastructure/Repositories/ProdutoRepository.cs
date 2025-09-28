using Microservice.Produto.Application.Interfaces;
using Microservice.Produto.Infrastructure.Data.Context;

namespace Microservice.Produto.Infrastructure.Repositories;

public class ProdutoRepository : Repository<Microservice.Produto.Domain.Entities.Produto>, IProdutoRepository
{
    public ProdutoRepository(ProdutoDbContext ctx) : base(ctx) { }
}
