using LBJ.Models.Nba;

namespace LBJ.APIServices.Contracts;
public interface INbaService
{
    Task<Game> GetLakersGames();
}
