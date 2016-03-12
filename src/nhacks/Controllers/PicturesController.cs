using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using nhacks.Models;
using Microsoft.AspNet.Authorization;
using nhacks.ViewModels.Picture;
using nhacks.Services;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace nhacks.Controllers
{
    [Produces("application/json")]
    [Route("api/Pictures")]
    [Authorize]
    public class PicturesController : Controller
    {
        private ApplicationDbContext _context;

        public PicturesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Pictures/add
        [HttpPost("add")]
        public async Task<IActionResult> PostAdd([FromBody] ScanViewModel scanViewModel)
        {
            //TODO: make sure that user is in group and image data is valid
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var kairos = new Kairos();
            var result = kairos.Enroll(scanViewModel.ImgData, User.GetUserId(), scanViewModel.GroupId);
            if (result.IsSuccessStatusCode)
            {
                var content = result.Content;
                var contentResult = content.ReadAsStringAsync().Result;
                JContainer jsonObject = JsonConvert.DeserializeObject<JContainer>(contentResult);
                var transactionJson = jsonObject.SelectToken("images[0].transaction");
                if (transactionJson != null)
                {
                    //transactionJson.SelectToken("");
                    return new HttpStatusCodeResult(StatusCodes.Status200OK); //TODO: Add scanned face to DB

                }
                else
                {
                    return new HttpStatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        // POST: api/Pictures/add
        [HttpPost("scan")]
        public async Task<IActionResult> PostScan([FromBody] ScanViewModel scanViewModel)
        {
            //TODO: make sure that user is in group and image data is valid
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var kairos = new Kairos();
            var result = kairos.Recognize(scanViewModel.ImgData, scanViewModel.GroupId);
            if (result.IsSuccessStatusCode)
            {
                var content = result.Content;
                var contentResult = content.ReadAsStringAsync().Result;
                JContainer jsonObject = JsonConvert.DeserializeObject<JContainer>(contentResult);
                var transactionJson = jsonObject.SelectToken("images[0].transaction");
                if (transactionJson != null)
                {
                    if (float.Parse(transactionJson.SelectToken("confidence").ToString()) > 0.5) //FIXME: CONFIDENCE LEVEL REQUIRED FOR MATCH
                    {
                        return Ok(_context.ApplicationUser.Single(u => u.Id == transactionJson.SelectToken("subject").ToString()));
                    }
                    else
                    {
                        return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
                    }
                    //transactionJson.SelectToken("");
                    return new HttpStatusCodeResult(StatusCodes.Status200OK); //TODO: Add scanned face to DB

                }
                else
                {
                    return new HttpStatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PictureExists(int id)
        {
            return _context.Picture.Count(e => e.Id == id) > 0;
        }
    }
}