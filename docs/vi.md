# Village Pack — vật thể và đồ họa RPG dùng chung

Village Pack cung cấp định nghĩa `rpg_object`, PNG riêng trong `rpg_images/`, sheet đi bộ, hình chiến đấu, quái vùng tuyết và hồ sơ khí quyển. Phụ thuộc `rpg` và `effects_base`. RPG Plus cung cấp bản đồ, truyện, vị trí đặt và phần thưởng.

## Vật thể và mùa

Vật liệu nền gồm cỏ, lát đá, đất, sàn, đường rừng, cát, đá, nước và bùn ốc đảo, tuyết và băng. Nhà, phòng, nội thất, thùng, cây, đèn và NPC là vật thể trong `objects/*.json`. Một bản đồ có thể dùng vật thể từ nhiều gói đã nạp.

Cây theo mùa dùng `Variants` của cùng vật thể: `village_pack/vegetation/oak` có `autumn`, `bare`, `winter`, hình cơ bản là mùa hè. Chọn biến thể trên bản đồ hoặc vị trí đặt. Mô hình hiện tại dùng `Legend`, `Ground`, `Structure` và `rpg_placement`, không dùng bảng tileset, chỉ số tile hay atlas sprite.

## Sử dụng gói

Đưa `village_pack` vào game và thêm `village_pack>=0.1.0` vào phụ thuộc mod nội dung. Trong trình sửa bản đồ chọn bề mặt, đoạn, sprite hoặc actor. Tệp bản đồ tối thiểu `maps/my_glade.json`:

```json
{
  "Kind": "rpg_map",
  "Items": [{
    "Id": "my_glade", "Width": 4, "Height": 3,
    "Legend": {"g": "village_pack/ground/grass"},
    "Ground": ["gggg", "gggg", "gggg"],
    "Spawn": {"X": 0, "Y": 0, "Dir": "Down"}
  }]
}
```

Một thùng trong `placements/my_glade/props.json`:

```json
{
  "Kind": "rpg_placement",
  "Items": [{
    "Id": "my_glade/barrel", "Object": "village_pack/container/barrel",
    "X": 1, "Y": 1
  }]
}
```

Game còn cần cảnh `my_glade` và trưởng nhóm có `Charset`. Hình của các phần dùng tên tương đối với `rpg_images/`, như `ground/grass`, không có `.png`. ID vật thể, tên hình và tên sheet đi bộ là các khóa riêng.

## Thùng và phòng

Thùng dùng chung là `village_pack/chest` và `village_pack/container/barrel`. Lệnh đặt `self.open`; trạng thái mở đổi hình và tương tác. Mẫu không chứa phần thưởng. Thêm thưởng trong vật thể hoặc vị trí của bạn, giữ cờ mở và trạng thái để không nhận lặp.

Sáu phòng kiến trúc chung và phòng nhà nhỏ là vật thể. `Footprint` định nghĩa va chạm; `Parts` dùng toàn hình nhìn từ trên và các mặt cắt `quad` giới hạn ở `isometric` bằng `Only`. Sáu phòng chung có kích thước 12 × 10 ô và cửa phía nam rộng hai ô. Game định nghĩa đích cửa và cảnh bên trong.

## Đi bộ và chiến đấu

Sheet đi bộ có 3 cột × 4 hàng (xuống, trái, phải, lên), cột giữa đứng yên. Dùng `Charset` của nhân vật hoặc actor, như `village/hero`, `village/villager`, `snow/white_wolf`. Chân dung dùng `Image`, như `characters/village/wolf`; nền gồm `village/meadow`, `village/forest`, `desert/outpost`.

Quái tuyết nằm trong `characters/snow/enemies.json`. XP bên dưới là `Exp` trước phép tính thưởng chiến đấu:

- `village_pack/white_wolf`: Lv 2 · HP 24 · DEF 0 · ATK 5 · XP 14.
- `village_pack/walrus`: Lv 3 · HP 32 · DEF 2 · ATK 5 · XP 20.
- `village_pack/polar_bear`: Lv 4 · HP 40 · DEF 1 · ATK 7 · XP 28.
- `village_pack/yeti`: Lv 5 · HP 48 · DEF 1 · ATK 8 · XP 36.

Mỗi con có 1 AP, xác suất tuyển 0 và bảng vật rơi rỗng. Khung đi bộ sói là 48 × 48 pixel, ba con kia 64 × 64. RPG Plus đặt tuần tra trong `placements/rpg_winter/snow_enemies.json` và định nghĩa cờ thắng.

## Khí quyển và thay thế

Các hồ sơ `woodland`, `winter_snow`, `desert_dust`, `rain` nằm trong `atmospheres/weather.json`. Chọn ở `Atmosphere` của bản đồ hoặc tô mặt nạ bằng công cụ hiệu ứng. Effects Engine phát; Effects Editor chỉnh hồ sơ.

Trong mod phụ thuộc chủ sở hữu, dùng cùng loại và ID để thay thực thể hoặc cùng tên tài nguyên để thay hình. Dùng bản vá cho từng trường. Hình tạo gốc và nguồn chỉnh sửa được lưu ngoài kho mã.

![Đồ họa mùa đông RPG Plus, góc đẳng cự](assets/winter_isometric.png)

Nếu thiếu vật thể, kiểm tra `Object`, khóa hình và phụ thuộc đã nạp. Nếu đường bị chặn, kiểm tra `Footprint.Solid` và va chạm bản đồ. Đi bộ cần `Charset`; nền trận cần `defaultBackground` của cảnh bản đồ.
