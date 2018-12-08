using System;
using System.Collections.Generic;

namespace NPMGame.Core.Models.Identity
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<Role> Roles { get; set; }
    }
}