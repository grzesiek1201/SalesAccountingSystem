using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;

public interface INumberSequenceRepository
{
    NumberSequence? GetNext(DocumentType type, int year);
    void Add(NumberSequence sequence);
    void Update(NumberSequence sequence);
}