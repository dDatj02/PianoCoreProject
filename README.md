# Piano Core Game
A rhythm-based piano game where players hit falling tiles to the music.

# English Guide

## Prerequisites
- Unity 2022.3 LTS or newer
- Android Studio (for Android development)
- Android device with USB debugging enabled (for testing on device)
- Xcode 14.0 or newer (for iOS development)
- macOS computer (required for iOS development)
- Apple Developer account (for distribution)

## Installation & Setup

### Running in Unity Editor
1. Clone the repository:
```bash
git clone https://github.com/yourusername/PianoCoreProject.git
```

2. Open the project in Unity:
   - Open Unity Hub
   - Click "Add" and select the cloned project folder
   - Open the project with Unity 2022.3 LTS or newer

3. Open the main scene:
   - Navigate to `Assets/Scenes/SampleScene.unity`
   - Click Play in the Unity Editor to test

### Running on Android Device
1. Build the APK:
   - In Unity, go to File > Build Settings
   - Select Android platform
   - Click "Switch Platform" if needed
   - Click "Build" and save the APK file

2. Install on Android device:
   - Enable USB debugging on your Android device
   - Connect device to computer via USB
   - Transfer the APK file to your device
   - Open the APK file on your device to install
   - Launch the game from your device's app drawer

### Running on iOS Device
1. Build for iOS:
   - In Unity, go to File > Build Settings
   - Select iOS platform
   - Click "Switch Platform" if needed
   - Click "Build" and select a folder to save the Xcode project

2. Open in Xcode:
   - Navigate to the exported Xcode project folder
   - Open the .xcworkspace file
   - Sign in with your Apple Developer account
   - Select your development team
   - Connect your iOS device via USB
   - Select your device as the build target
   - Click the Play button to build and run

3. Alternative: Build IPA for distribution:
   - In Xcode, go to Product > Archive
   - Once archiving is complete, click "Distribute App"
   - Choose your distribution method (App Store, Ad Hoc, etc.)
   - Follow the prompts to create and export the IPA file

## Game Controls
- Tap the falling tiles in time with the music
- Perfect hits give more points
- Missing tiles will end the game

## Troubleshooting
- If the game doesn't start, ensure all required assets are in the correct folders
- For Android installation issues, check USB debugging settings
- If music doesn't play, check device volume and permissions

---

# Hướng Dẫn Tiếng Việt

## Yêu Cầu Hệ Thống
- Unity 2022.3 LTS hoặc mới hơn
- Android Studio (để phát triển Android)
- Thiết bị Android với chế độ USB debugging được bật (để test trên thiết bị)
- Xcode 14.0 hoặc mới hơn (để phát triển iOS)
- Máy tính macOS (yêu cầu cho phát triển iOS)
- Tài khoản Apple Developer (để phân phối)

## Cài Đặt & Thiết Lập

### Chạy Trong Unity Editor
1. Clone repository:
```bash
git clone https://github.com/yourusername/PianoCoreProject.git
```

2. Mở project trong Unity:
   - Mở Unity Hub
   - Click "Add" và chọn thư mục project đã clone
   - Mở project với Unity 2022.3 LTS hoặc mới hơn

3. Mở scene chính:
   - Điều hướng đến `Assets/Scenes/MainScene.unity`
   - Click Play trong Unity Editor để test

### Chạy Trên Thiết Bị Android
1. Build file APK:
   - Trong Unity, vào File > Build Settings
   - Chọn nền tảng Android
   - Click "Switch Platform" nếu cần
   - Click "Build" và lưu file APK

2. Cài đặt trên thiết bị Android:
   - Bật USB debugging trên thiết bị Android
   - Kết nối thiết bị với máy tính qua USB
   - Chuyển file APK vào thiết bị
   - Mở file APK trên thiết bị để cài đặt
   - Khởi chạy game từ menu ứng dụng

### Chạy Trên Thiết Bị iOS
1. Build cho iOS:
   - Trong Unity, vào File > Build Settings
   - Chọn nền tảng iOS
   - Click "Switch Platform" nếu cần
   - Click "Build" và chọn thư mục để lưu project Xcode

2. Mở trong Xcode:
   - Điều hướng đến thư mục project Xcode đã export
   - Mở file .xcworkspace
   - Đăng nhập bằng tài khoản Apple Developer
   - Chọn development team của bạn
   - Kết nối thiết bị iOS qua USB
   - Chọn thiết bị của bạn làm build target
   - Click nút Play để build và chạy

3. Cách khác: Build IPA để phân phối:
   - Trong Xcode, vào Product > Archive
   - Sau khi archive hoàn tất, click "Distribute App"
   - Chọn phương thức phân phối (App Store, Ad Hoc, v.v.)
   - Làm theo các bước để tạo và export file IPA

## Điều Khiển Game
- Chạm vào các tile rơi theo nhịp nhạc
- Hit Perfect sẽ được nhiều điểm hơn
- Bỏ lỡ tile sẽ kết thúc game

## Xử Lý Sự Cố
- Nếu game không khởi động, kiểm tra xem tất cả assets đã ở đúng thư mục chưa
- Nếu gặp vấn đề cài đặt trên Android, kiểm tra cài đặt USB debugging
- Nếu nhạc không phát, kiểm tra âm lượng và quyền truy cập của thiết bị


## Giải Thích Thiết Kế

### Kiến Trúc Hệ Thống
1. **Quản Lý Game (GameManager)**
   - Điều khiển trạng thái game (bắt đầu, kết thúc, khởi động lại)
   - Quản lý điểm số và xử lý kết quả đánh giá
   - Điều phối tương tác giữa các thành phần khác

2. **Hệ Thống Sinh Tile (SpawnTiles)**
   - Sử dụng MIDI để đồng bộ nhạc và tile
   - Tự động tính toán vị trí sinh tile dựa trên kích thước màn hình
   - Hỗ trợ nhiều làn đường (lanes) tùy chỉnh
   - Sử dụng Object Pooling để tối ưu hiệu suất

3. **Xử Lý Input (InputHandler)**
   - Hỗ trợ cả input cảm ứng và bàn phím
   - Xử lý đa điểm chạm cho mobile
   - Tối ưu hóa cho trải nghiệm người dùng

4. **Hệ Thống Đánh Giá (HitJudgmentSystem)**
   - Phân loại độ chính xác (Perfect, Good, Miss)
   - Tính toán điểm dựa trên độ chính xác
   - Phản hồi trực quan cho người chơi

### Tối Ưu Hóa
1. **Object Pooling**
   - Tái sử dụng tile thay vì tạo/hủy liên tục
   - Giảm tải cho garbage collector
   - Cải thiện hiệu suất game

2. **Tính Toán Vị Trí**
   - Tự động điều chỉnh theo kích thước màn hình
   - Hỗ trợ nhiều tỷ lệ màn hình khác nhau
   - Tối ưu khoảng cách giữa các làn

3. **Xử Lý MIDI**
   - Đọc và xử lý file MIDI hiệu quả
   - Đồng bộ hóa nhạc và gameplay
   - Hỗ trợ nhiều định dạng MIDI

### Mở Rộng
1. **Tính Mở Rộng**
   - Dễ dàng thêm làn đường mới
   - Hỗ trợ nhiều loại tile khác nhau
   - Có thể thêm các chế độ chơi mới

2. **Tùy Biến**
   - Điều chỉnh tốc độ rơi
   - Thay đổi hệ thống tính điểm
   - Tùy chỉnh giao diện người dùng