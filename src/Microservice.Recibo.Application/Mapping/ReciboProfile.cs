using AutoMapper;
using Microservice.Recibo.Application.DTOs;

namespace Microservice.Recibo.Application.Mapping;

public class ReciboProfile : Profile
{
    public ReciboProfile()
    {
        CreateMap<Domain.Entities.Recibo, ReciboDto>()
            .ConstructUsing(r => new ReciboDto(r.Id, r.Numero, r.FaturaId, r.DataEmissao, r.ValorTotal));
    }
}
