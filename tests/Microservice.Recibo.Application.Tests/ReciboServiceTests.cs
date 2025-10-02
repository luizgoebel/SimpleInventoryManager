using AutoMapper;
using Microservice.Recibo.Application.Interfaces;
using Microservice.Recibo.Application.Mapping;
using Microservice.Recibo.Application.Services;
using Shared.Application.Exceptions;

namespace Microservice.Recibo.Application.Tests;

public class ReciboServiceTests
{
    private Mock<IReciboRepository> _repo = null!;
    private IMapper _mapper = null!;
    private ReciboService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IReciboRepository>(MockBehavior.Strict);
        var cfg = new MapperConfiguration(c => c.AddProfile<ReciboProfile>());
        _mapper = cfg.CreateMapper();
        _service = new ReciboService(_repo.Object, _mapper);
    }

    [Test]
    public void GerarPorFaturaAsync_JaExiste_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByFaturaIdAsync(1)).ReturnsAsync(new Microservice.Recibo.Domain.Entities.Recibo(1, "RC-1", 10m));
        Assert.ThrowsAsync<ServiceException>(() => _service.GerarPorFaturaAsync(1, "FT-1", 10m));
        _repo.Verify(r => r.GetByFaturaIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GerarPorFaturaAsync_Valido_DeveGerar()
    {
        // Usar outro id para garantir isolamento lógico mesmo que algum mock reste (defensivo)
        int faturaId = 2;
        _repo.Setup(r => r.GetByFaturaIdAsync(faturaId)).ReturnsAsync((Microservice.Recibo.Domain.Entities.Recibo?)null);
        _repo.Setup(r => r.AddAsync(It.IsAny<Microservice.Recibo.Domain.Entities.Recibo>())).Returns(Task.CompletedTask);

        var dto = await _service.GerarPorFaturaAsync(faturaId, "FT-2", 10m);

        _repo.Verify(r => r.GetByFaturaIdAsync(faturaId), Times.Once);
        _repo.Verify(r => r.AddAsync(It.Is<Microservice.Recibo.Domain.Entities.Recibo>(r => r.FaturaId == faturaId)), Times.Once);
        Assert.That(dto.ValorTotal, Is.EqualTo(10m));
    }

    [Test]
    public void ObterPorFaturaAsync_Inexistente_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByFaturaIdAsync(3)).ReturnsAsync((Microservice.Recibo.Domain.Entities.Recibo?)null);
        Assert.ThrowsAsync<ServiceException>(() => _service.ObterPorFaturaAsync(3));
        _repo.Verify(r => r.GetByFaturaIdAsync(3), Times.Once);
    }

    [Test]
    public async Task ObterPorFaturaAsync_Existente_DeveRetornar()
    {
        int faturaId = 4;
        var recibo = new Microservice.Recibo.Domain.Entities.Recibo(faturaId, "RC-1", 10m);
        _repo.Setup(r => r.GetByFaturaIdAsync(faturaId)).ReturnsAsync(recibo);
        var dto = await _service.ObterPorFaturaAsync(faturaId);
        Assert.That(dto, Is.Not.Null);
        _repo.Verify(r => r.GetByFaturaIdAsync(faturaId), Times.Once);
    }
}
