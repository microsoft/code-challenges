using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcMovie.Controllers;
using System.Web.Mvc;
using System.Linq;
using System.Linq.Expressions;
using MvcMovie.Models;
using System.Collections.Generic;

namespace Test
{
  [TestClass]
  public class MoviesControllerIndex
  {
    [TestMethod]
    public void ShouldReturnViewWithMovies()
    {

      MoviesController sut = new MoviesController(new StubMovieRepository());
      var result = sut.Index("", "") as ViewResult;

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Model is IQueryable);

    }

    [TestMethod]
    public void ShouldSearchTitles()
    {

      var sut = new MoviesController(new StubMovieRepository());
      var result = sut.Index("", "Ghost") as ViewResult;

      Assert.IsNotNull(result);

      var moviesModel = (result.Model as IQueryable<Movie>).ToArray();

      Assert.IsNotNull(moviesModel);
      Assert.AreEqual(2, moviesModel.Count());
    }


  }


  public class StubMovieRepository : IMovieRepository
  {

    private static readonly Movie[] _Movies = new []
      {
        new Movie {ID=1, Genre="Romantic Comedy", Price=7.99M, Rating="PG", ReleaseDate=new DateTime(1989,1,11), Title="When Harry Met Sally" },
        new Movie {ID=2, Genre="Comedy", Price=8.99M, Rating="PG", ReleaseDate=new DateTime(1984,3,13), Title="Ghostbusters" },
        new Movie {ID=3, Genre="Comedy", Price=9.99M, Rating="PG", ReleaseDate=new DateTime(1986,2,23), Title="Ghostbusters 2" }
      };

    public IQueryable<Movie> Get()
    {
      return _Movies.AsQueryable();
    }

    public string[] GetGenres()
    {
      return _Movies.Select(m => m.Genre).Distinct().ToArray();
    }
  }
}
