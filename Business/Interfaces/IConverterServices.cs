namespace Business.Interfaces;
public interface IConverterServices
{
    Database.Models.UserSocial? UserSociaModel(string json);
    Dto.UserSocial? UserSociaDto(string json);
    Dto.UserSocial? UserSociaModelToDto(Database.Models.UserSocial userSocial);
    Database.Models.UserSocial? UserSociaDtoToMode(Dto.UserSocial userSocial);
}