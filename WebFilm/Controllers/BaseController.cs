using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TKey, TEntity> : ControllerBase
    {
        #region Field
        IBaseService<TKey, TEntity> _baseService;
        protected readonly static Type EntityType = typeof(TEntity);
        #endregion

        #region Contructor
        public BaseController(IBaseService<TKey, TEntity> baseService)
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
        /// Lấy dữ liệu T Entity theo id
        /// </summary>
        /// <returns>IActionResult</returns>
        /// Author: Vũ Đức Giang
        [HttpGet("{id}")]
        public IActionResult GetByID(TKey id)
        {
            try
            {
                var entity = _baseService.GetByID(id);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Sửa dữ liệu theo id
        /// </summary>
        /// <returns>IActionResult</returns>
        /// Author: Vũ Đức Giang
        [HttpPut("{id}")]
        public IActionResult Edit(TKey id, TEntity entity)
        {
            try
            {
                var res = _baseService.Edit(id, entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Thêm dữ liệu mới
        /// </summary>
        /// <returns>IActionResult</returns>
        /// Author: Vũ Đức Giang
        [HttpPost]
        public IActionResult Add(TEntity entity)
        {
            try
            {
                var res = _baseService.Add(entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Xóa dữ liệu theo id
        /// </summary>
        /// <returns>IActionResult</returns>
        /// Author: Vũ Đức Giang
        [HttpDelete]
        public IActionResult Delete(TKey id)
        {
            try
            {
                var res = _baseService.Delete(id);
                return Ok(res);
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
