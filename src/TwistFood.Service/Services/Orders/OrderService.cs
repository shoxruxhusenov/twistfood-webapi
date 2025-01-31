﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TwistFood.DataAccess.Interfaces;
using TwistFood.Domain.Common;
using TwistFood.Domain.Entities.Order;
using TwistFood.Domain.Exceptions;
using TwistFood.Service.Dtos.Orders;
using TwistFood.Service.Interfaces.Orders;

namespace TwistFood.Service.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<Order> OrderCreateAsync(OrderCreateDto dto)
        {
            var user = await _unitOfWork.Users.FindByIdAsync(dto.UserId);
            if (user == null) { throw new StatusCodeException(HttpStatusCode.NotFound, "User not found"); }
            Location location = new Location() { Latitude = dto.Latitude, Longitude = dto.Longitude }; 
            _unitOfWork.Locations.Add(location);
            await _unitOfWork.SaveChangesAsync();
            var Ilocation = await _unitOfWork.Locations.FirstOrDefaultAsync(x => x.Latitude == location.Latitude && x.Longitude == location.Longitude);
            if (Ilocation == null) { throw new StatusCodeException(HttpStatusCode.NotFound, "Location not found"); }

            var order = (Order)dto;
            order.ILocationId = Ilocation.Id;
            _unitOfWork.Orders.Add(order);
            await _unitOfWork.SaveChangesAsync();

            var returnOrder = await _unitOfWork.Orders.FirstOrDefaultAsync(x => x.UserId == order.UserId && x.CreatedAt == order.CreatedAt);
            if (returnOrder == null) { throw new StatusCodeException(HttpStatusCode.NotFound, "Order not found"); }
            return  returnOrder;
        }

        public async Task<bool> OrderUpdateAsync(OrderUpdateDto dto)
        {
            
            var order = await _unitOfWork.Orders.FindByIdAsync(dto.OrderId);
            if (order == null) { throw new StatusCodeException(HttpStatusCode.NotFound, "Order not found"); }

            if (dto.OperatorId is not null)
            {
                var @operator = await _unitOfWork.Operators.FirstOrDefaultAsync(x => x.Id == dto.OperatorId);
                if (@operator == null) { throw new StatusCodeException(HttpStatusCode.NotFound, "Operator not found"); }
                order.OperatorId = @operator.Id;
            }
            if (dto.DeliverId is not null)
            {
                var deliver = await _unitOfWork.Delivers.FirstOrDefaultAsync(x => x.Id == dto.DeliverId);
                if (deliver == null) { throw new StatusCodeException(HttpStatusCode.NotFound, "Deliver not found"); }
                order.DeliverId = deliver.Id;
            }
            if (dto.DeliveredAt is not null)
            {
                order.DeliveredAt = dto.DeliveredAt.Value.ToUniversalTime();
            }
            if (dto.TotalSum is not null) 
            {
                order.TotalSum = dto.TotalSum.Value;    
            } 
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order.Id, order);
            await _unitOfWork.SaveChangesAsync();

            return true;

        }
    }
}
