using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IRFQRepository _rfqRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly IBuyerProfileRepository _buyerProfileRepository;
        private readonly INotificationService _notificationService;

        public QuoteService(IQuoteRepository quoteRepository,
            IRFQRepository rfqRepository,
            ISellerProfileRepository sellerProfileRepository,
            IBuyerProfileRepository buyerProfileRepository,
            INotificationService notificationService)
        {
            _quoteRepository = quoteRepository;
            _rfqRepository = rfqRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _buyerProfileRepository = buyerProfileRepository;
            _notificationService = notificationService;
        }

        public async Task<QuoteDto?> GetByIdAsync(Guid id)
        {
            var quote = await _quoteRepository.GetByIdAsync(id);
            if (quote == null) return null;

            return new QuoteDto
            {
                Id = quote.Id,
                RFQId = quote.RFQId,
                SellerProfileId = quote.SellerProfileId,
                Price = quote.Price,
                DeliveryTime = quote.DeliveryTime,
                Description = quote.Description,
                ValidUntil = quote.ValidUntil,
                Status = quote.Status,
                Conditions = quote.Conditions,
                Notes = quote.Notes ?? string.Empty,
                SubmittedAt = quote.SubmittedAt,
                CreatedAt = quote.CreatedAt
            };
        }

        public async Task<IEnumerable<QuoteDto>> GetByRFQIdAsync(Guid rfqId)
        {
            var quotes = await _quoteRepository.GetByRFQIdAsync(rfqId);
            if (quotes == null)
            {
                return Enumerable.Empty<QuoteDto>();
            }

            return quotes.Select(q => new QuoteDto
            {
                Id = q.Id,
                RFQId = q.RFQId,
                SellerProfileId = q.SellerProfileId,
                Price = q.Price,
                DeliveryTime = q.DeliveryTime,
                Description = q.Description,
                ValidUntil = q.ValidUntil,
                Status = q.Status,
                Conditions = q.Conditions ?? string.Empty,
                Notes = q.Notes ?? string.Empty,
                SubmittedAt = q.SubmittedAt,
                CreatedAt = q.CreatedAt
            });
        }

        public async Task<IEnumerable<QuoteDto>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            var quotes = await _quoteRepository.GetBySellerProfileIdAsync(sellerProfileId);
            if (quotes == null)
            {
                return Enumerable.Empty<QuoteDto>();
            }

            return quotes.Select(q => new QuoteDto
            {
                Id = q.Id,
                RFQId = q.RFQId,
                SellerProfileId = q.SellerProfileId,
                Price = q.Price,
                DeliveryTime = q.DeliveryTime,
                Description = q.Description,
                ValidUntil = q.ValidUntil,
                Status = q.Status,
                Conditions = q.Conditions ?? string.Empty,
                Notes = q.Notes ?? string.Empty,
                SubmittedAt = q.SubmittedAt,
                CreatedAt = q.CreatedAt
            });
        }

        public async Task<QuoteDto> CreateAsync(Guid rfqId, CreateQuoteDto createQuoteDto, Guid sellerProfileId)
        {
            // Validate that the RFQ exists
            var rfq = await _rfqRepository.GetByIdAsync(rfqId);
            if (rfq == null)
            {
                throw new ArgumentException("RFQ not found", nameof(rfqId));
            }

            // Validate that the seller profile exists
            var sellerProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
            if (sellerProfile == null)
            {
                throw new ArgumentException("Seller profile not found", nameof(sellerProfileId));
            }

            // Validate that the seller is a recipient of this RFQ (if recipients are specified)
            if (rfq.Recipients != null && rfq.Recipients.Any() && !rfq.Recipients.Any(r => r.SellerProfileId == sellerProfileId))
            {
                throw new ArgumentException("Seller is not a recipient of this RFQ", nameof(sellerProfileId));
            }

            // Create the quote entity
            var quote = new Quote
            {
                Id = Guid.NewGuid(),
                RFQId = rfqId,
                SellerProfileId = sellerProfileId,
                Price = createQuoteDto.Price > 0 ? createQuoteDto.Price : createQuoteDto.TotalPrice,
                DeliveryTime = createQuoteDto.DeliveryTime,
                Description = createQuoteDto.Description,
                ValidUntil = createQuoteDto.ValidUntil,
                Status = "Pending",
                Conditions = createQuoteDto.Conditions,
                Notes = createQuoteDto.Notes,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var createdQuote = await _quoteRepository.CreateAsync(quote);

            // Update RFQ status to "Responded" if this is the first quote
            if (rfq.Quotes == null || rfq.Quotes.Count == 0)
            {
                rfq.Status = RFQStatus.Responded;
                await _rfqRepository.UpdateAsync(rfq);
            }

            // Send notification to buyer
            await _notificationService.SendRFQResponseNotificationAsync(rfq, quote);

            return new QuoteDto
            {
                Id = createdQuote.Id,
                RFQId = createdQuote.RFQId,
                SellerProfileId = createdQuote.SellerProfileId,
                Price = createdQuote.Price,
                DeliveryTime = createdQuote.DeliveryTime,
                Description = createdQuote.Description,
                ValidUntil = createdQuote.ValidUntil,
                Status = createdQuote.Status,
                Conditions = createdQuote.Conditions ?? string.Empty,
                Notes = createdQuote.Notes ?? string.Empty,
                SubmittedAt = createdQuote.SubmittedAt,
                CreatedAt = createdQuote.CreatedAt
            };
        }

        public async Task<QuoteDto> UpdateAsync(Guid id, CreateQuoteDto updateQuoteDto)
        {
            var quote = await _quoteRepository.GetByIdAsync(id);
            if (quote == null)
            {
                throw new ArgumentException("Quote not found", nameof(id));
            }

            quote.Price = updateQuoteDto.Price > 0 ? updateQuoteDto.Price : updateQuoteDto.TotalPrice;
            quote.DeliveryTime = updateQuoteDto.DeliveryTime ?? quote.DeliveryTime;
            quote.Description = updateQuoteDto.Description ?? quote.Description;
            quote.ValidUntil = updateQuoteDto.ValidUntil != default ? updateQuoteDto.ValidUntil : quote.ValidUntil;
            quote.Conditions = updateQuoteDto.Conditions ?? quote.Conditions;
            quote.Notes = updateQuoteDto.Notes ?? quote.Notes;

            var updatedQuote = await _quoteRepository.UpdateAsync(quote);

            // Send notification to buyer that quote was updated
            await _notificationService.SendQuoteUpdatedNotificationAsync(updatedQuote);

            return new QuoteDto
            {
                Id = updatedQuote.Id,
                RFQId = updatedQuote.RFQId,
                SellerProfileId = updatedQuote.SellerProfileId,
                Price = updatedQuote.Price,
                DeliveryTime = updatedQuote.DeliveryTime,
                Description = updatedQuote.Description,
                ValidUntil = updatedQuote.ValidUntil,
                Status = updatedQuote.Status,
                Conditions = updatedQuote.Conditions ?? string.Empty,
                Notes = updatedQuote.Notes ?? string.Empty,
                SubmittedAt = updatedQuote.SubmittedAt,
                CreatedAt = updatedQuote.CreatedAt
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            await _quoteRepository.DeleteAsync(id);
        }
    }
}