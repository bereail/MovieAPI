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
        //Funcion para retornar una reserva  -> Admin
        //Ingresando un Id de User modifica el estado de la booking y las bookingDetail a returned
        public BookingResult ReturnBooking(int userId)
        {
            try
            {
                // Obtener la última BookingId asociada al usuario
                int lastBookingId = GetLastBookingIdByUserId(userId);
                Console.WriteLine($"Last Booking ID: {lastBookingId}");

                // Verificar si la reserva existe
                if (lastBookingId > 0)
                {
                    // Obtener el estado de la última reserva
                    var bookingState = GetBookingState(lastBookingId);

                    // Verificar si el estado es "Returned"
                    if (bookingState == BookingState.Returned)
                    {
                        return new BookingResult
                        {
                            Success = false,
                            Message = "El usuario no tiene reservas pendientes."
                        };
                    }

                    // Obtener todas las BookingDetails asociadas a la última reserva
                    var bookingDetails = GetBookingDetailsByBookingId(lastBookingId);

                    // Modificar el estado de todas las BookingDetails a "Returned"
                    foreach (var bookingDetail in bookingDetails)
                    {
                        bookingDetail.State = BookingDetailState.Returned;
                    }

                    // Guardar los cambios en la base de datos
                    _movieDbContext.SaveChanges();



                    // Verificar si todas las BookingDetails están en estado "Returned"
                    if (bookingDetails.All(bd => bd.State == BookingDetailState.Returned))
                    {

                        // Modificar el estado de la reserva a "Returned"
                        var lastBooking = _movieDbContext.Bookings.Find(lastBookingId);

                        if (lastBooking != null)
                        {
                            lastBooking.State = (int)BookingState.Returned;
                            _movieDbContext.SaveChanges();

                            // Actualizar el estado de las películas asociadas a "Available"
                            foreach (var bookingDetail in bookingDetails)
                            {
                                _movieService.SetMovieStateToAvailable(bookingDetail.IdMovie);

                                // Obtener el estado actualizado de la película
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
        //ingresando un id de user, trae la ultima bookingId asocidados(trae solo el id de booking) 
        //ReturnBooking(int userId)

        public int GetLastBookingIdByUserId(int userId)
        {
            try
            {
                // Obtener la última reserva asociada al userId 
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
        //Ingresnado un idBooking, retorna su state -> Admin
        //ReturnBooking(int userId)
        public BookingState GetBookingState(int bookingId)
        {
            try
            {
                // Buscar la reserva por su Id
                var booking = _movieDbContext.Bookings.Find(bookingId);

                // Verificar si la reserva existe
                if (booking != null)
                {
                    // Devolver el estado de la reserva
                    return (BookingState)booking.State;
                }
                else
                {
                    // La reserva no existe
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
        // Crea un nueva booking detail ingresando el id de un usuario, un booking no puede tener más de dos details -> CHECK
        // Modificar la firma del método para aceptar una lista de BookingDetailPostDto
        public CreateBooking CreateBookingDetail(int userId, ClaimsPrincipal user, BookingDetailPostDto bookingDetailPostDto)
        {
            try
            {
                // Verificar si el usuario autenticado es un administrador
                bool isAdmin = user.IsInRole("Admin");
                // Comparar el ID del usuario autenticado con el ID de usuario proporcionado o verificar si es un administrador
                if (!_userComparisonService.CompareUserIdWithLoggedInUser(userId, user) && !isAdmin)
                {
                    return new CreateBooking { Success = false, Message = "No tienes permisos para realizar esta acción." };
                }


                // Obtener el último estado de la reserva asociada al usuario
                var lastBookingState = GetStateLastBooking(userId);


                // Si el estado de la última reserva es "returned", agrega una nueva booking
                if (lastBookingState.Success && (lastBookingState.Message == "Reserva retornada, crear nueva reserva." || lastBookingState.Message == "El estado de la última reserva es 'returned'."))
                {
                    Console.WriteLine("Agregando nueva reserva...");
                    AddNewBooking(userId);
                }

                // Obtener la película por título
                var movieTitle = bookingDetailPostDto.MovieTitle;
                var existingMovie = _movieDbContext.Movies.SingleOrDefault(u => u.Title.ToLower() == movieTitle.ToLower());

                // Si la película ingresada no existe en la base de datos
                if (existingMovie == null)
                {
                    return new CreateBooking { Success = false, Message = $"La película con el título '{movieTitle}' no existe en la base de datos." };
                }

                // Verificar si la película está disponible para alquilar
                if (existingMovie.State != MovieState.Available)
                {
                    return new CreateBooking { Success = false, Message = $"La película '{existingMovie.Title}' no está disponible para alquilar en este momento." };
                }

                // Obtener la lista de BookingIds asociados al usuario
                var bookingIds = _userService.GetBookingIdsByUserId(userId);

                if (bookingIds.Count > 0)
                {
                    // Seleccionar el último BookingId de la lista
                    int lastBookingId = bookingIds.Last();

                    // Verificar el estado de la última reserva
                    var bookingState = CheckBookingDetailState(lastBookingId);

                    // Verificar la cantidad actual de BookingDetails
                    var currentBookingDetailsCount = _movieDbContext.BookingDetails.Count(bd => bd.IdBooking == lastBookingId);

                    if (bookingState.Success && currentBookingDetailsCount + 1 <= 2)
                    {
                        // Crear los BookingDetail con el state pending por defecto
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

                        // Modificar el estado de la película a Reserved
                        existingMovie.State = MovieState.Reserved;
                        _movieDbContext.SaveChanges();

                        // Se creó correctamente
                        return new CreateBooking { Success = true, Message = "BookingDetail creado con éxito." };
                    }
                    else
                    {
                        // Si supera los dos BookingDetail
                        return new CreateBooking { Success = false, Message = "No se pueden agregar más de 2 BookingDetails." };
                    }
                }
                else
                {
                    // Error
                    return new CreateBooking { Success = false, Message = "No hay BookingIds asociados al usuario." };
                }
            }
            catch (Exception ex)
            {
                // Error
                return new CreateBooking { Success = false, Message = $"Error al crear la BookingDetail: {ex.Message}" };
            }
        }






        //---------------------------------------------------------------------------------
        //Modifica el estado de una booking 
        //Ingresando IdBoookoing y nuevo estado
        public BookingResult UpdateBookingState(int bookingId, BookingState newState)
        {
            try
            {
                var booking = _movieDbContext.Bookings.Find(bookingId);

                if (booking != null)
                {
                    // Verificar si el nuevo estado es válido
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
        //Crea una nueva bookina asocidado a un userid en state available y con fecha de retorno en 3 dias
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
        //Trae todo el historial de rservas de un usuario

        public List<BookingHistoryDto> GetHistory(int userId, ClaimsPrincipal user)
        {
            List<int> bookingIds;

            // Verificar si el usuario autenticado es un administrador
            bool isAdmin = user.IsInRole("Admin");

            // Comparar el ID del usuario autenticado con el ID de usuario proporcionado o verificar si es un administrador
            if (_userComparisonService.CompareUserIdWithLoggedInUser(userId, user) || isAdmin)
            {
                // Obtener las BookingIds asociadas al usuario
                bookingIds = _userService.GetBookingIdsByUserId(userId);
            }
            else
            {
                // Usuario no autorizado para ver el historial
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
        // Trae todas las bookingDetail asociadas a una booking
        //CheckBookingdetail(int bookingId)
        public List<BookingDetail> GetBookingDetailsByBookingId(int bookingId)
        {
            try
            {
                // Ejecuta la consulta SQL para obtener las BookingDetail
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
        //Ingresando un idBooking chequea sus el state de sus detalles
        // CreateBookingDetail
        public BookingResult CheckBookingDetailState(int bookingId)
        {
            try
            {

                // Obtener todas las BookingDetail asociadas a la reserva
                var bookingDetails = GetBookingDetailsByBookingId(bookingId);


                //verifica si la reserva no tiene detalles aosciados
                if (bookingDetails.Count == 0)
                {
                    return new BookingResult
                    {
                        Success = true,
                        Message = "La reserva no tiene detalles asociados",
                        NumberState = 0
                    };
                }
                //verigifica que la reserva no tenga mas de dos detalles (1) -
                if (bookingDetails.Count >= 2)
                {
                    return new BookingResult
                    {
                        Success = false,
                        Message = "Error: La reserva no puede tener más de dos detalles de reserva.",
                        NumberState = 1
                    };
                }
                // Verifica el estado de cada BookingDetail
                var pendingBookingDetailsCount = bookingDetails.Count(bd => bd.State == BookingDetailState.Pending);
                var returnedBookingDetailsCount = bookingDetails.Count(bd => bd.State == BookingDetailState.Returned);

                //verifica si tiene dos reservas pendientes  (2) Pending
                if (pendingBookingDetailsCount == 2)
                {

                    return new BookingResult
                    {
                        Success = false,
                        Message = "No se puede generar una nueva reserva porque tiene dos reservas pendientes. Modificar el estado de la Booking a Pending",
                        NumberState = 2
                    };
                }

                // en el caos que solo tenga una pendiente si podra generar una nueva reserva (3)
                else if (pendingBookingDetailsCount == 1)
                {

                    //moodificar el estado a last pending
                    return new BookingResult
                    {
                        Success = true,
                        Message = "Puede generar una nueva bookingDetail (tiene solo una reserva pendiente).",
                        NumberState = 3
                    };
                }

                //en el caos que tenga dos resevras retornadas (4)
                else if (returnedBookingDetailsCount == 2)
                {

                    return new BookingResult
                    {
                        Success = true,
                        Message = "Puede generar una nueva reserva. Modificar slk estado de la booking a Returned",
                        NumberState = 4
                    };

                }
                //en el caso que tenga una solo retoronada puede agregar un nuevo detalle dentro de esa booking (5)
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
                    //estado de la reserva descocnocido (6)
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
                // Manejar la excepción según tus necesidades
                return new BookingResult
                {
                    Success = false,
                    Message = "Error interno al procesar la reserva."
                };
            }
        }




        //-----------------------------------------------------------------------------------------------------------------------------
        //Utiliza dos funciones, inrgesando id de user, busca su utlima booking, y retorna el state para saber si es returned o available
        //si el state de la ultima reserva es available -> devolver true
        //si el el ultimo state de la reserva es returned -> crear un nueva booking
        //si el ultimo state es pending -> el usurio tiene reservas pendientes
        //utiliza la funcion GetLastBookingIdByUserId(userId) y GetBookingState(bookingId)
        public BookingResult GetStateLastBooking(int userId)
        {

            //ingresando un id de user, trae la ultima bookingId asocidados
            var lastBooking = GetLastBookingIdByUserId(userId);

            //si no es null
            if (lastBooking > 0)
            {
                //trae el state de la  ultima booking
                var bookingState = GetBookingState(lastBooking);

                switch (bookingState)
                {
                    //si el state de disponible
                    case BookingState.Available: //1

                        //addbooking (agregar nueva reserva)  retornar el id de reserva
                        return new BookingResult
                        {
                            Success = true,
                            Message = "Puede agregar nuevo detalle de reserva.",
                            NumberState = 1

                        };

                    //si el state de returned
                    case BookingState.Returned: //2

                        //addbooking (agregar nueva reserva)  retornar el id de reserva
                        return new BookingResult
                        {
                            Success = true,
                            Message = "Reserva retornada, crear nueva reserva.",
                            NumberState = 2
                        };

                    //Si el state es pending
                    //El usuario no puede generar neuva reserva porque tiene reservas pendientes
                    case BookingState.Pending: //3

                        return new BookingResult
                        {
                            Success = false,
                            Message = "El estado de la última reserva es 'Pending'.",
                            NumberState = 3
                        };

                    default:


                        //estado desconoocido
                        return new BookingResult
                        {
                            Success = false,
                            Message = $"El estado de la última reserva no es reconocido: {bookingState}."
                        };
                }
            }
            else
            {
                //error
                return new BookingResult
                {
                    Success = false,
                    Message = "No se encontró una reserva para el usuario."
                };
            }
        }




        //--------------------------------------------------------------------------------------------------------------------------------------
        //Desactivar un usuario
        public BaseResponse DesactivateUser(int idUser, ClaimsPrincipal user)
        {
            var existingUser = _userService.GetUserById(idUser);

            if (existingUser != null)
            {
                try
                {
                    // Verificar si el usuario actual tiene permiso para desactivar la cuenta del usuario específico
                    if (!_userComparisonService.CompareUserIdWithLoggedInUser(idUser, user))
                    {
                        return new BaseResponse
                        {

                            Result = false,
                            Message = "No tienes permisos para desactivar la cuenta de este usuario."
                        };

                    }

                    // Verificar si el usuario tiene reservas pendientes
                    if (HasPendingBookings(idUser))
                    {
                        return new BaseResponse
                        {
                            Result = false,
                            Message = "No puedes desactivar la cuenta de este usuario porque tiene reservas pendientes."
                        };
                    }

                    // Cambiar el estado IsActive a false o eliminar físicamente el usuario según el rol
                    if (existingUser.Rol == "Admin")
                    {
                        // Eliminar físicamente si es un administrador
                        _movieDbContext.Users.Remove(existingUser);
                    }
                    else
                    {
                        // Desactivar la cuenta si es un cliente
                        existingUser.IsActive = false;
                    }

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



        //Verifica si el usario tiene reservas o no antes de ser desactivado

        public bool HasPendingBookings(int userId)
        {
            try
            {
                // Obtener la última reserva asociada al userId
                var lastBookingId = GetLastBookingIdByUserId(userId);

                // Si se encuentra una reserva
                if (lastBookingId > 0)
                {
                    // Obtener el estado de la última reserva
                    var bookingState = GetBookingState(lastBookingId);

                    // Verificar si el estado es "Pending"
                    if (bookingState == BookingState.Pending)
                    {
                        return true; // El usuario tiene reservas pendientes
                    }
                }

                return false; // El usuario no tiene reservas pendientes o no se encontró una reserva
            }
            catch (Exception ex)
            {
                // Manejar la excepción según tus necesidades
                Console.WriteLine($"Error al verificar reservas pendientes: {ex.Message}");
                return false;
            }
        }


    }


}



