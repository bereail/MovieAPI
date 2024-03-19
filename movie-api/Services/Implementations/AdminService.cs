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

        public int CreateAdmin(AdminCreateDto adminDto)
        {
            try
            {
                var existingUser = _moviedbContext.Users.SingleOrDefault(u => u.Email == adminDto.Email);

                if (existingUser == null)
                {
                    User newUser = new User
                    {
                        Name = adminDto.Name,
                        Lastname = adminDto.Lastname,
                        Email = adminDto.Email,
                        Pass = adminDto.Pass,
                        Rol = "Admin",  
                        IsActive = adminDto.IsActive,
                    };

                    Admin newAdmin = new Admin
                    {
                        EmployeeNum = adminDto.EmployeeNum
                    };

                    newUser.Admins.Add(newAdmin);

                    _moviedbContext.Users.Add(newUser);
                    _moviedbContext.SaveChanges();

                    return newUser.Id;  
                }
                else
                {
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

