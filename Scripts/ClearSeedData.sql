-- Chạy script này trên SQL Server nếu cần xóa sạch dữ liệu demo / seed tĩnh.
-- Sau khi chạy, mọi dữ liệu chỉ còn do user tự đăng ký / tạo qua API.

DELETE FROM [ApprovalHistory]
WHERE ExpenseId IN (SELECT Id FROM [Expense] WHERE UserId IN (SELECT Id FROM [User] WHERE UserName IN (N'user_demo', N'admin_demo', N'staff_demo')))
   OR AdminId IN (SELECT Id FROM [User] WHERE UserName IN (N'admin_demo', N'staff_demo'));

DELETE FROM [Expense]
WHERE UserId IN (SELECT Id FROM [User] WHERE UserName IN (N'user_demo', N'admin_demo', N'staff_demo'));

DELETE FROM [BudgetDetail]
WHERE BudgetId IN (SELECT Id FROM [Budget] WHERE UserId IN (SELECT Id FROM [User] WHERE UserName = N'user_demo'));

DELETE FROM [Budget]
WHERE UserId IN (SELECT Id FROM [User] WHERE UserName = N'user_demo');

DELETE FROM [Wallet]
WHERE UserId IN (SELECT Id FROM [User] WHERE UserName = N'user_demo');

DELETE FROM [Notification]
WHERE UserId IN (SELECT Id FROM [User] WHERE UserName IN (N'user_demo', N'admin_demo', N'staff_demo'));

DELETE FROM [Category]
WHERE OwnerUserId IN (SELECT Id FROM [User] WHERE UserName IN (N'user_demo', N'admin_demo', N'staff_demo'));

DELETE FROM [Category]
WHERE CompanyId IN (SELECT Id FROM [Company] WHERE Name = N'Công ty Demo ABC');

DELETE FROM [Account]
WHERE CompanyId IN (SELECT Id FROM [Company] WHERE Name = N'Công ty Demo ABC');

DELETE FROM [User]
WHERE UserName IN (N'user_demo', N'admin_demo', N'staff_demo');

DELETE FROM [Company]
WHERE Name = N'Công ty Demo ABC';

-- Danh mục hệ thống seed (Ăn uống, Di chuyển...) — chỉ xóa nếu chưa được dùng
DELETE FROM [Category]
WHERE Id IN (1, 2, 3, 4, 5)
  AND OwnerUserId IS NULL
  AND CompanyId IS NULL
  AND NOT EXISTS (SELECT 1 FROM [Expense] WHERE CategoryId = [Category].Id)
  AND NOT EXISTS (SELECT 1 FROM [Budget] WHERE CategoryId = [Category].Id);
