using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LongNumber.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LongNumberController : ControllerBase
    {
        [HttpGet]
        [Route("Convert")]
        public IActionResult Convert([Required] string number)
        {
            try
            {
                return Ok(LongNumber.ConvertToLongForm(number));
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 400);
            }
        }

        [HttpGet]
        [Route("ConvertCurrency")]
        public IActionResult ConvertCurrency([Required] string number)
        {
            try
            {
                return Ok(LongNumber.ConvertCurrencyToLongForm(number));
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 400);
            }
        }
    }
}
