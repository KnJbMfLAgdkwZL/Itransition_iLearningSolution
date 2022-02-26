using Business.Interfaces;
using Newtonsoft.Json;

namespace Business.Services;

public class ConverterService : IConverterService
{
    public Database.Models.UserSocial? UserSociaModel(string json)
    {
        var userSociaDto = UserSociaDto(json);

        return new Database.Models.UserSocial()
        {
            Uid = userSociaDto!.Uid!,
            Email = userSociaDto.Email!,
            Network = userSociaDto.Network,
            FirstName = userSociaDto.First_name,
            LastName = userSociaDto.Last_name
        };
    }

    public Dto.UserSocial? UserSociaDto(string json)
    {
        return JsonConvert.DeserializeObject<Dto.UserSocial>(json);
    }

    public Dto.UserSocial? UserSociaModelToDto(Database.Models.UserSocial userSocial)
    {
        return new Business.Dto.UserSocial()
        {
            Uid = userSocial.Uid,
            Email = userSocial.Email,
            Network = userSocial.Network,
            First_name = userSocial.FirstName!,
            Last_name = userSocial.LastName!
        };
    }

    public Database.Models.UserSocial? UserSociaDtoToMode(Dto.UserSocial userSocial)
    {
        return new Database.Models.UserSocial()
        {
            Uid = userSocial.Uid!,
            Email = userSocial.Email!,
            Network = userSocial.Network,
            FirstName = userSocial.First_name,
            LastName = userSocial.Last_name
        };
    }
}