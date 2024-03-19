using movie_api.Data;
using movie_api.Model.Dto;
using MOVIE_API.Models;
using movie_api.Services.Interfaces;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MOVIE_API.Models.Enum;
using movie_api.Model.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using movie_api.Models.DTO;
using System.Security.Claims;

namespace movie_api.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly movieDbContext _movieDbContext; //Context
        private readonly IUserService _userService;  // Interfaz de user
        private readonly IMovieService _movieService;  // Interfaz de reservas
        private readonly IUserComparisonService _userComparisonService;

        public string MovieName { get; private set; }

        public BookingService(movieDbContext movieDbContext, IUserService userService, IMovieService movieService, IUserComparisonService userComparisonService)
        {
            _movieDbContext = movieDbContext;
            _userService = userService;
            _movieService = movieService;
            _userComparisonService = userComparisonService;
        }

        //-------------------------------------------------------------------------------------------------------------------------
        public BookingResult ReturnBooking(int userId)
        {
            try
            {

                int lastBookingId = GetLastBookingIdByUserId(userId);
                Console.WriteLine($"Last Booking ID: {lastBookingId}");


                if (lastBookingId > 0)
                {

                    var bookingState = GetBookingState(lastBookingId);


                    if (bookingState == BookingState.Returned)
                    {
                        return new BookingResult
                        {
                            Success = false,
                            Message = "El usuario no tiene reservas pendientes."
                        };
                    }


                    var bookingDetails = GetBookingDetailsByBookingId(lastBookingId);


                    foreach (var bookingDetail in bookingDetails)
                    {
                        bookingDetail.State = BookingDetailState.Returned;
                    }


                    _movieDbContext.SaveChanges();


                    if (bookingDetails.All(bd => bd.State == BookingDetailState.Returned))
                    {

                        var lastBooking = _movieDbContext.Bookings.Find(lastBookingId);

                        if (lastBooking != null)
                        {
                            lastBooking.State = (int)BookingState.Returned;
                            _movieDbContext.SaveChanges();

                            foreach (var bookingDetail in bookingDetails)
                            {
                                _movieService.SetMovieStateToAvailable(bookingDetail.IdMovie);

                                var updatedMovieState = _movieService.GetMovieAndStateById(bookingDetail.IdMovie).Item2;

                                Console.WriteLine($"Movie ID: {bookingDetail.IdMovie}, Updated State: {updatedMovieState}");
                            }

                            return new BookingResult
                            {
                                Success = true,
                                Message = "Reserva retornada correctamente. Estado de películas actualizado a 'Available'."
                            };
                        }
                        else
                        {
                            Console.WriteLine("No se encontró la reserva para el usuario.");
                            return new BookingResult
                            {
                                Success = false,
                                Message = "No se encontró la reserva para el usuario."
                            };
                        }
                    }
                    else
                    {
                        return new BookingResult
                        {
                            Success = false,
                            Message = "BookingDetails actualizadas a 'Returned', pero no todas han sido marcadas."
                        };
                    }
                }
                else
                {
                    Console.WriteLine("No se encontró una reserva para el usuario.");
                    return new BookingResult
                    {
                        Success = false,
                        Message = "No se encontró una reserva para el usuario."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la operación ReturnedBooking: {ex.Message}");
                return new BookingResult
                {
                    Success = false,
                    Message = $"Error al realizar la operación ReturnedBooking: {ex.Message}"
                };
            }
        }


        //----------------------------------------------------------------------------------------------------------------------------------------
        public int GetLastBookingIdByUserId(int userId)
        {
            try
            {
                var lastBookingId = _movieDbContext.Bookings
                    .Where(b => b.IdUser == userId)
                    .OrderByDescending(b => b.Id)
                    .Select(b => b.Id)
                    .FirstOrDefault();

                return lastBookingId;
            }
            catch (Exception ex)
            {
                //Error
                Console.WriteLine($"Error al obtener la última BookingId por ID de usuario: {ex.Message}");
                return 0;
            }
        }


        //----------------------------------------------------------------------------------------------------------------------------------------
        public BookingState GetBookingState(int bookingId)
        {
            try
            {
                var booking = _movieDbContext.Bookings.Find(bookingId);

                if (booking != null)
                {
                    return (BookingState)booking.State;
                }
                else
                {
                    Console.WriteLine($"La reserva con Id {bookingId} no existe.");
                    return BookingState.Error;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el estado de la reserva: {ex.Message}");
                // Error
                return BookingState.Error;
            }
        }






        //-----------------------------------------------------------------------------------------------------------------------------
        public CreateBooking CreateBookingDetail(int userId, ClaimsPrincipal user, BookingDetailPostDto bookingDetailPostDto)
        {
            try
            {
                bool isAdmin = user.IsInRole("Admin");

                var existingUser = _userService.GetUserById(userId);
                if (existingUser == null || !existingUser.IsActive)
                {
                    return new CreateBooking { Success = false, Message = "El usuario no está activo y no puede realizar esta acción." };
                }

                if (!_userComparisonService.CompareUserIdWithLoggedInUser(userId, user) && !isAdmin)
                {
                    return new CreateBooking { Success = false, Message = "No tienes permisos para realizar esta acción." };
                }


                var lastBookingState = GetStateLastBooking(userId);

                if (lastBookingState.Success && (lastBookingState.Message == "Reserva retornada, crear nueva reserva." || lastBookingState.Message == "El estado de la última reserva es 'returned'."))
                {
                    Console.WriteLine("Agregando nueva reserva...");
                    AddNewBooking(userId);
                }

                var movieTitle = bookingDetailPostDto.MovieTitle;
                var existingMovie = _movieDbContext.Movies.SingleOrDefault(u => u.Title.ToLower() == movieTitle.ToLower());

                if (existingMovie == null)
                {
                    return new CreateBooking { Success = false, Message = $"La película con el título '{movieTitle}' no existe en la base de datos." };
                }

                if (existingMovie.State != MovieState.Available)
                {
                    return new CreateBooking { Success = false, Message = $"La película '{existingMovie.Title}' no está disponible para alquilar en este momento." };
                }

                var bookingIds = _userService.GetBookingIdsByUserId(userId);

                if (bookingIds.Count > 0)
                {
                    int lastBookingId = bookingIds.Last();

                    var bookingState = CheckBookingDetailState(lastBookingId);

                    var currentBookingDetailsCount = _movieDbContext.BookingDetails.Count(bd => bd.IdBooking == lastBookingId);

                    if (bookingState.Success && currentBookingDetailsCount + 1 <= 2)
                    {
                        var bookingDetailEntity = new BookingDetail
                        {
                            IdMovie = existingMovie.Id,
                            Comment = bookingDetailPostDto.Comment,
                            IdBooking = lastBookingId,
                            BookingDate = DateTime.Now,
                            ReturnDate = DateTime.Now.AddHours(48),
                            State = BookingDetailState.Pending
                        };



                        _movieDbContext.BookingDetails.Add(bookingDetailEntity);
                        _movieDbContext.SaveChanges();

                        existingMovie.State = MovieState.Reserved;
                        _movieDbContext.SaveChanges();

                        return new CreateBooking { Success = true, Message = "BookingDetail creado con éxito." };
                    }
                    else
                    {
                        return new CreateBooking { Success = false, Message = "No se pueden agregar más de 2 BookingDetails." };
                    }
                }
                else
                {
                    return new CreateBooking { Success = false, Message = "No hay BookingIds asociados al usuario." };
                }
            }
            catch (Exception ex)
            {
                return new CreateBooking { Success = false, Message = $"Error al crear la BookingDetail: {ex.Message}" };
            }
        }






        //---------------------------------------------------------------------------------

        public BookingResult UpdateBookingState(int bookingId, BookingState newState)
        {
            try
            {
                var booking = _movieDbContext.Bookings.Find(bookingId);

                if (booking != null)
                {
                    if (Enum.IsDefined(typeof(BookingState), newState))
                    {
                        booking.State = (int)newState;
                        _movieDbContext.SaveChanges();

                        return new BookingResult
                        {
                            Success = true,
                            Message = $"Estado de Booking con ID {bookingId} actualizado a {newState}."
                        };
                    }
                    else
                    {
                        return new BookingResult
                        {
                            Success = false,
                            Message = $"El estado proporcionado ({newState}) no es válido."
                        };
                    }
                }
                else
                {
                    return new BookingResult
                    {
                        Success = false,
                        Message = $"No se encontró el Booking con ID {bookingId}."
                    };
                }
            }
            catch (Exception ex)
            {
                return new BookingResult
                {
                    Success = false,
                    Message = $"Error al actualizar el estado del Booking: {ex.Message}"
                };
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------
        public BookingResult AddNewBooking(int userId)
        {
            try
            {
                Booking newBooking = new Booking
                {
                    IdUser = userId,
                    BookingDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddHours(72),
                    State = (int)BookingDetailState.Available,
                };


                _movieDbContext.Bookings.Add(newBooking);
                _movieDbContext.SaveChanges();

                int newBookingId = newBooking.Id;

                Console.WriteLine($"Nueva reserva creada con Id: {newBookingId}");

                return new BookingResult
                {
                    Success = true,
                    BookingId = newBooking.Id
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la reserva: {ex}");

                return new BookingResult
                {
                    Success = false,
                    Message = $"Error al crear la reserva: {ex.Message}"
                };
            }
        }



        //-------------------------------------------------------------------------------------------------

        public List<BookingHistoryDto> GetHistory(int userId, ClaimsPrincipal user)
        {
            List<int> bookingIds;

            bool isAdmin = user.IsInRole("Admin");


            if (_userComparisonService.CompareUserIdWithLoggedInUser(userId, user) || isAdmin)
            {

                bookingIds = _userService.GetBookingIdsByUserId(userId);
            }
            else
            {
                return new List<BookingHistoryDto>();
            }

            var bookingHistoryDtos = new List<BookingHistoryDto>();

            foreach (var bookingId in bookingIds)
            {
                var bookingDetails = GetBookingDetailsByBookingId(bookingId);

                var bookingHistoryDto = new BookingHistoryDto
                {
                    BookingDate = bookingDetails.FirstOrDefault()?.BookingDate ?? DateTime.MinValue,
                    ReturnDate = bookingDetails.FirstOrDefault()?.ReturnDate,
                    State = bookingDetails.FirstOrDefault()?.StateString,
                    BookingDetails = bookingDetails.Select(d => new BookingDetailDto
                    {
                        IdMovie = d.IdMovie,
                        MovieTitle = _movieService.GetMovieTitleById(d.IdMovie),
                        State = d.State,
                        Comment = d.Comment
                    }).ToList()
                };

                bookingHistoryDtos.Add(bookingHistoryDto);
            }

            return bookingHistoryDtos;
        }




        //---------------------------------------------------------------------------------------------------------

        public List<BookingDetail> GetBookingDetailsByBookingId(int bookingId)
        {
            try
            {

                var bookingDetails = _movieDbContext.BookingDetails
                    .FromSqlRaw("SELECT * FROM BookingDetails WHERE idBooking = {0}", bookingId)
                    .ToList();

                return bookingDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener BookingDetails por idBooking: {ex.Message}");
                return new List<BookingDetail>();
            }
        }



        //---------------------------------------------------------------------------------------------------------

        public BookingResult CheckBookingDetailState(int bookingId)
        {
            try
            {

                var bookingDetails = GetBookingDetailsByBookingId(bookingId);


                if (bookingDetails.Count == 0)
                {
                    return new BookingResult
                    {
                        Success = true,
                        Message = "La reserva no tiene detalles asociados",
                        NumberState = 0
                    };
                }

                if (bookingDetails.Count >= 2)
                {
                    return new BookingResult
                    {
                        Success = false,
                        Message = "Error: La reserva no puede tener más de dos detalles de reserva.",
                        NumberState = 1
                    };
                }

                var pendingBookingDetailsCount = bookingDetails.Count(bd => bd.State == BookingDetailState.Pending);
                var returnedBookingDetailsCount = bookingDetails.Count(bd => bd.State == BookingDetailState.Returned);

                if (pendingBookingDetailsCount == 2)
                {

                    return new BookingResult
                    {
                        Success = false,
                        Message = "No se puede generar una nueva reserva porque tiene dos reservas pendientes. Modificar el estado de la Booking a Pending",
                        NumberState = 2
                    };
                }

                else if (pendingBookingDetailsCount == 1)
                {

                    return new BookingResult
                    {
                        Success = true,
                        Message = "Puede generar una nueva bookingDetail (tiene solo una reserva pendiente).",
                        NumberState = 3
                    };
                }

                else if (returnedBookingDetailsCount == 2)
                {

                    return new BookingResult
                    {
                        Success = true,
                        Message = "Puede generar una nueva reserva. Modificar slk estado de la booking a Returned",
                        NumberState = 4
                    };

                }
                else if (returnedBookingDetailsCount == 1)
                {

                    return new BookingResult
                    {
                        Success = false,
                        Message = "Puede agregar un nuevo detalle al booking.",
                        NumberState = 5
                    };
                }
                else
                {
                    Console.WriteLine($"Estado de reserva no reconocido: , Valor real de bookingDetails.State: {bookingDetails}");


                    return new BookingResult
                    {
                        Success = false,
                        Message = "Error: Estado de reserva no válido.",
                        NumberState = 6
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new BookingResult
                {
                    Success = false,
                    Message = "Error interno al procesar la reserva."
                };
            }
        }




        //-----------------------------------------------------------------------------------------------------------------------------

        public BookingResult GetStateLastBooking(int userId)
        {

            var lastBooking = GetLastBookingIdByUserId(userId);

            if (lastBooking > 0)
            {

                var bookingState = GetBookingState(lastBooking);

                switch (bookingState)
                {

                    case BookingState.Available: 


                        return new BookingResult
                        {
                            Success = true,
                            Message = "Puede agregar nuevo detalle de reserva.",
                            NumberState = 1

                        };

                    case BookingState.Returned: 

                        return new BookingResult
                        {
                            Success = true,
                            Message = "Reserva retornada, crear nueva reserva.",
                            NumberState = 2
                        };

                    case BookingState.Pending: 

                        return new BookingResult
                        {
                            Success = false,
                            Message = "El estado de la última reserva es 'Pending'.",
                            NumberState = 3
                        };

                    default:


                        return new BookingResult
                        {
                            Success = false,
                            Message = $"El estado de la última reserva no es reconocido: {bookingState}."
                        };
                }
            }
            else
            {

                return new BookingResult
                {
                    Success = false,
                    Message = "No se encontró una reserva para el usuario."
                };
            }
        }




        //--------------------------------------------------------------------------------------------------------------------------------------
        public BaseResponse DesactivateUser(int idUser, ClaimsPrincipal user)
        {
            var existingUser = _userService.GetUserById(idUser);

            if (existingUser != null)
            {
                try
                {

                    if (!_userComparisonService.CompareUserIdWithLoggedInUser(idUser, user))
                    {
                        return new BaseResponse
                        {
                            Result = false,
                            Message = "No tienes permisos para desactivar la cuenta de este usuario."
                        };
                    }

                    if (HasPendingBookings(idUser))
                    {
                        return new BaseResponse
                        {
                            Result = false,
                            Message = "No puedes desactivar la cuenta de este usuario porque tiene reservas pendientes."
                        };
                    }

                    existingUser.IsActive = false;

                    _movieDbContext.SaveChanges();

                    return new BaseResponse
                    {
                        Result = true,
                        Message = "Usuario desactivado correctamente."
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse
                    {
                        Result = false,
                        Message = $"Error al desactivar el usuario: {ex.Message}"
                    };
                }
            }
            else
            {
                return new BaseResponse
                {
                    Result = false,
                    Message = "El usuario no existe."
                };
            }
        }


        //-----------------------------------------------------------------------------------------------------------------------
        public bool HasPendingBookings(int userId)
        {
            try
            {
                var lastBookingId = GetLastBookingIdByUserId(userId);

                var bookingDetails = GetBookingDetailsByBookingId(lastBookingId);

                if (bookingDetails.Any(detail => detail.State == BookingDetailState.Pending))
                {
                    return true;
                }

                return false; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar reservas pendientes: {ex.Message}");
                return false;
            }
        }


    }


}



