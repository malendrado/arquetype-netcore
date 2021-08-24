using System.Collections.Generic;
using System.Net;

namespace Backend.Exceptions
{
    public static class ExceptionHelper
    {
        public static AppException CrearExcepcionOferta(string codigo, string mensaje, string nombreCliente, string mensajeCliente, string prospectoId)
        {
            var causas = new List<ExceptionCauses>();
            var causa = new ExceptionCauses() { Code = codigo, Message = mensaje, ProspectoId = prospectoId};
            causa.Data.Add("nombre", nombreCliente);
            causa.Data.Add("mensaje", mensajeCliente);
            causas.Add(causa);

            var ex = new AppException(HttpStatusCode.BadRequest, mensaje);
            ex.Data.Add("DETALLE", causas);

            return ex;
        }
        
        public static AppException SimpleInternalException(string codigo, string mensaje, string detalleException)
        {
            var errores = new List<ExceptionCauses>();
            var e = new AppException(HttpStatusCode.InternalServerError, mensaje);
            errores.Add(new ExceptionCauses { Code = codigo, Message = detalleException });
            e.Data.Add("detalle", errores);
            return e;

        }
        
        
        public static AppException SimpleException(string codigo, string mensaje)
        {
            return SimpleException(codigo, mensaje, mensaje);
        }
        
        public static AppException SimpleException(string codigo, string mensaje, string detalleException)
        {
            var errores = new List<ExceptionCauses>();
            var e = new AppException(HttpStatusCode.BadRequest, mensaje);
            errores.Add(new ExceptionCauses { Code = codigo, Message = detalleException });
            e.Data.Add("detalle", errores);
            return e;

        }
    }
}