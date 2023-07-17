using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Product product)
        {
            Product prod = _db.Products.FirstOrDefault(pro=>pro.Id==product.Id);
            if (prod!=null)
            {
                prod.Title = product.Title;
                prod.Description = product.Description;
                prod.ISBN = product.ISBN;
                prod.Author = product.Author;
                prod.ListPrice = product.ListPrice;
                prod.Price = product.Price;
                prod.Price100 = product.Price100;
                prod.Price50 = product.Price50;
                prod.CategoryId = product.CategoryId;
                if(product.ImageUrl != null)
                {
                    prod.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
