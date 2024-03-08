using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movie_api.Data;
using movie_api.Model.Dto;
using movie_api.Model.Enum;
using movie_api.Services.Implementations;
using movie_api.Services.Interfaces;
using MOVIE_API.Models;
using MOVIE_API.Models.Enum;
using System;
using System.Security.Claims;

namespace movie_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }



        //---------------------------------------------------------------------------------------------------------
        //Retornar una reserva -> Admin

        [HttpPost("returnBooking/{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult ReturnBooking(int userId)
        {
            var result = _bookingService.ReturnBooking(userId);

            if (result.Success)
            {
                return Ok(result); 
            }
            //Error
            return BadRequest(result); 
        }



        //--------------------------------------------------------------------------------------------------------

        //CREAR RESERVA -> Admin o user autenticado
        [HttpPost("create-booking-detail/{userId}")]
        [Authorize(Roles = "Admin,Client")]
        public IActionResult CreateBookingDetail(int userId, [FromBody] BookingDetailPostDto bookingDetailPostDto)
        {
            var result = _bookingService.CreateBookingDetail(userId, User, bookingDetailPostDto);

            if (result.Success)
            {
                return Ok(new { Success = true, Message = result.Message });
            }
            else
            {
                return BadRequest(new { Success = false, Message = result.Message });
            }
        }




        //---------------------------------------------------------------------------------
        //Modifica el estado de una booking 
        //Ingresando IdBoookoing y nuevo estado -> Admin

        [HttpPost("updateBookingState/{bookingId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateBookingState(int bookingId, [FromBody] BookingState newState)
        {
            var result = _bookingService.UpdateBookingState(bookingId, newState);

            if (result.Success)
            {
                return Ok(result); // 200 OK
            }

            return BadRequest(result); // 400 Bad Request si hay un problema
        }


        //---------------------------------------------------------------------------------------------------------
        // Trae todas las bookingDetail asociadas a una booking -> Admin
        [HttpGet("bookingDetails/{bookingId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetBookingDetailsByBookingId(int bookingId)
        {
            try
            {
                var bookingDetails = _bookingService.GetBookingDetailsByBookingId(bookingId);

                if (bookingDetails.Count > 0)
                {
                    return Ok(bookingDetails);
                }
                else
                {
                    return NotFound($"No se encontraron BookingDetails para la reserva con ID {bookingId}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener BookingDetails por idBooking: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        //---------------------------------------------------------------------------------------------------------

        //Trae todo el historial de rservas de un usuario
        // Endpoint para obtener el historial de reservas de un usuario
        [HttpGet("history/{userId}")]
        [Authorize(Roles = "Admin,Client")] // Permite tanto a Admin como a Client acceder a este endpoint
        public IActionResult GetBookingHistory(int userId)
        {
            try
            {
                // Obtengo el usuario autenticado
                var user = HttpContext.User;

                // Llama al servicio para obtener el historial de reservas
                var bookingHistory = _bookingService.GetHistory(userId, user);

                // Verifica si el historial es nulo o vacío
                if (bookingHistory == null || bookingHistory.Count == 0)
                {
                    return NotFound("No se encontró historial de reservas para el usuario.");
                }

                return Ok(bookingHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener historial de reservas: {ex.Message}");
            }
        }


        //---------------------------------------------------------------------------------------------------------
        // Desactivar un user, siempre y cuando no tenga reservaspendientes
        [HttpDelete("{idUser}/deactivate")]
        [Authorize(Roles = "Admin, Client")] 
        public IActionResult DeactivateUser(int idUser)
        {
            var currentUser = HttpContext.User; 

            var result = _bookingService.DesactivateUser(idUser, currentUser);

            if (result.Result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }



    }
}

