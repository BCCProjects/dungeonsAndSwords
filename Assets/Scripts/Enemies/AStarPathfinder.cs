using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// A* implementado por nós, sobre um grid lido dos Tilemaps de colisão (Ground e Platforms).
// Cada célula do Tilemap vira um nó: com tile = bloqueado, sem tile = livre para voar.
// Também bloqueia célula onde o colisor de um tile vizinho invade (o desenho do tile é maior que a célula):
// senão o voador tenta passar rente à quina, bate e fica preso.
// Custo f = g + h, com h = distância de Manhattan; movimento em 4 direções, custo 1 por passo.
public class AStarPathfinder : MonoBehaviour
{
    [SerializeField] private Tilemap[] obstacleTilemaps;
    [SerializeField] private int padding = 2;       // células livres em volta do mapa
    [SerializeField] private int skyPadding = 8;    // espaço extra acima, para o voador manobrar
    [SerializeField] private int maxIterations = 4000;
    [Tooltip("Espaço livre exigido no centro de cada célula (o corpo do voador precisa caber).")]
    [SerializeField] private Vector2 clearance = new Vector2(0.95f, 0.95f);
    [SerializeField] private bool drawGrid;

    private class Node
    {
        public Vector2Int cell;
        public int g;       // custo do início até aqui
        public int h;       // estimativa até o objetivo (Manhattan)
        public int F => g + h;
        public Node parent;
    }

    private static readonly Vector2Int[] Neighbours = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private Grid grid;
    private BoundsInt bounds;
    private bool[,] walkable;
    private List<Vector3> lastPath = new List<Vector3>();

    private readonly Collider2D[] overlap = new Collider2D[1];

    // No Start os colisores dos Tilemaps já existem (no Awake podem ainda não ter sido gerados).
    private void Start() => BuildGrid();

    // Lê os Tilemaps de colisão e monta a matriz de células livres/bloqueadas.
    public void BuildGrid()
    {
        grid = obstacleTilemaps[0].layoutGrid;
        bounds = obstacleTilemaps[0].cellBounds;
        var filter = new ContactFilter2D { useTriggers = false };
        int obstacleMask = 0;
        foreach (Tilemap map in obstacleTilemaps)
        {
            obstacleMask |= 1 << map.gameObject.layer;
            if (map.TryGetComponent(out TilemapCollider2D tileCollider)) tileCollider.ProcessTilemapChanges();
            map.CompressBounds();
            bounds.xMin = Mathf.Min(bounds.xMin, map.cellBounds.xMin);
            bounds.yMin = Mathf.Min(bounds.yMin, map.cellBounds.yMin);
            bounds.xMax = Mathf.Max(bounds.xMax, map.cellBounds.xMax);
            bounds.yMax = Mathf.Max(bounds.yMax, map.cellBounds.yMax);
        }
        bounds.xMin -= padding;
        bounds.yMin -= padding;
        bounds.xMax += padding;
        bounds.yMax += skyPadding;
        filter.SetLayerMask(obstacleMask);
        Physics2D.SyncTransforms();

        walkable = new bool[bounds.size.x, bounds.size.y];
        for (int x = 0; x < bounds.size.x; x++)
            for (int y = 0; y < bounds.size.y; y++)
            {
                var cell = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                walkable[x, y] = true;
                foreach (Tilemap map in obstacleTilemaps)
                    if (map.HasTile(cell)) { walkable[x, y] = false; break; }
                if (walkable[x, y] && Physics2D.OverlapBox(grid.GetCellCenterWorld(cell), clearance, 0f, filter, overlap) > 0)
                    walkable[x, y] = false;
            }
    }

    // Retorna os pontos (centro das células) do caminho até o alvo, sem a célula inicial.
    public List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        var path = new List<Vector3>();
        Vector2Int start = FreeNeighbour(ToCell(startWorld));
        Vector2Int goal = ClosestWalkable(ToCell(targetWorld));
        if (!IsWalkable(start) || !IsWalkable(goal)) return path;

        var open = new List<Node>();                                // lista aberta: a avaliar
        var openLookup = new Dictionary<Vector2Int, Node>();
        var closed = new HashSet<Vector2Int>();                     // lista fechada: já avaliados

        var first = new Node { cell = start, g = 0, h = Manhattan(start, goal) };
        open.Add(first);
        openLookup[start] = first;

        int iterations = 0;
        while (open.Count > 0 && iterations++ < maxIterations)
        {
            // Pega o nó com menor f (desempate pelo menor h).
            Node current = open[0];
            for (int i = 1; i < open.Count; i++)
                if (open[i].F < current.F || (open[i].F == current.F && open[i].h < current.h))
                    current = open[i];

            if (current.cell == goal)
                return BuildPath(current);

            open.Remove(current);
            openLookup.Remove(current.cell);
            closed.Add(current.cell);

            foreach (Vector2Int dir in Neighbours)
            {
                Vector2Int next = current.cell + dir;
                if (!IsWalkable(next) || closed.Contains(next)) continue;

                int g = current.g + 1;
                if (openLookup.TryGetValue(next, out Node known))
                {
                    if (g < known.g) { known.g = g; known.parent = current; }
                }
                else
                {
                    var node = new Node { cell = next, g = g, h = Manhattan(next, goal), parent = current };
                    open.Add(node);
                    openLookup[next] = node;
                }
            }
        }
        return path; // sem caminho
    }

    private List<Vector3> BuildPath(Node end)
    {
        var path = new List<Vector3>();
        for (Node n = end; n.parent != null; n = n.parent)
            path.Add(grid.GetCellCenterWorld(new Vector3Int(n.cell.x, n.cell.y, 0)));
        path.Reverse();
        lastPath = path;
        return path;
    }

    private static int Manhattan(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    private Vector2Int ToCell(Vector3 world)
    {
        Vector3Int c = grid.WorldToCell(world);
        return new Vector2Int(c.x, c.y);
    }

    private bool IsWalkable(Vector2Int cell)
    {
        int x = cell.x - bounds.xMin, y = cell.y - bounds.yMin;
        return x >= 0 && y >= 0 && x < bounds.size.x && y < bounds.size.y && walkable[x, y];
    }

    // Se o alvo estiver dentro de um bloco (ex.: pés no chão), sobe até a primeira célula livre.
    private Vector2Int ClosestWalkable(Vector2Int cell)
    {
        for (int i = 0; i < 4 && !IsWalkable(cell); i++) cell += Vector2Int.up;
        return cell;
    }

    // O voador pode estar encostado numa célula marcada como bloqueada: começa pela vizinha livre.
    private Vector2Int FreeNeighbour(Vector2Int cell)
    {
        if (IsWalkable(cell)) return cell;
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                var next = new Vector2Int(cell.x + dx, cell.y + dy);
                if (IsWalkable(next)) return next;
            }
        return cell;
    }

    private void OnDrawGizmos()
    {
        if (walkable == null || grid == null) return;
        if (drawGrid)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            for (int x = 0; x < bounds.size.x; x++)
                for (int y = 0; y < bounds.size.y; y++)
                    if (!walkable[x, y])
                        Gizmos.DrawCube(grid.GetCellCenterWorld(new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0)), Vector3.one * 0.9f);
        }
        Gizmos.color = Color.cyan;
        for (int i = 0; i < lastPath.Count - 1; i++)
            Gizmos.DrawLine(lastPath[i], lastPath[i + 1]);
    }
}
