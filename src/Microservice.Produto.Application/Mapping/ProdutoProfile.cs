using AutoMapper;
using Microservice.Produto.Application.DTOs;
using Microservice.Produto.Domain.Entities;

namespace Microservice.Produto.Application.Mapping;

public class ProdutoProfile : Profile
{
    public ProdutoProfile()
    {
        CreateMap<Produto, ProdutoDto>();
        CreateMap<ProdutoCriacaoDto, Produto>();
        CreateMap<ProdutoAtualizacaoDto, Produto>();
    }
}
