﻿using Bookshelf.Data;
using Bookshelf.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly BookshelfDbContext _context;

        public TransactionController(BookshelfDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public IActionResult AddTransaction([FromBody] Transaction newTransaction)
        {
            var book = _context.Books
                .FirstOrDefault(b => b.BookId == newTransaction.BookId);

            var sellerId = book.SellerId;
            var TotalAmount = book.Price;
            newTransaction.SellerId = sellerId;
            newTransaction.TotalAmount = TotalAmount;
            _context.Transactions.Add(newTransaction);
            book.IsSold = true;
            _context.SaveChanges();

            var createdTransaction = _context.Transactions
                .Where(item => item.TransactionId == newTransaction.TransactionId)
                    .Select(item => new
                    {
                        item.TransactionId,
                        item.BookId,
                        item.BuyerId,
                        item.SellerId,
                        item.TransactionDate,
                        item.TotalAmount,
                        item.DeliveryCharge,
                        item.PlatformFee
                    })
                    .FirstOrDefault();

            return CreatedAtAction("Get", new { id = newTransaction.TransactionId }, createdTransaction);
        }

        [HttpGet("getOrders")]
        public IActionResult GetAll(int buyerId)
        {
            if (buyerId == default(int))
            {
                return BadRequest(new { error = "buyerId is required in the query." });
            }

            var data = _context.Transactions
                .Where(item => item.BuyerId == buyerId)
                .Select(item => new
                {
                    item.TransactionId,
                    item.BuyerId,
                    item.BookId,
                    item.TransactionDate,
                    BookTitle = item.Book.Title,
                    TotalAmount = item.TotalAmount+item.DeliveryCharge+item.PlatformFee
                })
                .ToList();

            return Ok(data);
        }
    }
}
