# Village Pack — arte para mapas RPG y batallas

Village Pack es el kit de arte detrás del mundo de RPG Plus: tilesets para pintar mapas, sprites que caminan para héroes, aldeanos y monstruos, retratos de combate y fondos de batalla. Todo está dibujado en un mismo estilo pictórico 2.5D, así que los mapas hechos con él parecen un solo mundo.

Es **solo arte**. La aldea, la casa del anciano, el bosque y el desierto que puedes recorrer son mapas y eventos de los mods de contenido RPG Demo y RPG Plus; caminar, las colisiones y las batallas son del motor RPG. Village Pack aporta las imágenes que estos usan.

![El mapa de la aldea de noche, pintado con el tileset village: cabañas, una plaza adoquinada con farolas, árboles, un estanque y aldeanos dibujados con los charsets del paquete.](assets/village-map.jpg)

## Qué contiene

### El tileset village

El tileset `village` lo usa cada mapa del mundo de la demo. Tiene dos partes.

Los objetos se colocan sobre casillas: una cabaña, un arco de santuario, la boca de una cueva, árboles, arbustos, un pozo, una hoguera, farolas, una valla, un cartel, barriles, cofres (cerrados y abiertos), una mesa de trabajo y objetos recogibles como leña, piedras y un champiñón. Arriba está el interior de la cabaña, una única imagen de sala de 12 × 10 que usa la casa del anciano.

![El atlas de objetos de la aldea: el interior de la cabaña arriba; debajo, la cabaña, el arco del santuario, la cueva y el roble; en la franja inferior, pequeños objetos.](assets/village_objects.jpg)

Las superficies son texturas de suelo continuas que rellenan áreas enteras: hierba, camino adoquinado, tierra, suelo de piedra clara, suelo de madera, un sendero rocoso de bosque y agua.

![Las siete superficies de la aldea: hierba, adoquines, tierra, piedra clara, tablones de madera, sendero rocoso y agua.](assets/village_surfaces.jpg)

Algunas tiles tienen propiedades extra para el motor RPG: el agua es sólida, y las farolas y ventanas llevan una luz que brilla en el mapa al anochecer y de noche.

![La casa del anciano, hecha con la imagen del interior de la cabaña, iluminada de noche por sus dos ventanas.](assets/elder-house.jpg)

### Vegetación estacional

Cuatro tilesets de vegetación muestran los mismos árboles, arbustos, tocones y flores en verano, otoño, una versión desnuda de finales de otoño e invierno nevado: roble, abeto, abedul, sauce, arce y pino, arbustos redondos y de bayas, tocones cortados y huecos, margaritas y violetas.

![La misma vegetación en cuatro estaciones, de izquierda a derecha: verano, otoño, ramas desnudas e invierno.](assets/vegetation.jpg)

Un mapa usa exactamente un tileset, y ninguno de los mapas incluidos usa todavía los de vegetación. Son material listo para quien escriba mapas.

### Desierto y nieve

- `desert`: superficies de arena y roca con objetos del desierto — edificios de adobe, un toldo de mercado, palmeras, cactus y hierba seca. El mapa desértico de RPG Plus, el Dry March, está pintado con él.
- `snow`: nieve fresca, hielo y dos superficies de sendero pisado (`packed_snow` y el `trampled_snow`, con huellas más marcadas), junto con una cabaña, un carro, un pozo, rocas, arbustos y abetos nevados. Ningún mapa incluido lo usa todavía.

![Las cuatro superficies de nieve: nieve fresca, hielo, nieve apisonada con huellas poco profundas y nieve muy pisoteada.](assets/snow_surfaces.png)

![El mapa del desierto pintado con el tileset desert: arena, un camino rocoso, un toldo, palmeras, cactus, escorpiones y escarabajos.](assets/desert-map.jpg)

### Charsets: sprites que caminan

Cada charset es una hoja de 3 × 4: tres fotogramas de caminar para cada una de las cuatro orientaciones (abajo, izquierda, derecha, arriba). El conjunto de la aldea trae un héroe, un anciano, un aldeano, un mercader, un slime y un lobo. El del desierto añade un escorpión, un escarabajo y un barril en estado cerrado y abierto.

