using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI2.Models;

namespace WebAPI2.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        private FabricsEntities db = new FabricsEntities();

        public ProductsController()
        {
            db.Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// 取得所有商品
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IQueryable<Product> GetProduct()
        {
            return db.Product.OrderByDescending(p=>p.ProductId).Take(10);
        }

        /// <summary>
        /// 取得單一商品
        /// </summary>
        /// <param name="id">商品編號</param>
        /// <returns></returns>        
        [Route("{id}",Name = "GetProductById")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [Route("{id:int}/orderlines")]
        public IHttpActionResult GetProductOrderLines(int id)
        {
            Product product = db.Product.Include("OrderLine").FirstOrDefault(p=> p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.OrderLine.ToList());
        }

        /// <summary>
        ///  修改指定商品編號的內容
        /// </summary>
        /// <param name="id">商品編號</param>
        /// <param name="product"></param>
        /// <returns></returns>
        [Route("{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// 新增一筆商品
        /// </summary>
        /// <param name="product">商品</param>
        /// <returns></returns>
        [Route("")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Product.Add(product);
            db.SaveChanges();

            //return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
            return CreatedAtRoute("GetProductById", new { id = product.ProductId }, product);            
        }

        /// <summary>
        /// 刪除指定商品編號
        /// </summary>
        /// <param name="id">商品編號</param>
        /// <returns></returns>
        [Route("{id}")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Product.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        /// <summary>
        /// OK
        /// </summary>
        /// <param name="id">The ID</param>
        /// <returns></returns>
        private bool ProductExists(int id)
        {
            return db.Product.Count(e => e.ProductId == id) > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        
    }
}