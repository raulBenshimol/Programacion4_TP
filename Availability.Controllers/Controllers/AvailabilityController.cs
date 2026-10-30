using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Application.UseCases;
using Availability.Controllers.DTOs;
using Availability.Controllers.Presenters;

namespace Availability.Controllers.Controllers;

public class AvailabilityController
{
    private readonly ConsultarDisponibilidadUseCase _consultarDisponibilidad;
    private readonly SolicitarReservaInteractor _solicitarReserva;
    private readonly SolicitudPresenter _presenter;

    public AvailabilityController(
        ConsultarDisponibilidadUseCase consultarDisponibilidad,
        SolicitarReservaInteractor solicitarReserva,
        SolicitudPresenter presenter)
    {
        _consultarDisponibilidad = consultarDisponibilidad;
        _solicitarReserva = solicitarReserva;
        _presenter = presenter;
    }

    // Este método simula el GET de la API
    public async Task<List<AsientoDto>> ConsultarAsientos(int eventoId)
    {
        var asientos = await _consultarDisponibilidad.Ejecutar(eventoId);

        // Mapeo manual (Entidad a DTO)
        return asientos.Select(a => new AsientoDto
        {
            Id = a.Id,
            Codigo = a.Codigo,
            Estado = a.Estado.ToString()
        }).ToList();
    }

    // Este método simula el POST de la API
    public async Task<SolicitudResponseDto> SolicitarReserva(SolicitudRequestDto request)
    {
        // 1. Ejecutamos el interactor
        await _solicitarReserva.Ejecutar(request.AsientoId, request.UsuarioId);

        // 2. Devolvemos lo que el Presenter armó
        return _presenter.Response;
    }
}