![Los charsets de la aldea: héroe, anciano, aldeano, mercader, slime y lobo, cada uno con cuatro filas de direcciones.](assets/charsets.png)

### Retratos y fondos de combate

Los retratos grandes muestran a los combatientes en batalla: el héroe, el slime y el lobo, y el escorpión y el escarabajo del desierto.

![Retratos de combate: el héroe con espada y escudo, el slime, el lobo, el escorpión y el escarabajo.](assets/portraits.jpg)

Tres fondos —una pradera, un bosque y un puesto del desierto— sirven de fondo a una batalla que empieza en un mapa. La pradera y el bosque traen su compañero `_depth` para el sistema de iluminación.

![Fondos de combate: una pradera, un bosque y un puesto del desierto.](assets/backdrops.jpg)

![Una batalla contra un slime: el retrato del slime arriba, el del héroe abajo a la izquierda, ante el fondo de la pradera de noche.](assets/battle.jpg)

## Dónde lo verás

Village Pack forma parte del juego `rpg_plus`. En ese juego:

- la aldea, la casa del anciano, el bosque y el santuario del bosque están pintados con el tileset `village`;
- el mapa del desierto usa `desert`;
- el anciano, el mercader, los aldeanos, los lobos, los slimes, los escorpiones y los escarabajos recorren los mapas con los charsets del paquete;
- las batallas muestran los retratos, y las escenas de mapa usan `village/meadow`, `village/forest` o `desert/outpost` como fondo de combate.

El editor de mapas muestra el tileset elegido como su paleta. Aquí está abierta la casa del anciano: el interior de la cabaña, los objetos y las muestras de superficie son el tileset village.

![El editor de mapas RPG con la casa del anciano abierta. La paleta de la izquierda muestra el tileset village; las propiedades del mapa a la derecha muestran «Tileset: village».](assets/map-editor-palette.jpg)

## Úsalo en tu contenido RPG

1. Incluye `village_pack` en la colección de tu juego. Requiere el mod de motor `rpg`.
2. Añade `village_pack>=1` a la lista `depends` del mod de contenido que use su arte.
3. Refiérete al arte por su nombre:

```jsonc
// Un mapa: un tileset para todo el mapa
{ "Kind": "rpg_map", "Items": [ { "Id": "my_glade", "Tileset": "village" /* … */ } ] }
```

```jsonc
// Un evento de mapa dibujado con un charset
{ "Id": "old_man", "X": 5, "Y": 3, "Sprite": "village/elder" }
```

```jsonc
// El retrato de combate de un personaje
{ "Id": "my_wolf", "Image": "characters/village/wolf" }
```

```json
{
  "name": "my_glade",
  "defaultBackground": "village/meadow"
}
```

Los nombres de tileset son `village`, `vegetation_summer`, `vegetation_autumn`, `vegetation_bare`, `vegetation_winter`, `desert` y `snow`. Los charsets son `village/<name>` y `desert/<name>`; los fondos de combate, `village/meadow`, `village/forest` y `desert/outpost`.

La forma más sencilla de pintar es el editor de mapas RPG: elige `village` como tileset del mapa y pinta con su paleta en lugar de teclear números de tile.

## Límites y sustitución

- Un tileset por mapa. No puedes mezclar el tileset village con uno de vegetación en el mismo mapa.
- El arte tiene la luz y la perspectiva horneadas. Está dibujado para el aspecto 2.5D; el renderizador isométrico reutiliza las mismas imágenes, así que los objetos muy altos conservan su ángulo pintado.
- Para sustituir una imagen en tu juego, incluye en tu mod un archivo con el mismo nombre relativo y declara dependencia de `village_pack`. Gana el mod dependiente, que se carga después.
- Las generaciones originales, los prompts y las fuentes editables están archivados fuera del juego; el mod contiene solo las imágenes listas para ejecutar y las definiciones de tileset.

## Si algo se ve mal

1. Un mapa sin suelo u objetos suele deberse a un nombre de `Tileset` mal escrito o a una dependencia que falta.
2. Un personaje invisible: comprueba el nombre de `Sprite` y que la hoja del charset sea de 3 × 4 fotogramas.
3. Una batalla con fondo liso: la escena del mapa no tiene `defaultBackground`.
