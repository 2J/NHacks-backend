using nhacks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nhacks.ViewModels.Group
{
    public class GroupViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Password { get; set; } //this is in plaintext (lol)

        // connected users
        public virtual ICollection<UserGroup> UserGroups { get; set; }

        public static GroupViewModel ToViewModel(Models.Group group)
        {
            GroupViewModel viewModel = new GroupViewModel();

            viewModel.Id = group.Id;
            viewModel.Name = group.Name;
            viewModel.Description = group.Description;
            viewModel.Password = group.Password;
            viewModel.UserGroups = group.UserGroups;
            if(viewModel.UserGroups != null)
            {
                foreach (var userGroup in viewModel.UserGroups)
                {
                    userGroup.Group = null;
                    if (userGroup.User != null) //TODO: Change to user view model
                    {
                        userGroup.User.UserGroups = null;
                    }
                }
            }

            return viewModel;
        }

        public static IEnumerable<GroupViewModel> ToViewModel(IEnumerable<Models.Group> groups)
        {
            foreach (var group in groups)
            {
                yield return ToViewModel(group);
            }
        }

        public static Models.Group FromViewmodel(GroupViewModel viewModel, ApplicationDbContext _context)
        {
            Models.Group group = _context.Group.SingleOrDefault(m => m.Id == viewModel.Id);

            if (group == null)
            {
                group = new Models.Group();
            }

            group.Id = viewModel.Id;
            group.Name = viewModel.Name;
            group.Description = viewModel.Description;
            group.Password = viewModel.Password;
            group.UserGroups = viewModel.UserGroups;

            return group;
        }

        public static IEnumerable<Models.Group> FromViewmodel(IEnumerable<GroupViewModel> viewModels, ApplicationDbContext _context)
        {
            foreach (var viewModel in viewModels)
            {
                yield return FromViewmodel(viewModel, _context);
            }
        }
    }
}
