namespace Emm.Domain.Abstractions;

/// <summary>
/// Giao diện đánh dấu cho các sự kiện miền phải được xuất bản ngay lập tức trong giao dịch hiện tại.
/// Các sự kiện này đại diện cho logic nghiệp vụ quan trọng, phải thành công hoặc thất bại một cách nguyên tử với giao dịch.
/// Nếu bất kỳ trình xử lý nào thất bại, toàn bộ giao dịch sẽ bị hủy bỏ.
///
/// Các trường hợp sử dụng:
/// - Các sự kiện yêu cầu tính nhất quán ngay lập tức (ví dụ: xác thực hàng tồn kho)
/// - Các sự kiện kích hoạt các quy tắc nghiệp vụ quan trọng trong cùng một giao dịch
/// - Các sự kiện mà lỗi sẽ ngăn cản hoạt động hoàn tất
/// </summary>
public interface IImmediateDomainEvent : IDomainEvent
{
}
