using Microsoft.AspNetCore.Mvc;
using movie_api.Model.Dto;
using movie_api.Model.Enum;
using movie_api.Models.DTO;
using MOVIE_API.Models;
using MOVIE_API.Models.DTO;
using MOVIE_API.Models.Enum;
using System.Collections.Generic;
using System.Security.Claims;

namespace movie_api.Services.Interfaces
{
    public interface IBookingService 
    {


        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //Modificar el estado de una booking a Returned -> Admin
        BookingResult ReturnBooking(int userId);


        //Ingresando un id de User, agrega una bookingDetail asociada 
        CreateBooking CreateBookingDetail(int userId, ClaimsPrincipal user, BookingDetailPostDto bookingDetailPostDto);


        //Modifica el estado de una booking -> Admin
        BookingResult UpdateBookingState(int bookingId, BookingState newState);


        //Crea una nueva bookina asocidado a un userid en state available y con fecha de retorno en 3 dias
        public BookingResult AddNewBooking(int userId);



        // Trae todas las bookingDetail asociadas a una booking
        List<BookingDetail> GetBookingDetailsByBookingId(int bookingId);


        //Ingresando un idBooking chequea sus el state de sus detalles
        BookingResult CheckBookingDetailState(int bookingId);



        //Trae todo el historial de rservas de un usuario
        List<BookingHistoryDto> GetHistory(int userId, ClaimsPrincipal user);


        //Desactivar un usuario 
        //Un usuario no puede ser desactivado si tiene reservas pendientes
        BaseResponse DesactivateUser(int idUser, ClaimsPrincipal user);


        //Verifica si el usario tiene reservas o no antes de ser desactivado
        bool HasPendingBookings(int userId);
    }
}
