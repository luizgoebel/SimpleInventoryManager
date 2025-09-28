using Shared.Domain.Entities;

namespace Microservice.Estoque.Domain.Entities;

public class Estoque : BaseModel<Estoque>
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}
