﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Notification;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController<int, Notification>
    {
        #region Field
        INotificationService _notificationService;
        #endregion

        #region Contructor
        public NotificationController(INotificationService notificationService) : base(notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion


        [HttpPost("GetAll")]
        public IActionResult getNoti([FromBody] PagingParameter parameter)
        {
            try
            {
                var res = _notificationService.GetNotification(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}/MarkAsSeen")]
        public IActionResult markAsSeen(int id)
        {
            try
            {
                var res = _notificationService.MarkAsSeen(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
