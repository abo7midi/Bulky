using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        //base(db):تعني تمرير المتغير db الى المشيدة في كلاس الابRepository 
        //هذه هي طريقة تمرير مشيدة المورث من مشيدة الوارث
        public ApplicationUserRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }

    }
}
