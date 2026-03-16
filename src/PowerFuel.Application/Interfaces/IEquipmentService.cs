using PowerFuel.Application.DTOs.Equipment;

namespace PowerFuel.Application.Interfaces;

public interface IEquipmentService
{
    Task<EquipmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EquipmentListDto>> ListAsync(string? categorySlug = null, CancellationToken cancellationToken = default);
}
