﻿using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        //base(db):تعني تمرير المتغير db الى المشيدة في كلاس الابRepository 
        //هذه هي طريقة تمرير مشيدة المورث من مشيدة الوارث
        public OrderHeaderRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }
    

        public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb=_db.OrderHeaders.FirstOrDefault(x => x.Id == id);
		    if(orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if(!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus= paymentStatus;
                }
            }
        }

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb=_db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId= paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
		}
	}
}
