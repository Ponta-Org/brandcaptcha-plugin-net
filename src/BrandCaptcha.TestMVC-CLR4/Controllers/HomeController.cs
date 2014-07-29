using Brandcaptcha;
using System.Web.Mvc;

namespace BrandCaptcha.TestMVC_CLR4.Controllers
{
    public class HomeController : Controller
    {

        [BrandcaptchaControlMvc.CaptchaValidator]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [BrandcaptchaControlMvc.CaptchaValidator]
        public ActionResult Index(bool captchaValid, string captchaErrorMessage)
        {
            TempData["CaptchaValid"] = captchaValid;
            TempData["CaptchaError"] = captchaErrorMessage;
            return RedirectToAction("Index");
        }

    }
}
