namespace Emm.Domain.Abstractions;

/// <summary>
/// Giao diện đánh dấu cho các sự kiện miền cần được xuất bản không đồng bộ sau khi giao dịch được xác nhận.
/// Các sự kiện này được xếp hàng trong hộp thư đi và được xử lý bởi một trình xử lý nền.
/// Lỗi trong trình xử lý sẽ không ảnh hưởng đến giao dịch gốc.
///
/// Các trường hợp sử dụng:
/// - Thông báo (email, SMS, thông báo đẩy)
/// - Tích hợp không quan trọng với các hệ thống bên ngoài
/// - Ghi nhật ký và phân tích kiểm tra
/// - Các sự kiện mà tính nhất quán cuối cùng có thể chấp nhận được
/// </summary>
public interface IDeferredDomainEvent : IDomainEvent
{
}
