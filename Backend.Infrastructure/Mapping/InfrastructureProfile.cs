using AutoMapper;
using Backend.Data.Entities;
//using Backend.Models.DTO;
//using Backend.Models.Models;

namespace Backend.Infrastructure.Mapping
{
    public class InfrastructureProfile : Profile
    {
        #region Constructor
        /*public InfrastructureProfile()
        {
            CreateMap<OrigenUsuario, OrigenUsuarioDTO>().ReverseMap();
            CreateMap<ProspectoDTO , SolicitudProductoModel>()
                .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => src.FechaNacimiento.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.HoraIngreso, opt => opt.MapFrom(src => src.HoraIngreso.ToString("HH:mm:ss")))
                .ForMember(dest => dest.FechaIngreso, opt => opt.MapFrom(src => src.FechaIngreso.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.EtapaId, opt => opt.MapFrom(src => (int)src.TipoEtapa))
                .ForMember(dest => dest.EstadoId, opt => opt.MapFrom(src => (int)src.TipoEstado))
                .ForMember(dest => dest.Dv, opt => opt.MapFrom(src => src.DigitoVerificador))
                .ForMember(dest => dest.OfertaId, opt => opt.MapFrom(src => (int)src.TipoOferta))
                .ForMember(dest => dest.ObservacionRechazo, opt => opt.MapFrom(src => src.ObservacionRechazo ?? ""))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre ?? ""))
                .ForMember(dest => dest.IdOcupacion, opt => opt.MapFrom(src => src.OcupacionId))
                .ForMember(dest => dest.NroPersonaDomicilio, opt => opt.MapFrom(src => src.NumeroPersonasDomicilio))
                .ReverseMap();
        }*/
        #endregion
    }
}