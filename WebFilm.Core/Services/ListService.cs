using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class ListService : BaseService<int, List>, IListService
    {
        IListRepository _listRepository;
        IUserRepository _userRepository;
        IFilmListRepository _filmListRepository;
        IFilmRepository _filmRepository;
        ILikeRepository _likeRepository;
        private readonly IConfiguration _configuration;

        public ListService(IListRepository listRepository,
            IConfiguration configuration,
            IUserRepository userRepository,
            IFilmListRepository filmListRepository,
            IFilmRepository filmRepository,
            ILikeRepository likeRepository) : base(listRepository)
        {
            _listRepository = listRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _filmListRepository = filmListRepository;
            _filmRepository = filmRepository;
            _likeRepository = likeRepository;
        }

        public List<ListPopularDTO> GetListPopular()
        {
            List<ListPopularDTO> dtos = new List<ListPopularDTO>();
            List<List> lists = _listRepository.GetAll().OrderByDescending(p => p.LikesCount).Take(3).ToList();
            this.enrichListPopular(dtos, lists, 5);
            return dtos;
        }

        public List<ListPopularDTO> GetListPopularWeek()
        {
            List<ListPopularDTO> dtos = new List<ListPopularDTO>();
            List<ListPopularWeekDTO> lists = _listRepository.PopularWeekList();
            List<int> ids = lists.Select(t => t.ListID).ToList();
            List<List> listPopular = _listRepository.GetAll().Where(p => ids.Contains(p.ListID)).ToList();
            this.enrichListPopular(dtos, listPopular, 5);
            return dtos;
        }

        public List<ListPopularDTO> GetListRecentLikes()
        {
            List<ListPopularDTO> dtos = new List<ListPopularDTO>();
            List<ListRecentLikeDTO> likesList = _listRepository.RecentLikeList();
            List<int> ids = likesList.Select(t => t.ListID).ToList();
            List<List> listPopular = _listRepository.GetAll().Where(p => ids.Contains(p.ListID)).ToList();
            this.enrichListPopular(dtos, listPopular, 5);
            return dtos;
        }

        private void enrichListPopular(List<ListPopularDTO> dtos, List<List> lists, int count)
        {
            foreach (var list in lists)
            {
                ListPopularDTO dto = new ListPopularDTO();
                UserReviewDTO userDTO = new UserReviewDTO();
                //user
                User user = _userRepository.GetByID(list.UserID);
                if (user != null)
                {
                    userDTO.Avatar = user.Avatar;
                    userDTO.FullName = user.FullName;
                    userDTO.UserName = user.UserName;
                    userDTO.UserID = user.UserID;
                }

                //film
                List<BaseFilmDTO> filmDTOs = new List<BaseFilmDTO>();
                List<FilmList> filmLists = _filmListRepository.GetAll().Where(p => p.ListID == list.ListID).Take(count).ToList();
                foreach (var filmList in filmLists)
                {
                    BaseFilmDTO filmDTO = new BaseFilmDTO();
                    Film film = _filmRepository.GetByID(filmList.FilmID);
                    filmDTO.FilmID = film.FilmID;
                    filmDTO.Title = film.Title;
                    filmDTO.Poster_path = film.Poster_path;
                    filmDTO.Release_date = film.Release_date;
                    filmDTOs.Add(filmDTO);
                }


                dto.ListID = list.ListID;
                dto.ListName = list.ListName;
                dto.Description = list.Description;
                dto.TotalLike = list.LikesCount;
                dto.TotalComment = list.CommentsCount;
                dto.User = userDTO;
                dto.List = filmDTOs;
                dto.Total = filmLists.Count();

                dtos.Add(dto);
            }
        }

        public List<ListPopularDTO> GetListPopularMonth()
        {
            List<ListPopularDTO> dtos = new List<ListPopularDTO>();
            List<ListPopularWeekDTO> lists = _listRepository.PopularMonthList();
            List<int> ids = lists.Select(t => t.ListID).ToList();
            List<List> listPopular = _listRepository.GetAll().Where(p => ids.Contains(p.ListID)).ToList();
            this.enrichListPopular(dtos, listPopular, 5);
            return dtos;
        }

        public List<ListPopularDTO> GetCrewList()
        {
            List<ListPopularDTO> dtos = new List<ListPopularDTO>();
            List<ListPopularWeekDTO> lists = _listRepository.ListCrew();
            List<int> ids = lists.Select(t => t.ListID).ToList();
            List<List> crewList = _listRepository.GetAll().Where(p => ids.Contains(p.ListID)).ToList();
            this.enrichListPopular(dtos, crewList, 5);
            return dtos;
        }

        public List<ListPopularDTO> ListTop()
        {
            List<ListPopularDTO> dtos = new List<ListPopularDTO>();
            List<ListPopularWeekDTO> lists = _listRepository.ListTopLike();
            List<int> ids = lists.Select(t => t.ListID).ToList();
            List<List> crewList = _listRepository.GetAll().Where(p => ids.Contains(p.ListID)).ToList();
            this.enrichListPopular(dtos, crewList, 10);
            return dtos;
        }
    }
}
