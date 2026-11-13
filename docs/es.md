# Village Pack — objetos y arte RPG reutilizables

Village Pack aporta definiciones `rpg_object`, PNG individuales en `rpg_images/`, hojas de caminar, arte de combate, enemigos de nieve y perfiles de ambiente. Depende de `rpg` y `effects_base`. RPG Plus aporta mapas, historias, colocaciones y recompensas.

## Objetos y estaciones

Los materiales incluyen hierba, adoquines, tierra, suelos, senderos, arena, roca, agua y limo de oasis, nieve y hielo. Edificios, interiores, muebles, contenedores, vegetación, luces y NPC son objetos de `objects/*.json`. Un mapa puede combinar objetos de varios paquetes cargados.

La vegetación estacional usa `Variants` de un objeto: `village_pack/vegetation/oak` ofrece `autumn`, `bare` y `winter`; el aspecto base es verano. Elige la variante en el mapa o la colocación. El modelo actual usa `Legend`, `Ground`, `Structure` y `rpg_placement`, sin paletas de tilesets, índices numéricos de tiles ni atlas de sprites.

## Usar el paquete

Incluye `village_pack` en el juego y añade `village_pack>=0.1.0` a las dependencias del mod de contenido. En RPG Map Editor elige superficies, segmentos, sprites o actores. Archivo mínimo `maps/my_glade.json`:

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

Un barril en `placements/my_glade/props.json`:

```json
{
  "Kind": "rpg_placement",
  "Items": [{
    "Id": "my_glade/barrel", "Object": "village_pack/container/barrel",
    "X": 1, "Y": 1
  }]
}
```

También hacen falta una escena `my_glade` y un líder con `Charset`. Las partes usan imágenes como `ground/grass`, relativas a `rpg_images/` y sin `.png`. El ID del objeto, la imagen y la hoja de caminar son claves diferentes.

## Contenedores e interiores

Los contenedores son `village_pack/chest` y `village_pack/container/barrel`. Sus órdenes activan `self.open`; el estado abierto cambia imagen e interacción. Las plantillas no dan botín. Añade recompensas en tu objeto o colocación conservando el flag y el estado para impedir recompensas repetidas.

Las seis habitaciones compartidas y la habitación de la cabaña son objetos. `Footprint` define colisiones; `Parts` usa la imagen completa desde arriba y recortes `quad` limitados a `isometric` mediante `Only`. Las seis habitaciones compartidas ocupan 12 × 10 casillas y tienen una salida sur de dos casillas. El juego define los destinos y las escenas interiores.

## Caminar y combatir

Las hojas tienen 3 columnas y 4 filas (abajo, izquierda, derecha, arriba); la columna central es reposo. Usa `Charset`, por ejemplo `village/hero`, `village/villager` o `snow/white_wolf`. Los retratos usan `Image`, como `characters/village/wolf`; los fondos son `village/meadow`, `village/forest` y `desert/outpost`.

Los enemigos de nieve están en `characters/snow/enemies.json`. XP indica `Exp` antes de calcular la recompensa de combate:

- `village_pack/white_wolf`: Lv 2 · HP 24 · DEF 0 · ATK 5 · XP 14.
- `village_pack/walrus`: Lv 3 · HP 32 · DEF 2 · ATK 5 · XP 20.
- `village_pack/polar_bear`: Lv 4 · HP 40 · DEF 1 · ATK 7 · XP 28.
- `village_pack/yeti`: Lv 5 · HP 48 · DEF 1 · ATK 8 · XP 36.

Todos tienen 1 AP, probabilidad de reclutamiento cero y botín vacío. Los fotogramas del lobo miden 48 × 48 píxeles; los otros, 64 × 64. RPG Plus coloca las patrullas en `placements/rpg_winter/snow_enemies.json` y define sus flags de victoria.

## Ambiente y sustituciones

Los perfiles `woodland`, `winter_snow`, `desert_dust` y `rain` están en `atmospheres/weather.json`. Elige `Atmosphere` o pinta una máscara con Effects del editor de mapas. Effects Engine los reproduce; Effects Editor los edita.

Sustituye una entidad con el mismo tipo e ID, o una imagen con el mismo nombre, desde un mod que dependa del propietario. Usa parches para campos concretos. Las generaciones y fuentes editables se guardan fuera de los repositorios.

![Arte invernal en RPG Plus, vista isométrica](assets/winter_isometric.png)

Si falta un objeto, comprueba `Object`, la clave de imagen y las dependencias. Para pasos bloqueados revisa `Footprint.Solid` y la colisión del mapa. Caminar necesita `Charset`; el fondo de combate necesita `defaultBackground` en la escena.
