using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Twilio;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace IVRPhoneTree.Web.Controllers
{
    public class IvrController : TwilioController
    {
        // GET: IVR
        public ActionResult Index()
        {
            return View();
        }

        // POST: IVR/Welcome
        //https://www.twilio.com/docs/api/twiml/say
        [HttpPost]
        public async Task<TwiMLResult> Welcome()
        {
            var response = new TwilioResponse();
            //var survey = _surveysRepository.FirstOrDefault();
            var welcomeMessage = "Welcome to the Cardinal Test I V R demo application.";
            response.Say(welcomeMessage);
            response.Redirect(Url.Action("GetSubjectNumber", "Visits"));

            return TwiML(response);
        }
    }
}