using MOVIE_API.Models.DTO;

namespace movie_api.Services.Interfaces
{
    public interface IClientService
    {
        //funcion para crear un nuevo cliente
        int CreateClient(ClientCreateDto clientDto);


    }
}
