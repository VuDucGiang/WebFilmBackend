﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class FollowingDTO
    {
        public Guid UserID { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }

        public string? Fullname { get; set; }
    }
}
