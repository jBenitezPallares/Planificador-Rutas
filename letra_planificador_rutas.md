# Proyecto: Planificador de rutas (PathLab)

Aplicación web que busca el camino más corto sobre un mapa de grilla y **compara algoritmos** (BFS, Dijkstra y A*) mostrando cuántas celdas explora cada uno. El foco del proyecto es demostrar estructuras de datos y algoritmos propios, con un resultado visual.

- **Duración estimada:** 3 semanas, 25 a 30 horas.
- **Stack:** C# / .NET 10, Blazor Server para la interfaz, MSTest para los tests, GitHub Actions para CI.
- **Restricción clave:** el min-heap y el grafo los implementás vos. No se usa `PriorityQueue<T>` ni librerías de grafos en el núcleo.

---

## 1. Qué hace la aplicación

1. Muestra una grilla editable (por defecto 40 × 25 celdas).
2. El usuario puede poner muros, elegir el inicio y el destino, y pintar celdas con costos distintos.
3. Elige un algoritmo y ejecuta la búsqueda. La interfaz anima el orden en que se exploran las celdas y dibuja el camino final.
4. Muestra estadísticas: costo del camino, largo, celdas exploradas y tiempo en milisegundos.
5. Modo **comparar**: corre dos algoritmos sobre el mismo mapa, lado a lado.
6. Genera mapas aleatorios y laberintos, y guarda y carga mapas en JSON.

## 2. Modelo del mapa

| Tipo de celda | Costo de entrar | Notas |
|---|---|---|
| Libre | 1 | Es el valor por defecto. |
| Barro | 5 | Se puede cruzar, pero cuesta más. |
| Agua | 10 | Se puede cruzar, pero cuesta mucho. |
| Muro | no transitable | Nunca se explora. |

- Movimiento configurable: **4 direcciones** o **8 direcciones**. En diagonal, el costo se multiplica por 1,414 (`√2`) y no se permite cortar una esquina entre dos muros.
- El costo de un camino es la suma de los costos de las celdas a las que se entra, sin contar la celda de inicio.

## 3. Núcleo (librería sin dependencias de UI)

Proyecto `PathLab.Core`, con su propio proyecto de tests `PathLab.Core.Tests`.

### 3.1 Estructuras propias

- `MinHeap<T>`: heap binario con arreglo, con un comparador. Debe soportar `Insertar`, `ExtraerMinimo` y `EstaVacio`, todo en O(log n) salvo `EstaVacio`, que es O(1).
- `Grilla`: celdas, tamaño y costos. Método para obtener los vecinos válidos de una celda según el modo de movimiento.
- Opcional: una versión del heap con `DisminuirClave`, para comparar su rendimiento contra insertar duplicados.

### 3.2 Algoritmos

Todos implementan la misma interfaz:

```csharp
public interface IBuscadorRutas
{
    ResultadoBusqueda Buscar(Grilla grilla, Punto inicio, Punto destino);
}

public class ResultadoBusqueda
{
    public bool Encontrado { get; }
    public IReadOnlyList<Punto> Camino { get; }      // vacío si no hay
    public double Costo { get; }
    public IReadOnlyList<Punto> OrdenExploracion { get; }
    public long TiempoMs { get; }
}
```

| Algoritmo | Qué debe cumplir |
|---|---|
| **BFS** | Camino con menos celdas, ignorando los costos. Solo es óptimo en costo cuando todas las celdas valen lo mismo. |
| **Dijkstra** | Camino de costo mínimo, con tu `MinHeap`. |
| **A\*** | Camino de costo mínimo, usando `f = g + h`. Heurística configurable: Manhattan (4 direcciones), euclídea u octil (8 direcciones). |

Condiciones:
- La heurística tiene que ser **admisible** (nunca sobreestima), para que A* siga siendo óptimo.
- Con un mapa sin camino, el resultado debe ser `Encontrado = false` sin lanzar excepciones.
- Si inicio y destino son la misma celda, el costo es 0 y el camino tiene una sola celda.
- Si el inicio o el destino es un muro, devolver `Encontrado = false`.

