using Data.Abstract;
using Data.Concrete;
using Data.Core;
using Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Data.Repos
{
    public class ProductDataRepo : EntityBaseSqlServerData<Product>, IProductDataRepo
    {
        public ProductDataRepo(DataContext dataContext) : base(dataContext)
        {
        }

        public decimal GetTotalProductPrice()
        {
           return _context.Set<Product>().Sum(x => x.Price);
           
        }
    }
}
