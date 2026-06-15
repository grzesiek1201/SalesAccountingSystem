using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;

namespace AccountingSystem.Application.Services
{
    public class NumberSequenceService
    {
        private readonly INumberSequenceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public NumberSequenceService(
            INumberSequenceRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public string GetNext(DocumentType type)
        {
            int year = DateTime.Now.Year;

            var sequence = _repository.Get(type, year);

            if (sequence == null)
            {
                sequence = new NumberSequence
                {
                    DocumentType = type,
                    Year = year,
                    LastNumber = 1
                };

                _repository.Add(sequence);
            }
            else
            {
                sequence.LastNumber++;
                _repository.Update(sequence);
            }

            _unitOfWork.Save();

            string prefix = GetPrefix(type);

            return $"{prefix}-{year}-{sequence.LastNumber:0000}";
        }

        private string GetPrefix(DocumentType type)
        {
            return type switch
            {
                DocumentType.Quotation => "Q",
                DocumentType.Order => "O",
                DocumentType.Invoice => "I",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}