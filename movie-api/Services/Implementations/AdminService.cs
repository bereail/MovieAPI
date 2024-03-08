using movie_api.Data;
using movie_api.Models.DTO;
using MOVIE_API.Models;
using MOVIE_API.Models.DTO;
using MOVIE_API.Services.Interfaces;

namespace MOVIE_API.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly movieDbContext _moviedbContext;


        public AdminService(movieDbContext moviedbContext)
        {
            _moviedbContext = moviedbContext;
        }



        /*-------------------------------------------------------------------------------------------------------------*/

        //Crear Admin y lo asocia a un nuevo booking id -> Admin
        public int CreateAdmin(AdminCreateDto adminDto)
        {
            try
            {
                // Verificar si el user ya existe y no tiene un rol de administrador
                var existingUser = _moviedbContext.Users.SingleOrDefault(u => u.Email == adminDto.Email);

                if (existingUser == null)
                {
                    // Crear un nuevo objeto de tipo User y asignarle los valores del DTO
                    User newUser = new User
                    {
                        Name = adminDto.Name,
                        Lastname = adminDto.Lastname,
                        Email = adminDto.Email,
                        Pass = adminDto.Pass,
                        Rol = "Admin",  
                        IsActive = adminDto.IsActive,
                    };

                    // Crear un nuevo objeto de tipo Admin y asignarle los valores del DTO
                    Admin newAdmin = new Admin
                    {
                        EmployeeNum = adminDto.EmployeeNum
                    };

                    // Asociar el admin al user
                    newUser.Admins.Add(newAdmin);

                    // Agregar el nuevo usuario a la base de datos
                    _moviedbContext.Users.Add(newUser);
                    _moviedbContext.SaveChanges();

                    return newUser.Id;  
                }
                else
                {
                    // Usuario ya existe
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear un administrador: {ex.Message}");
                throw;
            }
        }

    }
}

