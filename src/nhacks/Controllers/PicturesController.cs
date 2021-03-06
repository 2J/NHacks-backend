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
using System;
using nhacks.ViewModels.User;

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
                        var scanned_user = _context.ApplicationUser
                            .Include(u => u.Socials)
                            .SingleOrDefault(u => u.Id == transactionJson.SelectToken("subject").ToString());
                        if(scanned_user != null)
                        {
                            if(!_context.Picture.Where(p => (p.GroupId == int.Parse(scanViewModel.GroupId)) && (p.UserId == User.GetUserId()) && (p.ScannedUserId == scanned_user.Id)).Any())
                            {
                                _context.Picture.Add(new Picture { GroupId = Int32.Parse(scanViewModel.GroupId), UserId = User.GetUserId(), ScannedUserId = scanned_user.Id });
                                var saveResult = await _context.SaveChangesAsync();
                            }
                            return Ok(UserViewModel.ToViewModel(scanned_user));
                        }

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