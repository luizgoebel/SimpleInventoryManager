using Shared.Domain.Entities;

namespace Microservice.Produto.Domain.Entities;

public class Produto : BaseModel<Produto>
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string? Descricao { get; set; }
}
