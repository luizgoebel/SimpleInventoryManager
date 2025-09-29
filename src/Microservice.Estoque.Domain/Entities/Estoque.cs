using Shared.Domain.Entities;
using Shared.Domain.Exceptions;

namespace Microservice.Estoque.Domain.Entities;

public class Estoque : BaseModel<Estoque>
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }

    public void Validar()
    {
        if (this.ProdutoId <= 0) throw new DomainException("Não encontrado identificador do produto.");
        if (this.Quantidade < 0) throw new DomainException("Quantidade não pode ser menor que 0.");
    }

    public void AdicionarQuantidade(int quantidade)
    {
        this.Quantidade += quantidade;
    }

    public void SubtrairQuantidade(int quantidade)
    {
        this.Quantidade -= quantidade;
    }
}
