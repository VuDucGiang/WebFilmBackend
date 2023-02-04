using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity> : ControllerBase
    {
        #region Field
        IBaseService<TEntity> _baseService;
        #endregion

        #region Contructor
        public BaseController(IBaseService<TEntity> baseService)
        {
            _baseService = baseService;
        }
        #endregion

        /// <summary>
        /// Lấy toàn bộ dữ liệu T Entity
        /// </summary>
        /// <returns>IActionResult</returns>
        /// Author: Vũ Đức Giang
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var entity = _baseService.GetAll();
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

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
