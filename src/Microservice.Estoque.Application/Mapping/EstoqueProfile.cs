using AutoMapper;
using Microservice.Estoque.Application.DTOs;

namespace Microservice.Estoque.Application.Mapping;

public class EstoqueProfile : Profile
{
    public EstoqueProfile()
    {
        CreateMap<Microservice.Estoque.Domain.Entities.Estoque, EstoqueDto>();
    }
}
