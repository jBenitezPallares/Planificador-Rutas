# PathLab - Planificador de rutas

[![CI](https://github.com/jBenitezPallares/Planificador-Rutas/actions/workflows/ci.yml/badge.svg)](https://github.com/jBenitezPallares/Planificador-Rutas/actions/workflows/ci.yml)

Librería en C# que busca el camino más corto sobre un mapa de grilla y **compara algoritmos** (BFS, Dijkstra y A\*), midiendo cuántas celdas explora cada uno. El foco del proyecto es mostrar estructuras de datos y algoritmos **propios**: el min-heap y la grilla están implementados a mano, sin `PriorityQueue<T>` ni librerías de grafos.

> **Estado:** el núcleo (estructuras, algoritmos, heurísticas y generador aleatorio) está terminado y probado. La interfaz web, el generador de laberintos, la persistencia en JSON y los benchmarks están pendientes (ver [Estado del proyecto](#estado-del-proyecto)).

## Qué incluye hoy

- **Grilla** con celdas de distinto costo y movimiento en 4 u 8 direcciones.
- **Tres algoritmos de búsqueda** con la misma interfaz (`IPathFinder`): BFS, Dijkstra y A\*.
- **Tres heurísticas** para A\*: Manhattan, euclídea y octil.
- **Generador de mapas aleatorios** con porcentajes configurables de muros, barro y agua.
- **Min-heap binario propio** con comparador.
- **99 tests** con MSTest, entre ellos un test de propiedades sobre 200 mapas aleatorios.

## Modelo del mapa

| Tipo de celda | Costo de entrar | Notas |
|---|---|---|
| Libre | 1 | Valor por defecto |
| Barro | 5 | Se puede cruzar, pero cuesta más |
| Agua | 10 | Se puede cruzar, pero cuesta mucho |
| Muro | no transitable | Nunca se explora |

- **4 direcciones:** arriba, abajo, izquierda y derecha.
- **8 direcciones:** suma las diagonales. Una diagonal cuesta el costo de la celda por **1,414** (√2), y **no se permite cortar una esquina** entre dos muros.
- El costo de un camino es la suma de los costos de las celdas a las que se entra, **sin contar la celda de inicio**.

## Algoritmos

Todos implementan `IPathFinder.Find(grid, start, goal)` y devuelven un `SearchResult` con `Found`, `Path`, `Cost`, `ExplorationOrder` y `ElapsedMs`.

| Algoritmo | Qué garantiza | Estructura que usa | Complejidad |
|---|---|---|---|
| **BFS** | Camino con **menos celdas**, ignorando los costos | Cola común | O(V) |
| **Dijkstra** | Camino de **costo mínimo** | `MinHeap` propio | O(V log V) |
| **A\*** | Camino de **costo mínimo**, explorando menos | `MinHeap` propio + heurística | O(V log V) en el peor caso |

(V es la cantidad de celdas.)

BFS solo da el camino de menor costo cuando todas las celdas valen lo mismo. Cuando hay barro o agua, puede devolver un camino con menos celdas pero **más caro**; por eso el `Cost` que informa es el costo real del camino que encontró, y se puede comparar con el de Dijkstra.

### Por qué A\* explora menos celdas que Dijkstra

Dijkstra ordena las celdas pendientes solo por el **costo acumulado** `g` desde el inicio, así que se expande en todas direcciones, como una onda circular, hasta llegar al destino. No tiene ninguna idea de dónde está la meta.

A\* ordena por `f = g + h`, donde `h` es una **estimación** del costo que falta hasta el destino. Con eso, las celdas que van en dirección al objetivo tienen un `f` menor y se expanden primero, y las que se alejan quedan en la cola sin explorarse.

Para que A\* siga siendo óptimo, la heurística tiene que ser **admisible** (nunca sobreestima el costo real) y, como cada celda se cierra una sola vez, **consistente**. Si `h` vale siempre 0, A\* se reduce exactamente a Dijkstra.

### Heurísticas

| Heurística | Fórmula | Se usa con |
|---|---|---|
| **Manhattan** | `dr + dc` | 4 direcciones (exacta en un mapa libre) |
| **Octil** | `max(dr, dc) + (1,414 - 1) × min(dr, dc)` | 8 direcciones (exacta en un mapa libre) |
| **Euclídea** | `√(dr² + dc²)` | Ambos modos, pero está menos informada y explora más |

Manhattan **no** se debe usar con 8 direcciones: una diagonal avanza dos unidades por 1,414 de costo, así que sobreestima y A\* podría devolver un camino que no es el óptimo. `HeuristicFactory.CreateFor(mode)` elige la correcta para cada modo.

## Uso

```csharp
using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms;
using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Generators;
using PathLab.Core.Structures;

// Un mapa aleatorio de 40 x 25 con 20 % de muros, 10 % de barro y 5 % de agua.
MapGenerationOptions options = new MapGenerationOptions(WallPercentage: 0.2, MudPercentage: 0.1, WaterPercentage: 0.05);
Grid grid = new RandomMapGenerator(options, new Random(42)).Generate(40, 25);

// A* en 8 direcciones, con la heurística adecuada para ese modo.
MovementMode mode = MovementMode.EightDirections;
IPathFinder finder = new AStar(mode, HeuristicFactory.CreateFor(mode));

SearchResult result = finder.Find(grid, new Point(0, 0), new Point(24, 39));

Console.WriteLine($"Encontrado: {result.Found}, costo: {result.Cost}, explorados: {result.ExplorationOrder.Count}");
```

Los puntos son `(fila, columna)`. Si el inicio o el destino son un muro, el resultado es `Found = false`; si están fuera de la grilla, se lanza `ArgumentOutOfRangeException`.

## Cómo correrlo

Requiere el **SDK de .NET 10** (la versión está fijada en `global.json`).

```bash
dotnet build PathLab/PathLab.sln
dotnet test PathLab/PathLab.sln
```

## Estructura del proyecto

```
PathLab/
├── PathLab.Core/               Núcleo, sin dependencias de interfaz
│   ├── Abstractions/           Contratos: IPathFinder, IHeuristic, IMapGenerator
│   ├── Algorithms/             PathFinderBase, Dijkstra, BreadthFirstSearch, AStar, SearchResult
│   │   └── Heuristics/         Manhattan, euclídea, octil y HeuristicFactory
│   ├── Generators/             RandomMapGenerator y MapGenerationOptions
│   └── Structures/             Grid, MinHeap, Point, CellType, MovementMode
└── PathLab.Core.Tests/         Tests del núcleo, con la misma estructura de carpetas
```

### Decisiones de diseño

- **Template Method:** `PathFinderBase.Find` valida los puntos, cronometra y chequea los muros; cada algoritmo solo implementa su bucle en `Search`.
- **Strategy:** `IPathFinder` e `IHeuristic` permiten intercambiar algoritmos y heurísticas sin cambiar quien los usa.
- **Tests de contrato:** `PathFinderContractTests` y `HeuristicContractTests` son clases base abstractas con los casos que debe cumplir cualquier implementación. Cada algoritmo hereda de ellas y suma solo lo propio.
- **Azar inyectado:** el generador recibe un `Random`, así que los tests usan una semilla fija y los mapas se pueden reproducir.

## Tests

Los tests cubren, entre otras cosas:

- El mapa a mano con resultado conocido (costo 7 en 4 direcciones) y los casos borde: sin camino, inicio igual a destino, inicio o destino en muro y puntos fuera de la grilla.
- Que A\* y Dijkstra den el **mismo costo** y que A\* explore **menos celdas**.
- La admisibilidad de cada heurística contra el costo real de Dijkstra.
- El **test de propiedades**: sobre 200 mapas aleatorios (semillas 0 a 199), en 4 y en 8 direcciones, A\* y Dijkstra coinciden en `Found` y en el costo.
- Las estructuras: `MinHeap` (los valores salen ordenados), vecinos de la grilla y la regla de no cortar esquinas.

## Estado del proyecto

- [x] `MinHeap`, `Grid`, BFS y Dijkstra con tests
- [x] A\*, heurísticas y test de propiedades
- [x] Generador de mapas aleatorios
- [ ] Generador de laberintos
- [ ] Guardar y cargar mapas en JSON
- [ ] Interfaz web con Blazor (edición de mapa, animación y modo comparar)
- [ ] Benchmarks (50×50, 100×100 y 200×200) y capturas
- [ ] Despliegue

## Benchmarks

_Pendiente._ Acá va a ir la tabla de celdas exploradas y tiempo por algoritmo, medida sobre mapas de 50×50, 100×100 y 200×200 con el mismo porcentaje de muros, con una conclusión de dos o tres líneas.

## Flujo de trabajo

El repositorio sigue **Git Flow**: `main` es lo estable, `develop` integra, y cada cambio va en una rama `feature/*` o `refactor/*` creada desde `develop` y se integra por Pull Request con el CI en verde.