### 3.3 Complejidad esperada

| Operación | Complejidad |
|---|---|
| `MinHeap.Insertar` y `ExtraerMinimo` | O(log n) |
| Dijkstra y A* (peor caso) | O(V log V), con V la cantidad de celdas |
| BFS | O(V) |

Documentá en el README por qué A* explora menos celdas que Dijkstra en la práctica.

## 4. Interfaz (Blazor)

- Grilla dibujada con CSS grid o canvas, con celdas clickeables.
- Herramientas: pintar muro, barro, agua, borrar, poner inicio y poner destino.
- Controles: algoritmo, heurística, tipo de movimiento, velocidad de la animación y botones de ejecutar, limpiar camino y limpiar todo.
- Colores sugeridos: explorado en celeste, camino en amarillo, inicio en verde, destino en rojo.
- Panel de estadísticas con los valores del punto 1.4.
- Modo comparar: dos grillas iguales, cada una con su algoritmo, y una tabla comparativa abajo.

## 5. Generadores de mapas

- **Aleatorio:** porcentaje de muros configurable (por ejemplo 20 %).
- **Laberinto:** con backtracking recursivo (o con Kruskal sobre la grilla). Tiene que garantizar que hay camino entre dos esquinas.

## 6. Persistencia

Guardar y cargar mapas como JSON con este formato:

```json
{
  "ancho": 5,
  "alto": 4,
  "inicio": [0, 0],
  "destino": [3, 4],
  "celdas": ["S...#", ".#.#.", ".#...", "...#E"]
}
```

## 7. Tests obligatorios

1. **Mapa a mano con resultado conocido** (4 direcciones, todos los costos en 1). Debe dar costo 7:
   ```
   S . . # .
   . # . # .
   . # . . .
   . . . # E
   ```
   El camino óptimo pasa por la única abertura de la columna 3, en la fila 2.
2. Mapa sin camino: `Encontrado = false`.
3. Inicio igual al destino.
4. Con costos distintos, Dijkstra y A* dan el mismo costo, y BFS puede dar un camino distinto o más caro.
5. **Test de propiedades:** sobre 200 mapas aleatorios, A* y Dijkstra devuelven el mismo costo.
6. `MinHeap`: inserta valores en orden aleatorio y comprueba que salen ordenados.
7. Los generadores producen mapas válidos (con camino, en el caso del laberinto).

## 8. Benchmarks

Tabla en el README, midiendo con mapas de 50×50, 100×100 y 200×200 con el mismo porcentaje de muros:

| Mapa | Algoritmo | Celdas exploradas | Tiempo (ms) |
|---|---|---|---|
| 100 × 100 | BFS | | |
| 100 × 100 | Dijkstra | | |
| 100 × 100 | A* | | |

Agregá una conclusión escrita de dos o tres líneas.

## 9. Fases y horas

| Fase | Entregable | Horas |
|---|---|---|
| 1 | `MinHeap`, `Grilla`, BFS y Dijkstra con tests | 8 |
| 2 | A*, heurísticas y test de propiedades | 4 |
| 3 | Interfaz Blazor con edición de mapa y animación | 9 |
| 4 | Modo comparar, generadores y JSON | 5 |
| 5 | Benchmarks, README con capturas o GIF, CI y despliegue | 4 |

## 10. Criterios de entrega

- Repo **público** en GitHub con README (qué hace, cómo correrlo, capturas o GIF, tabla de benchmarks).
- La aplicación **desplegada** con un link que funcione.
- CI con GitHub Actions: compila y corre los tests en cada push.
- Cobertura razonable del núcleo y todos los tests de la sección 7 en verde.

## 11. Extras opcionales

- Búsqueda bidireccional, y comparar su costo contra A*.
- Pesos dinámicos: arrastrar el mouse para cambiar el costo de celdas a mano.
- Cargar un mapa desde una imagen en blanco y negro.
- Algoritmo Greedy Best-First, para mostrar que es más rápido pero no siempre óptimo.
- Pasar del mapa de grilla a un grafo de calles con aristas ponderadas.
