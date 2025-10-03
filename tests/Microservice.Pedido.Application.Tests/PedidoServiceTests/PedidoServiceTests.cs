using AutoMapper;
using Microservice.Pedido.Application.DTOs;
using Microservice.Pedido.Application.Interfaces;
using Microservice.Pedido.Application.Mapping;
using Microservice.Pedido.Application.Services;
using Shared.Application.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Shared.Messaging; // novo
using System.Threading; // adiciona cancellation token

namespace Microservice.Pedido.Application.Tests.PedidoServiceTests
{
    public class PedidoServiceTests
    {
        private readonly Mock<IPedidoRepository> _repo = new Mock<IPedidoRepository>();
        private readonly Mock<IEstoqueMovimentoClient> _estoqueClient = new Mock<IEstoqueMovimentoClient>();
        private readonly Mock<IEventBus> _eventBus = new Mock<IEventBus>(); // novo
        private readonly IMapper _mapper;
        private readonly PedidoService _service;

        public PedidoServiceTests()
        {
            MapperConfiguration cfg = new MapperConfiguration(c => c.AddProfile<PedidoProfile>());
            this._mapper = cfg.CreateMapper();
            this._service = new PedidoService(this._repo.Object, this._mapper, this._estoqueClient.Object, _eventBus.Object); // adiciona eventBus
        }

        [SetUp]
        public void SetUp()
        {
            this._repo.Invocations.Clear();
            this._estoqueClient.Invocations.Clear();
            this._eventBus.Invocations.Clear(); // limpar
        }

        [Test]
        public void CriarAsync_SemItens_DeveLancarServiceException()
        {
            PedidoCriacaoDto dto = new PedidoCriacaoDto { Itens = new List<PedidoItemCriacaoDto>() };
            Assert.ThrowsAsync<ServiceException>(() => this._service.CriarAsync(dto));
        }

        [Test]
        public void CriarAsync_ItemProdutoInvalido_DeveLancarServiceException()
        {
            PedidoCriacaoDto dto = new PedidoCriacaoDto
            {
                Itens = new List<PedidoItemCriacaoDto>
                {
                    new PedidoItemCriacaoDto { ProdutoId = 0, Quantidade = 1, PrecoUnitario = 10m }
                }
            };
            Assert.ThrowsAsync<ServiceException>(() => this._service.CriarAsync(dto));
        }

        [Test]
        public void CriarAsync_ItemQuantidadeInvalida_DeveLancarServiceException()
        {
            PedidoCriacaoDto dto = new PedidoCriacaoDto
            {
                Itens = new List<PedidoItemCriacaoDto>
                {
                    new PedidoItemCriacaoDto { ProdutoId = 1, Quantidade = 0, PrecoUnitario = 10m }
                }
            };
            Assert.ThrowsAsync<ServiceException>(() => this._service.CriarAsync(dto));
        }

        [Test]
        public void CriarAsync_ItemPrecoInvalido_DeveLancarServiceException()
        {
            PedidoCriacaoDto dto = new PedidoCriacaoDto
            {
                Itens = new List<PedidoItemCriacaoDto>
                {
                    new PedidoItemCriacaoDto { ProdutoId = 1, Quantidade = 2, PrecoUnitario = -1m }
                }
            };
            Assert.ThrowsAsync<ServiceException>(() => this._service.CriarAsync(dto));
        }

        [Test]
        public async Task CriarAsync_ItensValidos_DeveCriarPedidoEChamarEstoqueEPublicarEvento()
        {
            PedidoCriacaoDto dto = new PedidoCriacaoDto
            {
                Itens = new List<PedidoItemCriacaoDto>
                {
                    new PedidoItemCriacaoDto { ProdutoId = 1, Quantidade = 2, PrecoUnitario = 10m },
                    new PedidoItemCriacaoDto { ProdutoId = 2, Quantidade = 1, PrecoUnitario = 5m }
                }
            };
            PedidoDto resp = await this._service.CriarAsync(dto);
            Assert.That(resp.Itens.Count, Is.EqualTo(2));
            this._repo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Pedido>()), Times.Once);
            this._estoqueClient.Verify(c => c.RegistrarSaidaAsync(1, 2), Times.Once);
            this._estoqueClient.Verify(c => c.RegistrarSaidaAsync(2, 1), Times.Once);
            this._eventBus.Verify(b => b.PublishAsync(It.IsAny<Shared.Messaging.IntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once); // verifica publicação
        }

        [Test]
        public void GetByIdAsync_Inexistente_DeveLancarServiceException()
        {
            this._repo.Setup(r => r.GetByIdWithItemsAsync(1))
                .ReturnsAsync((Domain.Entities.Pedido)null);
            Assert.ThrowsAsync<ServiceException>(() => this._service.GetByIdAsync(1));
        }

        [Test]
        public async Task GetByIdAsync_Existente_DeveRetornarDto()
        {
            Domain.Entities.Pedido pedido = new Domain.Entities.Pedido();
            pedido.AdicionarItem(1, 1, 10m);
            this._repo.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync(pedido);
            PedidoDto? dto = await this._service.GetByIdAsync(1);
            Assert.That(dto, Is.Not.Null);
            Assert.That(dto!.Itens.Count, Is.EqualTo(1));
        }

        [Test]
        public void CancelarAsync_Inexistente_DeveLancarServiceException()
        {
            this._repo.Setup(r => r.GetByIdWithItemsAsync(1))
                .ReturnsAsync((Domain.Entities.Pedido)null);
            Assert.ThrowsAsync<ServiceException>(() => this._service.CancelarAsync(1));
        }

        [Test]
        public async Task CancelarAsync_Pendente_DeveCancelar()
        {
            Domain.Entities.Pedido pedido = new Domain.Entities.Pedido();
            pedido.AdicionarItem(1, 1, 10m);
            this._repo.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync(pedido);
            bool ok = await this._service.CancelarAsync(1);
            Assert.That(ok, Is.True);
            this._repo.Verify(r => r.UpdateAsync(It.Is<Domain.Entities.Pedido>(p => p.Status.ToString() == "Cancelado")), Times.Once);
        }

        [Test]
        public async Task CancelarAsync_JaCancelado_NaoAltera()
        {
            Domain.Entities.Pedido pedido = new Domain.Entities.Pedido();
            pedido.AdicionarItem(1, 1, 10m);
            pedido.Cancelar();
            this._repo.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync(pedido);
            bool ok = await this._service.CancelarAsync(1);
            Assert.That(ok, Is.True);
            this._repo.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Pedido>()), Times.Once);
        }
    }
}
