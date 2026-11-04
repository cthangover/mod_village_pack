# Village Pack — reusable RPG objects and art

Village Pack supplies `rpg_object` definitions, individual PNGs in `rpg_images/`, walking sheets, battle art, snow enemies and atmosphere profiles. It depends on `rpg` and `effects_base`. RPG Plus supplies the maps, stories, placements and rewards.

## Objects and seasons

Ground materials include grass, paving, earth, floors, forest paths, desert sand, rocky ground, oasis water/silt, snow and ice. Buildings, room interiors, furniture, containers, vegetation, lights and NPCs are ordinary objects in `objects/*.json`. A map can use objects from several loaded packs together.

Seasonal vegetation uses one object with `Variants`: for example `village_pack/vegetation/oak` has `autumn`, `bare` and `winter`; the base look is summer. Select the variant on the map or placement. The current model has no tileset palette, numeric tile IDs or sprite atlas: maps use `Legend`, `Ground` and `Structure`, and scenery uses `rpg_placement`.

## Use the pack

Include `village_pack` in the game and add `village_pack>=1` to the content mod’s dependencies. In RPG Map Editor choose an object from Surfaces, Segments, Sprites or Actors. A minimal map file `maps/my_glade.json`:

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

A barrel in `placements/my_glade/props.json`:

```json
{
  "Kind": "rpg_placement",
  "Items": [{
    "Id": "my_glade/barrel", "Object": "village_pack/container/barrel",
    "X": 1, "Y": 1
  }]
}
```

The game also needs a scene named `my_glade` and a party leader with a `Charset`. Sprite parts refer to pictures such as `ground/grass` relative to `rpg_images/`, without `.png`. Object IDs, image names and walking-sheet names are separate keys.

## Containers and interiors

Reusable containers are `village_pack/chest` and `village_pack/container/barrel`. Their commands set `self.open`; the open state changes the picture and interaction. Templates have no loot. Add rewards in your own object or placement, preserving the opening flag and state so repeated use gives nothing.

The six shared architectural rooms and the cottage room are objects, not maps. Their `Footprint` defines collision; `Parts` uses a whole top-down picture and cropped `quad` faces restricted to `isometric` with `Only`. Six shared rooms have 12 × 10 footprints and two-cell southern openings. Door destinations and interior scenes belong to the game.

## Walking sheets and battles

Walking sheets are 3 columns × 4 rows (down, left, right, up), with the middle column idle. Use a character’s or actor’s `Charset`, such as `village/hero`, `village/villager` or `snow/white_wolf`. Battle portraits use `Image`, such as `characters/village/wolf`; scene backgrounds include `village/meadow`, `village/forest` and `desert/outpost`.

Snow enemies are defined in `characters/snow/enemies.json`. Below, XP is the character’s `Exp` value, before the battle reward calculation:

- `village_pack/white_wolf`: Lv 2 · HP 24 · DEF 0 · ATK 5 · XP 14.
- `village_pack/walrus`: Lv 3 · HP 32 · DEF 2 · ATK 5 · XP 20.
- `village_pack/polar_bear`: Lv 4 · HP 40 · DEF 1 · ATK 7 · XP 28.
- `village_pack/yeti`: Lv 5 · HP 48 · DEF 1 · ATK 8 · XP 36.

Each has 1 AP, no recruitment chance and an empty loot table. The wolf’s walking frames are 48 × 48 pixels; the other three use 64 × 64. RPG Plus places the patrols in `placements/rpg_winter/snow_enemies.json` and owns their victory flags.

## Atmosphere and replacement

Profiles `woodland`, `winter_snow`, `desert_dust` and `rain` live in `atmospheres/weather.json`. Select one in the map’s `Atmosphere`, or paint a mask with the map editor’s Effects tool. Effects Engine owns playback; Effects Editor edits the profiles.

Replace an entity by its same kind and ID, or a picture by the same resource name, in a mod that depends on the owner. Use patches for individual fields. Raw generations and editable sources remain outside repositories; only runtime assets belong here.

![Winter art in RPG Plus, isometric view](assets/winter_isometric.png)

If an object is missing, check the exact `Object` ID, image key and loaded dependencies. If walking is blocked, inspect `Footprint.Solid` and the map’s collision. A walking sprite needs a valid `Charset`; a battle backdrop needs the map scene’s `defaultBackground`.
