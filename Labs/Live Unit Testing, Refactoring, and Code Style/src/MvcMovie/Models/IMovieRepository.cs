using System.Linq;

namespace MvcMovie.Models
{
  public interface IMovieRepository
  {
    IQueryable<Movie> Get();
    string[] GetGenres();
  }
}