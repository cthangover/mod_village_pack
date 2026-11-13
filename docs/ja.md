# Village Pack — 再利用できる RPG オブジェクトと画像

Village Pack は `rpg_object` 定義、`rpg_images/` の個別 PNG、歩行画像、戦闘画像、雪国の敵、環境プロファイルを提供します。依存先は `rpg` と `effects_base`。マップ、物語、配置、報酬は RPG Plus が提供します。

## 物体と季節

地表には草、石畳、土、床、森の道、砂、岩地、オアシスの水と泥、雪、氷があります。建物、室内、家具、容器、植物、照明、NPC は `objects/*.json` の通常のオブジェクトです。複数のパックの物体を同じマップに使えます。

季節の植物は同じ物体の `Variants` を使います。`village_pack/vegetation/oak` には `autumn`、`bare`、`winter` があり、基本は夏です。マップまたは配置で指定します。現行モデルは `Legend`、`Ground`、`Structure` と `rpg_placement` を使い、タイルセットのパレット、数値タイル ID、画像アトラスは使いません。

## 利用する

ゲームに `village_pack` を含め、コンテンツ MOD の依存に `village_pack>=0.1.0` を加えます。マップエディタで地表、セグメント、スプライト、アクターから選択します。最小マップ `maps/my_glade.json`：

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

`placements/my_glade/props.json` に置く樽：

```json
{
  "Kind": "rpg_placement",
  "Items": [{
    "Id": "my_glade/barrel", "Object": "village_pack/container/barrel",
    "X": 1, "Y": 1
  }]
}
```

`my_glade` というシーンと `Charset` を持つリーダーも必要です。画像名は `rpg_images/` からの相対名（例：`ground/grass`）で、`.png` は省きます。物体 ID、画像名、歩行画像名は別のキーです。

## 容器と室内

共通の容器は `village_pack/chest` と `village_pack/container/barrel` です。コマンドが `self.open` を立て、開いた状態が画像と操作を変えます。テンプレートに報酬はありません。自分の物体や配置に報酬を足し、開いたフラグと状態を保って重複取得を防ぎます。

共通の 6 室とコテージの部屋はオブジェクトです。`Footprint` が衝突を定め、`Parts` がトップダウンの全体画像と、`Only` で `isometric` に限定した `quad` の切り抜きを使います。共通の 6 室は 12 × 10 マスで南側に 2 マスの開口があります。移動先と室内シーンはゲーム側で定義します。

## 歩行と戦闘

歩行画像は 3 列 × 4 行（下、左、右、上）、中央列が静止です。キャラクターやアクターの `Charset` に `village/hero`、`village/villager`、`snow/white_wolf` などを指定します。戦闘絵は `Image`（例：`characters/village/wolf`）。背景は `village/meadow`、`village/forest`、`desert/outpost` です。

雪国の敵は `characters/snow/enemies.json` にあります。下の XP は戦闘報酬計算前の `Exp` です：

- `village_pack/white_wolf`: Lv 2 · HP 24 · DEF 0 · ATK 5 · XP 14.
- `village_pack/walrus`: Lv 3 · HP 32 · DEF 2 · ATK 5 · XP 20.
- `village_pack/polar_bear`: Lv 4 · HP 40 · DEF 1 · ATK 7 · XP 28.
- `village_pack/yeti`: Lv 5 · HP 48 · DEF 1 · ATK 8 · XP 36.

全員 AP 1、勧誘確率 0、戦利品なしです。狼の歩行フレームは 48 × 48、ほかは 64 × 64 ピクセル。RPG Plus が `placements/rpg_winter/snow_enemies.json` に巡回を配置し、勝利フラグを定義します。

## 環境と置き換え

`woodland`、`winter_snow`、`desert_dust`、`rain` は `atmospheres/weather.json` にあります。マップの `Atmosphere` で選ぶか、マップエディタの効果ツールでマスクを塗ります。再生は Effects Engine、編集は Effects Editor が担当します。

所有元に依存する MOD で、同じ種類と ID のエンティティや同名の画像を提供すると置き換えられます。個別の項目にはパッチを使います。生成元画像と編集可能な素材はリポジトリ外に保管します。

![RPG Plus の冬の画像、等角投影](assets/winter_isometric.png)

物体がない場合は `Object`、画像キー、依存関係を確認します。通れない場合は `Footprint.Solid` とマップの衝突を調べます。歩行には `Charset`、戦闘背景にはマップシーンの `defaultBackground` が必要です。
