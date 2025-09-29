using AutoMapper;
using Microservice.Pedido.Application.DTOs;
using Microservice.Pedido.Domain.Entities;
using DomainPedido = Microservice.Pedido.Domain.Entities.Pedido;

namespace Microservice.Pedido.Application.Mapping;

public class PedidoProfile : Profile
{
    public PedidoProfile()
    {
        CreateMap<PedidoItem, PedidoItemDto>()
            .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.Subtotal));
        CreateMap<DomainPedido, PedidoDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Itens, o => o.MapFrom(s => s.Itens));
    }
}
