using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using nhacks.Models;
using System.Security.Claims;
using Microsoft.AspNet.Authorization;
using nhacks.ViewModels.Group;
using nhacks.Services;

namespace nhacks.Controllers
{
    [Produces("application/json")]
    [Route("api/Groups")]
    [Authorize]
    public class GroupsController : Controller
    {
        private ApplicationDbContext _context;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Groups
        [HttpGet("")]
        public async Task<IActionResult> GetGroup()
        {
            IQueryable<Group> groupQuery = _context.Group;
            //.Include(g => g.UserGroups);

            //return only groups that user is part of
            var groups = groupQuery.Where(g => g.UserGroups.Where(ug => ug.UserId == User.GetUserId()).Any()).ToList();
            return Ok(GroupViewModel.ToViewModel(groups));
        }

        // GET: api/Groups/5
        [HttpGet("{id}", Name = "GetGroup")]
        public async Task<IActionResult> GetGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Group group = await _context.Group
                .Include(g => g.UserGroups)
                .ThenInclude(ug => ug.User)
                .SingleAsync(m => m.Id == id);
            if (group == null)
            {
                return HttpNotFound();
            }

            return Ok(GroupViewModel.ToViewModel(group));
        }

        // PUT: api/Groups/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup([FromRoute] int id, [FromBody] Group group)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            //TODO: check if group owner

            if (id != group.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    throw;
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/Groups
        [HttpPost]
        public async Task<IActionResult> PostGroup([FromBody] Group group)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            group.OwnerId = User.GetUserId();

            _context.Group.Add(group);

            var userGroup = new UserGroup { UserId = User.GetUserId(), GroupId = group.Id };
            _context.UserGroup.Add(userGroup);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GroupExists(group.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetGroup", new { id = group.Id }, GroupViewModel.ToViewModel(group));
        }

        // POST: api/Groups/join
        [HttpPost("join")]
        public async Task<IActionResult> PostJoinGroup([FromBody] GroupJoinViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            var group = _context.Group.Where(g => g.Name.ToLower() == viewModel.Name.ToLower() && g.Password == viewModel.Password).SingleOrDefault();

            if(group == null)
            {
                return new HttpStatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var userGroup = new UserGroup { UserId = User.GetUserId(), GroupId = group.Id };
            _context.UserGroup.Add(userGroup);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
            }

            return CreatedAtRoute("GetGroup", new { id = group.Id }, GroupViewModel.ToViewModel(group));
        }

        // GET: api/Groups/1/scanned
        [HttpGet("{id}/scanned", Name = "GetScannedGroup")]
        public async Task<IActionResult> GetScannedGroup([FromRoute] int id)
        {
            //TODO: Check if user is in group
            var pictures = await _context.Picture
                .Include(p => p.ScannedUser)
                .Where(p => p.UserId == User.GetUserId() && p.GroupId == id)
                .ToListAsync();

            return Ok(pictures);
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            //TODO: Check if group owner

            Group group = await _context.Group.SingleAsync(m => m.Id == id);
            if (group == null)
            {
                return HttpNotFound();
            }

            _context.Group.Remove(group);
            await _context.SaveChangesAsync();

            return Ok(GroupViewModel.ToViewModel(group));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(int id)
        {
            return _context.Group.Count(e => e.Id == id) > 0;
        }
    }
}