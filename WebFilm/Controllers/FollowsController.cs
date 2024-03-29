﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : BaseController<int, Follow>
    {
        #region Field
        IFollowService _followService;
        #endregion

        #region Contructor
        public FollowsController(IFollowService followService) : base(followService)
        {
            _followService = followService;
        }
        #endregion

    }
}
