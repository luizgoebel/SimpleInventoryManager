using AutoMapper;
using Microservice.Recibo.Application.DTOs;
using Microservice.Recibo.Application.Interfaces;
using Microservice.Recibo.Application.Mapping;
using Microservice.Recibo.Application.Services;
using Shared.Application.Exceptions;

namespace Microservice.Recibo.Application.Tests.ReciboServiceTests;

public class ReciboServiceTests
{
    private Mock<IReciboRepository> _repo = null!;
    private IMapper _mapper = null!;
    private ReciboService _service = null!;

    [SetUp]
    public void SetUp()
    {
        this._repo = new Mock<IReciboRepository>(MockBehavior.Strict);
        MapperConfiguration cfg = new MapperConfiguration(c => c.AddProfile<ReciboProfile>());
        this._mapper = cfg.CreateMapper();
        this._service = new ReciboService(this._repo.Object, this._mapper);
    }

    [Test]
    public void GerarPorFaturaAsync_JaExiste_DeveLancarServiceException()
    {
        this._repo.Setup(r => r.GetByFaturaIdAsync(1)).ReturnsAsync(new Domain.Entities.Recibo(1, "RC-1", 10m));
        Assert.ThrowsAsync<ServiceException>(() => this._service.GerarPorFaturaAsync(1, "FT-1", 10m));
        this._repo.Verify(r => r.GetByFaturaIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GerarPorFaturaAsync_Valido_DeveGerar()
    {
        int faturaId = 2;
        this._repo.Setup(r => r.GetByFaturaIdAsync(faturaId)).ReturnsAsync((Domain.Entities.Recibo?)null);
        this._repo.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Recibo>())).Returns(Task.CompletedTask);

        ReciboDto dto = await this._service.GerarPorFaturaAsync(faturaId, "FT-2", 10m);

        this._repo.Verify(r => r.GetByFaturaIdAsync(faturaId), Times.Once);
        this._repo.Verify(r => r.AddAsync(It.Is<Domain.Entities.Recibo>(r => r.FaturaId == faturaId)), Times.Once);
        Assert.That(dto.ValorTotal, Is.EqualTo(10m));
    }

    [Test]
    public void ObterPorFaturaAsync_Inexistente_DeveLancarServiceException()
    {
        this._repo.Setup(r => r.GetByFaturaIdAsync(3)).ReturnsAsync((Domain.Entities.Recibo?)null);
        Assert.ThrowsAsync<ServiceException>(() => this._service.ObterPorFaturaAsync(3));
        this._repo.Verify(r => r.GetByFaturaIdAsync(3), Times.Once);
    }

    [Test]
    public async Task ObterPorFaturaAsync_Existente_DeveRetornar()
    {
        int faturaId = 4;
        Domain.Entities.Recibo recibo = new Domain.Entities.Recibo(faturaId, "RC-1", 10m);
        this._repo.Setup(r => r.GetByFaturaIdAsync(faturaId)).ReturnsAsync(recibo);
        ReciboDto? dto = await this._service.ObterPorFaturaAsync(faturaId);
        Assert.That(dto, Is.Not.Null);
        this._repo.Verify(r => r.GetByFaturaIdAsync(faturaId), Times.Once);
    }
}
