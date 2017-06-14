using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcMovie.Models
{

  public class MovieRepository : IMovieRepository
  {

    private MovieDBContext db = new MovieDBContext();

    public IQueryable<Movie> Get()
    {
      return from m in db.Movies
             select m;
    }

    public string[] GetGenres()
    {
      var q = from d in db.Movies
              orderby d.Genre
              select d.Genre;

      return q.Distinct().ToArray();

    }

  }

}