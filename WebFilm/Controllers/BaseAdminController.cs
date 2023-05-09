using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseAdminController<TKey, TEntity> : ControllerBase
    {
        #region Field
        IBaseService<TKey, TEntity> _baseService;
        protected readonly static Type EntityType = typeof(TEntity);
        #endregion

        #region Contructor
        public BaseAdminController(IBaseService<TKey, TEntity> baseService)
        {
            _baseService = baseService;
        }
        #endregion

        /// <summary>
        /// Lấy toàn bộ dữ liệu T Entity
        /// </summary>
        /// <returns>IActionResult</returns>
        /// Author: Vũ Đức Giang
        

        /// <summary>
        /// Xử lý exception
        /// </summary>
        /// <param name="ex">thông tin exception</param>
        /// <returns>
        /// 500: Xảy ra exception
        /// </returns>
        /// Author: Vũ Đức Giang
        protected IActionResult HandleException(Exception ex)
        {
            var response = new
            {
                devMsg = ex.Message,
                userMsg = Core.Resources.Resource.Error_Exception,
            };
            return StatusCode(500, response);

        }
    }
}
