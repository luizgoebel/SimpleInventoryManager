using AutoMapper;
using Microservice.Estoque.Application.DTOs;
using Microservice.Estoque.Domain.Entities;

namespace Microservice.Estoque.Application.Mapping;

public class EstoqueProfile : Profile
{
    public EstoqueProfile()
    {
        CreateMap<Estoque, EstoqueDto>();
    }
}
