# Village Pack — 可复用的 RPG 物件与美术

Village Pack 提供 `rpg_object` 定义、`rpg_images/` 中的独立 PNG、行走图、战斗美术、雪地敌人和氛围配置。依赖 `rpg` 与 `effects_base`。地图、剧情、放置实例及奖励由 RPG Plus 提供。

## 物件与季节

地表包括草地、铺路石、泥土、地板、林间小径、沙地、岩地、绿洲水与淤泥、雪和冰。建筑、房间、家具、容器、植物、灯光与 NPC 均为 `objects/*.json` 中的物件。同一地图可混用多个已加载包的物件。

季节植被使用同一物件的 `Variants`：如 `village_pack/vegetation/oak` 有 `autumn`、`bare`、`winter`，基础外观为夏季。在地图或实例上选择变体。当前模型使用 `Legend`、`Ground`、`Structure` 和 `rpg_placement`，不使用图块集调色板、数字图块索引或精灵图集。

## 使用方法

将 `village_pack` 纳入游戏，并在内容模组依赖中加入 `village_pack>=0.1.0`。在 RPG 地图编辑器的地表、线段、精灵或角色页选择物件。最小地图文件 `maps/my_glade.json`：

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

在 `placements/my_glade/props.json` 放置木桶：

```json
{
  "Kind": "rpg_placement",
  "Items": [{
    "Id": "my_glade/barrel", "Object": "village_pack/container/barrel",
    "X": 1, "Y": 1
  }]
}
```

游戏还需要名为 `my_glade` 的场景和具有 `Charset` 的队长。部件使用相对于 `rpg_images/` 的图片名，如 `ground/grass`，不带 `.png`。物件 ID、图片名与行走图名是不同的键。

## 容器与室内

通用容器为 `village_pack/chest` 与 `village_pack/container/barrel`。命令设置 `self.open`，开启状态改变图片及交互。模板不含战利品；在自己的物件或实例中添加奖励，并保留开启标志与状态以避免重复领取。

六个通用建筑房间及小屋房间是物件。`Footprint` 定义碰撞；`Parts` 使用完整俯视图以及通过 `Only` 限定为 `isometric` 的裁剪 `quad` 面。六个通用房间占地 12 × 10，南侧有两格开口。门的目的地与室内场景由游戏定义。

## 行走图与战斗

行走图为 3 列 × 4 行（下、左、右、上），中列为站立。使用角色或演员的 `Charset`，如 `village/hero`、`village/villager`、`snow/white_wolf`。战斗肖像用 `Image`，如 `characters/village/wolf`；背景包括 `village/meadow`、`village/forest`、`desert/outpost`。

雪地敌人位于 `characters/snow/enemies.json`。下面的 XP 是计算战斗奖励前的 `Exp`：

- `village_pack/white_wolf`: Lv 2 · HP 24 · DEF 0 · ATK 5 · XP 14.
- `village_pack/walrus`: Lv 3 · HP 32 · DEF 2 · ATK 5 · XP 20.
- `village_pack/polar_bear`: Lv 4 · HP 40 · DEF 1 · ATK 7 · XP 28.
- `village_pack/yeti`: Lv 5 · HP 48 · DEF 1 · ATK 8 · XP 36.

它们均有 1 AP、零招募概率、空战利品表。狼的行走帧为 48 × 48 像素，其余为 64 × 64。RPG Plus 在 `placements/rpg_winter/snow_enemies.json` 放置巡逻并定义胜利标志。

## 氛围与替换

`woodland`、`winter_snow`、`desert_dust`、`rain` 位于 `atmospheres/weather.json`。在地图 `Atmosphere` 中选择，或用地图编辑器的效果工具绘制遮罩。Effects Engine 负责播放，Effects Editor 负责编辑。

依赖原资源所有者后，可用相同类型与 ID 替换实体，或用同名资源替换图片。单个字段可用补丁修改。原始生成图与可编辑源文件保存在仓库外。

![RPG Plus 冬季美术，等距视角](assets/winter_isometric.png)

物件缺失时检查 `Object`、图片键和依赖。道路受阻时检查 `Footprint.Solid` 和地图碰撞。行走需要正确的 `Charset`；战斗背景需要地图场景的 `defaultBackground`。
