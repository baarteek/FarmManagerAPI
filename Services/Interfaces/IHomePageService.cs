using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces;

public interface IHomePageService
{
    Task<HomePageDTO> GetHomePageInfo(string userName);
}