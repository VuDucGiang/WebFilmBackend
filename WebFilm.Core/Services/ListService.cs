﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Exceptions;
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
        ICommentRepository _commentRepository;
        private readonly IConfiguration _configuration;

        public ListService(IListRepository listRepository,
            IConfiguration configuration,
            IUserRepository userRepository,
            IFilmListRepository filmListRepository,
            IFilmRepository filmRepository,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository) : base(listRepository)
        {
            _listRepository = listRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _filmListRepository = filmListRepository;
            _filmRepository = filmRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
        }
        public async Task<object> GetListOfUser(int pageSize, int pageIndex, string userName)
        {
            return await _listRepository.GetListOfUser(pageSize, pageIndex, userName);
        }

        public async Task<object> AddListDetail(ListDTO list)
        {
            if (string.IsNullOrEmpty(list.FilmIDs)) {
                throw new ServiceException("FilmIDs is required");
            }
            return await _listRepository.AddListDetail(list);
        }
        public async Task<bool> EditListDetail(ListDTO list)
        {
            return await _listRepository.EditListDetail(list);
        }

        public async Task<bool> DeleteListDetail(int listID)
        {
            return await _listRepository.DeleteListDetail(listID);
        }

        public async Task<object> GetPaging(PagingFilterParameter parameter)
        {
            return await _listRepository.GetPaging(parameter);
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
            List<List> listPopular = _listRepository.GetAll().Where(p => ids.Contains(p.ListID)).OrderBy(p => ids.IndexOf(p.ListID)).ToList();
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
                List<FilmList> filmCounts = _filmListRepository.GetAll().Where(p => p.ListID == list.ListID).ToList();
                foreach (var filmList in filmLists)
                {
                    Film film = _filmRepository.GetByID(filmList.FilmID);
                    if(film != null)
                    {
                        BaseFilmDTO filmDTO = new BaseFilmDTO();
                        filmDTO.FilmID = film.FilmID;
                        filmDTO.Title = film.Title;
                        filmDTO.Poster_path = film.Poster_path;
                        filmDTO.Release_date = film.Release_date;
                        filmDTOs.Add(filmDTO);
                    }
                }


                dto.ListID = list.ListID;
                dto.ListName = list.ListName;
                dto.Description = list.Description;
                dto.LikesCount = list.LikesCount;
                dto.CommentsCount = list.CommentsCount;
                dto.User = userDTO;
                dto.List = filmDTOs;
                dto.Total = filmCounts.Count();

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

        public PagingFilmResult DetailList(int id, PagingDetailList paging)
        {
            PagingFilmResult res = new PagingFilmResult();
            List listDetail = _listRepository.GetByID(id);
            if (listDetail == null) {
                throw new ServiceException("Không tìm thấy list phù hợp");
            }
            List<FilmList> filmList = _filmListRepository.GetAll().Where(p => p.ListID == id).ToList();
            List<int> ids = filmList.Select(t => t.FilmID).ToList();
            var films = _filmRepository.GetAll().Where(p => ids.Contains(p.FilmID));
            //var films = _filmRepository.GetAll();
            if (paging.genre != null && !"".Equals(paging.genre)) {
                films = films.Where(f => JArray.Parse(f.Genres).Select(g => g["name"].ToString().ToUpper()).ToList().Contains(paging.genre.ToUpper()));
            }

            if (paging.year != null)
            {
                if (paging.year > 0)
                {
                    films = films.Where(p => (p.Release_date.Year >= paging.year && p.Release_date.Year <= (paging.year + 9)));
                }
                if (paging.year == -1)
                {
                    films = films.Where(p => p.Release_date > DateTime.Now);
                }
               
            }

            if (paging.filmName != null && !"".Equals(paging.filmName))
            {
                films = films.Where(p => (p.Title.ToUpper().Contains(paging.filmName.ToUpper())));
            }

            if (paging.rating != null && !"".Equals(paging.rating))
            {
                if ("asc".ToUpper().Equals(paging.rating.ToUpper()))
                {
                    films = films.OrderBy(p => p.Vote_average);
                }
                if ("desc".ToUpper().Equals(paging.rating.ToUpper()))
                {
                    films = films.OrderByDescending(p => p.Vote_average);
                } 
            }
            int totalCount = films.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / paging.pageSize);
            films = films.Skip((paging.pageIndex - 1) * paging.pageSize).Take(paging.pageSize);
            films = films.ToList();
            List<BaseFilmDTO> filmDTOs= new List<BaseFilmDTO>();
            foreach (Film film in films)
            {
                BaseFilmDTO filmDTO = new BaseFilmDTO();
                filmDTO.FilmID = film.FilmID;
                filmDTO.Poster_path = film.Poster_path;
                filmDTO.Release_date = film.Release_date;
                filmDTO.Title = film.Title;
                filmDTOs.Add(filmDTO);

            }
            res.Data = filmDTOs;
            res.TotalPage = totalPages;
            res.Total = totalCount;
            res.PageSize = paging.pageSize;
            res.PageIndex = paging.pageIndex;
            return res;
        }

        public ListPopularDTO GetListByID(int id)
        {
            var listDetail = _listRepository.GetByID(id);
            if (listDetail == null)
            {
                throw new ServiceException("Không tìm thấy list phù hợp");
            }
            ListPopularDTO res = new ListPopularDTO();
            List<FilmList> filmLists = _filmListRepository.GetAll().Where(p => p.ListID == id).ToList();
            UserReviewDTO userReview = new UserReviewDTO();
            User user = _userRepository.GetByID(listDetail.UserID);
            if (user != null)
            {
                userReview.Avatar = user.Avatar;
                userReview.UserName = user.UserName;
                userReview.UserID = user.UserID;
                userReview.FullName = user.FullName;
            }

            res.ListID = listDetail.ListID;
            res.LikesCount = listDetail.LikesCount;
            res.CommentsCount = listDetail.CommentsCount;
            res.ListName = listDetail.ListName;
            res.Description = listDetail.Description;
            res.CreatedDate = listDetail.CreatedDate;
            res.ModifiedDate = listDetail.ModifiedDate;
            res.Total = filmLists.Count;
            res.User = userReview;
            return res;
        }

        public PagingCommentResult GetCommentList(int ListID, PagingParameter paging)
        {
            PagingCommentResult res = new PagingCommentResult();
            List listDetail = _listRepository.GetByID(ListID);
            if (listDetail == null)
            {
                throw new ServiceException("Không tìm thấy list phù hợp");
            }

            var comments = _commentRepository.GetAll().Where(p => p.ParentID == ListID && "List".Equals(p.Type.ToString()));
            //var comments = _commentRepository.GetAll();

            int totalCount = comments.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / paging.pageSize);
            comments = comments.OrderByDescending(p => p.CreatedDate).Skip((paging.pageIndex - 1) * paging.pageSize).Take(paging.pageSize);
            comments = comments.ToList();
            List<BaseCommentDTO> commentDTOs = new List<BaseCommentDTO>();
            foreach (Comment comment in comments)
            {
                BaseCommentDTO commentDTO = new BaseCommentDTO();
                User user = _userRepository.GetByID(comment.UserID);

                commentDTO.UserID = comment.UserID;
                commentDTO.Avatar = user.Avatar;
                commentDTO.Username = user.UserName;
                commentDTO.Fullname = user.FullName;
                commentDTO.CommentID = comment.CommentID;
                commentDTO.Content = comment.Content;
                commentDTO.CreatedDate = comment.CreatedDate;

                commentDTOs.Add(commentDTO);

            }
            res.Data = commentDTOs;
            res.TotalPage = totalPages;
            res.Total = totalCount;
            res.PageSize = paging.pageSize;
            res.PageIndex = paging.pageIndex;
            return res;
        }
    }
}
