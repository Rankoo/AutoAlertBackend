using AutoAlertBackEnd.Dtos;

namespace AutoAlertBackEnd.Repositories;

public interface IUserCatalogRepository
{
    Task<UserCatalogsDto> GetUserCatalogsAsync();
}