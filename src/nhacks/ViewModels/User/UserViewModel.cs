using nhacks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nhacks.ViewModels.User
{
    public class UserViewModel
    {
        public string Id { get; set; }

        public virtual ICollection<UserGroup> UserGroups { get; set; }

        public virtual ICollection<Social> Socials { get; set; }

        public string Email { get; set; }

        public static UserViewModel ToViewModel(Models.ApplicationUser user)
        {
            UserViewModel viewModel = new UserViewModel();

            viewModel.Id = user.Id;
            viewModel.Email = user.Email;
            viewModel.UserGroups = user.UserGroups;
            if (viewModel.UserGroups != null)
            {
                foreach (var userGroup in viewModel.UserGroups)
                {
                    userGroup.User = null;
                    if(userGroup.Group != null)
                    {
                        userGroup.Group.UserGroups = null;
                    }
                }
            }

            viewModel.Socials = user.Socials;
            if(viewModel.Socials != null)
            {
                foreach(var social in viewModel.Socials)
                {
                    social.User = null;
                }
            }

            return viewModel;
        }

        public static IEnumerable<UserViewModel> ToViewModel(IEnumerable<Models.ApplicationUser> users)
        {
            foreach (var user in users)
            {
                yield return ToViewModel(user);
            }
        }

        public static Models.ApplicationUser FromViewmodel(UserViewModel viewModel, ApplicationDbContext _context)
        {
            Models.ApplicationUser user = _context.ApplicationUser.SingleOrDefault(m => m.Id == viewModel.Id);

            if (user == null)
            {
                user = new Models.ApplicationUser();
            }

            user.Id = viewModel.Id;
            user.Email = viewModel.Email;
            user.UserGroups = viewModel.UserGroups;
            user.Socials = viewModel.Socials;

            return user;
        }

        public static IEnumerable<Models.ApplicationUser> FromViewmodel(IEnumerable<UserViewModel> viewModels, ApplicationDbContext _context)
        {
            foreach (var viewModel in viewModels)
            {
                yield return FromViewmodel(viewModel, _context);
            }
        }
    }
}
