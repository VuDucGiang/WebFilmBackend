﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Follow;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IFilmRepository : IBaseRepository<int, Film>
    {
    }
}