using PollyWebApp.Models;

namespace PollyWebApp.Services;

public interface IMyService
{
    Task<SomeDtoModel> GetSome(string content);
}