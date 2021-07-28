using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnlineShop.Controllers
{
    public class productsController : ApiController
    {
        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3 },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16 }
        };

        //public IQueryable<string> Get()
        //{
        //    return new string[] { "value1", "value2" }.AsQueryable();
        //}

        // GET api/products
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //public IEnumerable<Product> GetAllProducts()
        //{
        //    return products;
        //}

        public IHttpActionResult GetProduct(int id)
        {
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public int Price { get; set; }
        }
    }
}
