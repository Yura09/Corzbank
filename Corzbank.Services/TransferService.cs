﻿using AutoMapper;
using Corzbank.Data.Models;
using Corzbank.Data.Models.DTOs;
using Corzbank.Data.Enums;
using Corzbank.Helpers.Exceptions;
using Corzbank.Repository.Interfaces;
using Corzbank.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Corzbank.Services
{
    public class TransferService : ITransferService
    {
        private readonly IGenericRepository<Transfer> _transferRepo;
        private readonly IGenericRepository<Card> _cardRepo;
        private readonly IMapper _mapper;

        public TransferService(IGenericRepository<Transfer> transferRepo, IMapper mapper, IGenericRepository<Card> cardRepo)
        {
            _transferRepo = transferRepo;
            _mapper = mapper;
            _cardRepo = cardRepo;
        }

        public async Task<IEnumerable<TransferDetailsDTO>> GetTransfers()
        {
            var transfers = await _transferRepo.GetQueryable().ToListAsync();

            var result = _mapper.Map<IEnumerable<TransferDetailsDTO>>(transfers);

            return result;
        }

        public async Task<TransferDetailsDTO> GetTransferById(Guid id)
        {
            var transfer = await _transferRepo.GetQueryable().FirstOrDefaultAsync(c => c.Id == id);

            var result = _mapper.Map<TransferDetailsDTO>(transfer);

            return result;
        }

        public async Task<IEnumerable<TransferDetailsDTO>> GetTransfersForCard(Guid cardId)
        {
            var transfers = await _transferRepo.GetQueryable().Where(c => c.SenderCardId == cardId || c.ReceiverCardId == cardId).ToListAsync();

            var result = _mapper.Map<IEnumerable<TransferDetailsDTO>>(transfers);

            return result;
        }

        public async Task<TransferDetailsDTO> CreateTransfer(TransferDTO transferRequest)
        {
            if (transferRequest.TransferType == TransferType.Card)
            {
                transferRequest.ReceiverPhoneNumber = null;

                if (string.IsNullOrWhiteSpace(transferRequest.ReceiverCardNumber))
                    throw new ForbiddenException();
            }
            else
            {
                transferRequest.ReceiverCardNumber = null;

                if (string.IsNullOrWhiteSpace(transferRequest.ReceiverPhoneNumber))
                    throw new ForbiddenException();
            }

            var senderCard = await _cardRepo.GetQueryable().FirstOrDefaultAsync(c => c.Id == transferRequest.SenderCardId);

            if (senderCard.Balance <= transferRequest.Amount)
                throw new ForbiddenException();

            senderCard.Balance -= transferRequest.Amount;

            await _cardRepo.Update(senderCard);

            var transfer = _mapper.Map<Transfer>(transferRequest);

            if (!string.IsNullOrWhiteSpace(transferRequest.ReceiverCardNumber))
            {
                var receiverCard = await _cardRepo.GetQueryable().FirstOrDefaultAsync(c => c.CardNumber == transferRequest.ReceiverCardNumber) ?? null;

                if (receiverCard == null || senderCard.Id == receiverCard.Id)
                    throw new ForbiddenException();

                receiverCard.Balance += transferRequest.Amount;

                await _cardRepo.Update(receiverCard);

                transfer.ReceiverCardId = receiverCard.Id;
            }

            transfer.IsSuccessful = true;

            await _transferRepo.Insert(transfer);

            var result = _mapper.Map<TransferDetailsDTO>(transfer);

            return result;
        }

        public async Task<bool> DeleteTransfer(Guid id)
        {
            var transfer = await _transferRepo.GetQueryable().FirstOrDefaultAsync(t => t.Id == id);

            if (transfer != null)
                return false;

            var mappedCard = _mapper.Map<Transfer>(transfer);

            await _transferRepo.Remove(mappedCard);

            return true;
        }
    }
}
