using AutoMapper;
using Microservice.Produto.Application.DTOs;

namespace Microservice.Produto.Application.Mapping;

public class ProdutoProfile : Profile
{
    public ProdutoProfile()
    {
        CreateMap<Microservice.Produto.Domain.Entities.Produto, ProdutoDto>();
        CreateMap<ProdutoCriacaoDto, Microservice.Produto.Domain.Entities.Produto>();
        CreateMap<ProdutoAtualizacaoDto, Microservice.Produto.Domain.Entities.Produto>();
    }
}
