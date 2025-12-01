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
    public class RFQService : IRFQService
    {
        private readonly IRFQRepository _rfqRepository;
        private readonly IBuyerProfileRepository _buyerProfileRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly IRFQRecipientRepository _rfqRecipientRepository;
        private readonly INotificationService _notificationService;

        public RFQService(IRFQRepository rfqRepository,
            IBuyerProfileRepository buyerProfileRepository,
            ISellerProfileRepository sellerProfileRepository,
            IRFQRecipientRepository rfqRecipientRepository,
            INotificationService notificationService)
        {
            _rfqRepository = rfqRepository;
            _buyerProfileRepository = buyerProfileRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _rfqRecipientRepository = rfqRecipientRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<RFQDto>> GetAllAsync()
        {
            var rfqs = await _rfqRepository.GetAllAsync();
            return rfqs.Select(r => new RFQDto
            {
                Id = r.Id,
                BuyerProfileId = r.BuyerProfileId,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                ClosedAt = r.ClosedAt
            });
        }

        public async Task<RFQDto?> GetByIdAsync(Guid id)
        {
            var rfq = await _rfqRepository.GetByIdAsync(id);
            if (rfq == null) return null;

            return new RFQDto
            {
                Id = rfq.Id,
                BuyerProfileId = rfq.BuyerProfileId,
                Title = rfq.Title,
                Description = rfq.Description,
                Status = rfq.Status,
                CreatedAt = rfq.CreatedAt,
                ClosedAt = rfq.ClosedAt
            };
        }

        public async Task<IEnumerable<RFQDto>> GetByBuyerProfileIdAsync(Guid buyerProfileId)
        {
            var rfqs = await _rfqRepository.GetByBuyerProfileIdAsync(buyerProfileId);
            return rfqs.Select(r => new RFQDto
            {
                Id = r.Id,
                BuyerProfileId = r.BuyerProfileId,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                ClosedAt = r.ClosedAt
            });
        }

        public async Task<IEnumerable<RFQDto>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            var rfqs = await _rfqRepository.GetBySellerProfileIdAsync(sellerProfileId);
            return rfqs.Select(r => new RFQDto
            {
                Id = r.Id,
                BuyerProfileId = r.BuyerProfileId,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                ClosedAt = r.ClosedAt
            });
        }

        public async Task<RFQDto> CreateAsync(CreateRFQDto createRFQDto, Guid buyerProfileId)
        {
            // Validate that the buyer profile exists
            var buyerProfile = await _buyerProfileRepository.GetByIdAsync(buyerProfileId);
            if (buyerProfile == null)
            {
                throw new ArgumentException("Buyer profile not found", nameof(buyerProfileId));
            }

            // Validate that the recipient seller profiles exist (if any provided)
            if (createRFQDto.RecipientIds != null && createRFQDto.RecipientIds.Any())
            {
                var validSellers = await _sellerProfileRepository.GetByIdsAsync(createRFQDto.RecipientIds);
                if (validSellers.Count() != createRFQDto.RecipientIds.Count)
                {
                    throw new ArgumentException("One or more seller profiles not found", nameof(createRFQDto.RecipientIds));
                }
            }

            // Create the main RFQ entity
            var rfq = new RFQ
            {
                Id = Guid.NewGuid(),
                BuyerProfileId = buyerProfileId,
                Title = createRFQDto.Title,
                Description = createRFQDto.Description,
                Status = RFQStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            // Save the RFQ first to get the ID
            var createdRFQ = await _rfqRepository.CreateAsync(rfq);

            // Create RFQ recipients (if any provided)
            var recipients = new List<RFQRecipient>();
            if (createRFQDto.RecipientIds != null && createRFQDto.RecipientIds.Any())
            {
                foreach (var recipientId in createRFQDto.RecipientIds)
                {
                    var rfqRecipient = new RFQRecipient
                    {
                        Id = Guid.NewGuid(),
                        RFQId = createdRFQ.Id,
                        SellerProfileId = recipientId
                    };

                    var createdRecipient = await _rfqRecipientRepository.CreateAsync(rfqRecipient);
                    recipients.Add(createdRecipient);
                }

                // Send notifications to all recipients
                foreach (var recipientId in createRFQDto.RecipientIds)
                {
                    await _notificationService.SendRFQReceivedNotificationAsync(createdRFQ, recipientId);
                }
            }

            return new RFQDto
            {
                Id = createdRFQ.Id,
                BuyerProfileId = createdRFQ.BuyerProfileId,
                Title = createdRFQ.Title,
                Description = createdRFQ.Description,
                Status = createdRFQ.Status,
                CreatedAt = createdRFQ.CreatedAt,
                ClosedAt = createdRFQ.ClosedAt
            };
        }

        public async Task<RFQDto> UpdateStatusAsync(Guid id, UpdateRFQStatusDto updateRFQStatusDto)
        {
            var rfq = await _rfqRepository.GetByIdAsync(id);
            if (rfq == null)
            {
                throw new ArgumentException("RFQ not found", nameof(id));
            }

            rfq.Status = updateRFQStatusDto.Status;
            if (updateRFQStatusDto.Status == RFQStatus.Closed)
            {
                rfq.ClosedAt = DateTime.UtcNow;
            }

            var updatedRFQ = await _rfqRepository.UpdateAsync(rfq);

            return new RFQDto
            {
                Id = updatedRFQ.Id,
                BuyerProfileId = updatedRFQ.BuyerProfileId,
                Title = updatedRFQ.Title,
                Description = updatedRFQ.Description,
                Status = updatedRFQ.Status,
                CreatedAt = updatedRFQ.CreatedAt,
                ClosedAt = updatedRFQ.ClosedAt
            };
        }

        public async Task<RFQDto> CloseRFQAsync(Guid id, Guid buyerProfileId)
        {
            var rfq = await _rfqRepository.GetByIdAsync(id);
            if (rfq == null)
            {
                throw new ArgumentException("RFQ not found", nameof(id));
            }

            // Verify that the buyer profile matches the one that created the RFQ
            if (rfq.BuyerProfileId != buyerProfileId)
            {
                throw new UnauthorizedAccessException("Buyer is not authorized to close this RFQ");
            }

            if (rfq.Status != RFQStatus.Open)
            {
                throw new InvalidOperationException("Only open RFQs can be closed");
            }

            rfq.Status = RFQStatus.Closed;
            rfq.ClosedAt = DateTime.UtcNow;

            var updatedRFQ = await _rfqRepository.UpdateAsync(rfq);

            return new RFQDto
            {
                Id = updatedRFQ.Id,
                BuyerProfileId = updatedRFQ.BuyerProfileId,
                Title = updatedRFQ.Title,
                Description = updatedRFQ.Description,
                Status = updatedRFQ.Status,
                CreatedAt = updatedRFQ.CreatedAt,
                ClosedAt = updatedRFQ.ClosedAt
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            await _rfqRepository.DeleteAsync(id);
        }
    }
}