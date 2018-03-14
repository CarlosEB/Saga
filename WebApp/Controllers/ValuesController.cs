using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApp.Controllers
{
    public class CreateRequest
    {
        public DateTime SendDate { get; set; }
    }

    public class ValuesController : ApiController
    {
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult PostCreate(CreateRequest request)
        {
            return RandomExecution();
        }

        [HttpPost]
        [Route("Token")]
        public IHttpActionResult PostToken()
        {
            return RandomExecution();
        }

        [HttpPost]
        [Route("Map")]
        public IHttpActionResult PostMap()
        {
            return RandomExecution();
        }

        [HttpPost]
        [Route("Send")]
        public IHttpActionResult PostSend()
        {
            return RandomExecution();
        }

        [HttpPost]
        [Route("Move")]
        public IHttpActionResult PostMove()
        {
            return RandomExecution();
        }

        [HttpPost]
        [Route("Validate")]
        public IHttpActionResult PostValidate()
        {
            return RandomExecution();
        }

        [HttpPost]
        [Route("Mask")]
        public IHttpActionResult PostMask()
        {
            return RandomExecution();
        }


        [HttpPost]
        [Route("MapSendLog")]
        public IHttpActionResult PostMapSendLog()
        {
            return RandomExecution();
        }

        [HttpPost]
        [Route("Cancel")]
        public IHttpActionResult PostCancel()
        {
            return Ok(Guid.NewGuid());
        }

        private IHttpActionResult RandomExecution()
        {
            var rnd = new Random();
            var time = rnd.Next(1, 5) * 1000;
            Task.Delay(time).Wait();
            var error = rnd.Next(1, 100);

            if (error < 60) return Ok(Guid.NewGuid());

            return InternalServerError();
        }
    }
}
