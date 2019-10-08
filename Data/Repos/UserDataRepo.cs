using Data.Abstract;
using Data.Concrete;
using Data.Core;
using Model;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Data.Repos
{
    public class UserDataRepo : EntityBaseSqlServerData<User>, IUserDataRepo 
    {
        public UserDataRepo(DataContext dataContext) : base(dataContext)
        {
        }
    }
}
