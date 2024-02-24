using movie_api.Models.DTO;
using MOVIE_API.Models.DTO;
using System.Diagnostics;

namespace MOVIE_API.Services.Interfaces
{
    public interface IAdminService
    {

        //funcion para crear un admin
        int CreateAdmin(AdminCreateDto adminDto);

    }
}
