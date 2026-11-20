using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Génère une carte procédurale à l'aide de l'algorithme Wave Function Collapse.
/// Le système instancie une grille de cellules, réduit progressivement les possibilités
/// de chaque cellule selon les contraintes de voisinage, puis génère les bordures
/// et déclenche le spawn des entités une fois la carte terminée.
/// </summary>
    
/// <summary>
/// GÃ¨re le comportement de wave function collapse dans la scÃ¨ne.
/// </summary>
public class WaveFunctionCollapse : MonoBehaviour
{
    #region Singleton & Properties

    /// <summary>
    /// Instance globale du générateur.
    /// </summary>
    public static WaveFunctionCollapse Instance { get; private set; }

    /// <summary>
    /// Taille d'une tuile.
    /// </summary>
    public int TileSize { get => tileSize; set => tileSize = value; }

    /// <summary>
    /// Nombre de cellules sur un axe de la grille.
    /// </summary>
    public int Dimensions { get => dimensions; set => dimensions = value; }

    /// <summary>
    /// Taille totale utilisée entre deux cellules, en prenant en compte le multiplicateur.
    /// </summary>
    public int TotalSizeTile { get => totalSizeTile; set => totalSizeTile = value; }

    #endregion

    #region Inspector - Grid

    [Header("Grid")]
    [SerializeField] private int dimensions;

    [SerializeField] private int tileSize = 18;

    [SerializeField] private int sizeMultiplier = 3;

    /// <summary>
    /// Taille finale utilisée pour espacer les cellules dans le monde.
    /// </summary>
    private int totalSizeTile;

    #endregion

    #region Inspector - Tiles

    [Header("Tiles")]
    [SerializeField] private Tile[] tileObjects;
    [SerializeField] private List<Cell> gridComponents;
    [SerializeField] private Cell cellObj;

    [SerializeField]
    private Tile borderTileObj;

    #endregion

    #region Inspector - Containers

    [Header("Containers")]
    [SerializeField] private Transform cellContainer;
    [SerializeField] private Transform tilesContainer;
    [SerializeField] private Transform borderContainer;
    [SerializeField] private Transform limitMapContainer;

    #endregion

    #region Private Fields

    /// <summary>
    /// Nombre d'itérations de génération effectuées.
    /// </summary>
    private int iteration;

    /// <summary>
    /// Cache permettant de retrouver rapidement l'index d'une tuile à partir de sa référence.
    /// </summary>
    private Dictionary<Tile, int> tileIndexByReference;

    // Buffers réutilisés pour limiter les allocations mémoire pendant la génération.
    private readonly List<Cell> lowestEntropyCells = new List<Cell>();
    private readonly List<Tile> workingOptions = new List<Tile>();
    private readonly HashSet<Tile> validTilesBuffer = new HashSet<Tile>();

    /// <summary>
    /// Position de spawn du joueur.
    /// </summary>
    private Vector3 playerSpawnPosition;


    #endregion

    #region Events & References

    /// <summary>
    /// Événement invoqué lorsque la génération de la carte est terminée.
    /// </summary>
    public UnityEvent OnMapGenerated;

    /// <summary>
    /// Point représentant le centre de la carte.
    /// </summary>
    public Transform pointMiddleMap;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initialise le singleton, prépare les tailles, les références,
    /// puis construit la grille de départ.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        TotalSizeTile = TileSize * sizeMultiplier;
        //pointMiddleMap.position = new Vector3((Dimensions * TotalSizeTile) / 2, 0, (Dimensions * TotalSizeTile) / 2);
        playerSpawnPosition = new Vector3(TileSize / 2, 0, TileSize / 2);

        gridComponents = new List<Cell>(Dimensions * Dimensions);

        BuildTileIndexCache();
        InitializeGrid();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Construit un dictionnaire reliant chaque tuile à son index dans <see cref="tileObjects"/>.
    /// Permet d'accélérer la recherche lors de l'application des contraintes.
    /// </summary>
    private void BuildTileIndexCache()
    {
        tileIndexByReference = new Dictionary<Tile, int>(tileObjects.Length);

        for (int i = 0; i < tileObjects.Length; i++)
        {
            Tile tile = tileObjects[i];
            if (tile != null && !tileIndexByReference.ContainsKey(tile))
            {
                tileIndexByReference.Add(tile, i);
            }
        }
    }

    /// <summary>
    /// Instancie toutes les cellules de la grille, identifie les bords
    /// et initialise les options possibles pour chaque cellule.
    /// </summary>
    private void InitializeGrid()
    {
        for (int y = 0; y < Dimensions; y++)
        {
            for (int x = 0; x < Dimensions; x++)
            {
                Cell newCell = Instantiate(
                    cellObj,
                    new Vector3(x * TotalSizeTile, 0, y * TotalSizeTile),
                    Quaternion.identity,
                    cellContainer
                );

                bool isBorder = x == 0 || y == 0 || x == Dimensions - 1 || y == Dimensions - 1;

                newCell.name = isBorder ? $"Border {x} {y}" : $"Cell {x} {y}";
                newCell.isBorder = isBorder;
                newCell.x = x;
                newCell.y = y;

                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }

        StartCoroutine(GenerateRoutine());
    }

    #endregion

    #region Generation

    /// <summary>
    /// Coroutine principale de génération.
    /// Continue tant qu'il reste des cellules non résolues,
    /// puis génère les bordures et déclenche l'événement de fin.
    /// </summary>
    private IEnumerator GenerateRoutine()
    {
        while (true)
        {
            List<Cell> candidates = GetLowestEntropyCells();

            if (candidates.Count == 0)
            {
                break;
            }

            CollapseCell(candidates);
            UpdateGeneration();

            // Attend une frame pour étaler la génération dans le temps.
            yield return null;
        }

        InitializeGridBorder();
        OnMapGenerated.Invoke();
    }

    /// <summary>
    /// Fait apparaître le joueur, les ennemis et les objets après génération de la carte.
    /// </summary>
    public void SpawnEntities()
    {

    }

    /// <summary>
    /// Récupère toutes les cellules non résolues ayant le plus faible nombre d'options disponibles.
    /// Ces cellules ont l'entropie la plus faible et sont donc prioritaires.
    /// </summary>
    /// <returns>Liste des cellules candidates à l'effondrement.</returns>
    private List<Cell> GetLowestEntropyCells()
    {
        lowestEntropyCells.Clear();

        int minOptions = int.MaxValue;

        for (int i = 0; i < gridComponents.Count; i++)
        {
            Cell cell = gridComponents[i];

            if (cell == null || cell.collapsed || cell.isBorder)
                continue;

            int optionCount = cell.tileOptions != null ? cell.tileOptions.Length : 0;

            if (optionCount <= 0)
            {
                Debug.LogError($"WFC contradiction: cell {cell.name} has no valid tile options.");
                continue;
            }

            if (optionCount < minOptions)
            {
                minOptions = optionCount;
                lowestEntropyCells.Clear();
                lowestEntropyCells.Add(cell);
            }
            else if (optionCount == minOptions)
            {
                lowestEntropyCells.Add(cell);
            }
        }

        return lowestEntropyCells;
    }

    /// <summary>
    /// Choisit aléatoirement une cellule parmi les candidates,
    /// puis sélectionne aléatoirement une tuile parmi ses options.
    /// </summary>
    /// <param name="tempGrid">Liste des cellules candidates.</param>
    private void CollapseCell(List<Cell> tempGrid)
    {
        if (tempGrid == null || tempGrid.Count == 0)
            return;

        int randIndex = Random.Range(0, tempGrid.Count);
        Cell cellToCollapse = tempGrid[randIndex];

        if (cellToCollapse == null || cellToCollapse.tileOptions == null || cellToCollapse.tileOptions.Length == 0)
            return;

        cellToCollapse.collapsed = true;

        Tile selectedTile = cellToCollapse.tileOptions[Random.Range(0, cellToCollapse.tileOptions.Length)];
        cellToCollapse.tileOptions = new Tile[] { selectedTile };

        if (selectedTile != null)
        {
            Tile tile = Instantiate(
                selectedTile,
                cellToCollapse.transform.position,
                selectedTile.transform.rotation,
                tilesContainer
            );


        }
    }

    /// <summary>
    /// Recalcule les options possibles de chaque cellule non résolue
    /// en fonction des tuiles voisines.
    /// </summary>
    private void UpdateGeneration()
    {
        for (int y = 0; y < Dimensions; y++)
        {
            for (int x = 0; x < Dimensions; x++)
            {
                int index = x + y * Dimensions;
                Cell currentCell = gridComponents[index];

                if (currentCell == null || currentCell.collapsed || currentCell.isBorder)
                    continue;

                // Réinitialise la liste de travail avec toutes les tuiles valides.
                workingOptions.Clear();
                for (int i = 0; i < tileObjects.Length; i++)
                {
                    if (tileObjects[i] != null)
                        workingOptions.Add(tileObjects[i]);
                }

                // Applique les contraintes des voisins.
                if (y > 0)
                {
                    Cell up = gridComponents[x + (y - 1) * Dimensions];
                    ApplyConstraintFromNeighbor(workingOptions, up, NeighborSide.Up);
                }

                if (x < Dimensions - 1)
                {
                    Cell right = gridComponents[x + 1 + y * Dimensions];
                    ApplyConstraintFromNeighbor(workingOptions, right, NeighborSide.Left);
                }

                if (y < Dimensions - 1)
                {
                    Cell down = gridComponents[x + (y + 1) * Dimensions];
                    ApplyConstraintFromNeighbor(workingOptions, down, NeighborSide.Down);
                }

                if (x > 0)
                {
                    Cell left = gridComponents[x - 1 + y * Dimensions];
                    ApplyConstraintFromNeighbor(workingOptions, left, NeighborSide.Right);
                }

                currentCell.RecreateCell(workingOptions.ToArray());
            }
        }

        iteration++;
    }

    /// <summary>
    /// Réduit les options disponibles d'une cellule en utilisant les contraintes
    /// provenant d'un voisin donné.
    /// </summary>
    /// <param name="options">Liste des options actuellement possibles.</param>
    /// <param name="neighbor">Cellule voisine à analyser.</param>
    /// <param name="side">Côté du voisin à prendre en compte.</param>
    private void ApplyConstraintFromNeighbor(List<Tile> options, Cell neighbor, NeighborSide side)
    {
        if (neighbor == null || neighbor.tileOptions == null || neighbor.tileOptions.Length == 0)
        {
            options.Clear();
            return;
        }

        validTilesBuffer.Clear();

        for (int i = 0; i < neighbor.tileOptions.Length; i++)
        {
            Tile possibleOption = neighbor.tileOptions[i];

            if (possibleOption == null)
                continue;

            if (!tileIndexByReference.TryGetValue(possibleOption, out int tileIndex))
                continue;

            Tile sourceTile = tileObjects[tileIndex];
            Tile[] validNeighbours = GetNeighboursForSide(sourceTile, side);

            if (validNeighbours == null)
                continue;

            for (int j = 0; j < validNeighbours.Length; j++)
            {
                Tile validTile = validNeighbours[j];
                if (validTile != null)
                    validTilesBuffer.Add(validTile);
            }
        }

        for (int i = options.Count - 1; i >= 0; i--)
        {
            if (!validTilesBuffer.Contains(options[i]))
            {
                options.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Retourne la liste des voisins autorisés d'une tuile selon un côté donné.
    /// </summary>
    /// <param name="tile">Tuile source.</param>
    /// <param name="side">Côté à consulter.</param>
    /// <returns>Tableau des tuiles compatibles sur ce côté.</returns>
    private Tile[] GetNeighboursForSide(Tile tile, NeighborSide side)
    {
        if (tile == null)
            return null;

        switch (side)
        {
            case NeighborSide.Up:
                return tile.UpNeighbours;
            case NeighborSide.Right:
                return tile.RightNeighbours;
            case NeighborSide.Down:
                return tile.DownNeighbours;
            case NeighborSide.Left:
                return tile.LeftNeighbours;
            default:
                return null;
        }
    }

    #endregion

    #region Border Generation

    /// <summary>
    /// Lance la génération des bordures une fois la génération principale terminée.
    /// </summary>
    private void InitializeGridBorder()
    {
        Debug.Log("Final iteration reached, updating border generation...");
        UpdateBorderGeneration();
    }

    /// <summary>
    /// Attribue les tuiles de bordure aux cellules situées sur le contour de la carte.
    /// </summary>
    private void UpdateBorderGeneration()
    {
        foreach (Cell cell in gridComponents)
        {
            if (cell == null || !cell.isBorder)
                continue;

            Tile borderTile = GetBorderTile(cell.x, cell.y);
            if (borderTile != null)
            {
                cell.AssignBorderTile(borderTile, borderContainer);
            }
        }

        InitializeMapLimit();
    }

    /// <summary>
    /// Génère la limite extérieure de la carte avec les tuiles de bordure.
    /// </summary>
    private void InitializeMapLimit()
    {
        Tile starTile = Instantiate(
            borderTileObj,
            new Vector3(-TotalSizeTile, 0, -TotalSizeTile),
            borderTileObj.transform.rotation,
            limitMapContainer
        );

        starTile.gameObject.name = $"LimitTile x == -1 && y == -1";

        for (int x = 0; x <= Dimensions; x++)
        {
            for (int y = 0; y <= Dimensions; y++)
            {
                if (x == 0)
                {
                    Tile tile = Instantiate(
                        borderTileObj,
                        new Vector3(-TotalSizeTile, 0, y * TotalSizeTile),
                        borderTileObj.transform.rotation,
                        limitMapContainer
                    );

                    if (y == 0)
                    {
                        tile.gameObject.name = $"LimitTile x == 0 && y == 0";
                        tile.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else if (y == Dimensions)
                    {
                        tile.gameObject.name = $"LimitTile x == 0 && y dimensions";
                        tile.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else
                    {
                        tile.gameObject.name = $"LimitTile x == 0";
                        tile.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                }

                if (y == 0)
                {
                    Tile tile = Instantiate(
                        borderTileObj,
                        new Vector3(x * TotalSizeTile, 0, -TotalSizeTile),
                        borderTileObj.transform.rotation,
                        limitMapContainer
                    );

                    tile.gameObject.name = $"LimitTile y == 0";
                    tile.transform.rotation = Quaternion.Euler(0, 180, 0);
                }

                if (x == Dimensions)
                {
                    Tile tile = Instantiate(
                        borderTileObj,
                        new Vector3(x * TotalSizeTile, 0, y * TotalSizeTile),
                        borderTileObj.transform.rotation,
                        limitMapContainer
                    );

                    if (y == 0)
                    {
                        tile.gameObject.name = $"LimitTile x == dimensions && y == 0";
                        tile.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (y == Dimensions)
                    {
                        tile.gameObject.name = $"LimitTile x == dimensions && y dimensions";
                        tile.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else
                    {
                        tile.gameObject.name = $"LimitTile x == dimensions";
                        tile.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                }

                if (y == Dimensions)
                {
                    Tile tile = Instantiate(
                        borderTileObj,
                        new Vector3(x * TotalSizeTile, 0, y * TotalSizeTile),
                        borderTileObj.transform.rotation,
                        limitMapContainer
                    );

                    tile.gameObject.name = $"LimitTile y == dimensions";
                }
            }
        }
    }

    /// <summary>
    /// Retourne la tuile de bordure adaptée à la position donnée.
    /// </summary>
    /// <param name="x">Position X dans la grille.</param>
    /// <param name="y">Position Y dans la grille.</param>
    /// <returns>Tuile de bordure correspondante, ou null si aucune n'est trouvée.</returns>
    private Tile GetBorderTile(int x, int y)
    {
        // Coins
        if (x == 0 && y == 0)
            return FindFirstBorderTile(BorderType.BottomLeft);

        if (x == Dimensions - 1 && y == 0)
            return FindFirstBorderTile(BorderType.BottomRight);

        if (x == 0 && y == Dimensions - 1)
            return FindFirstBorderTile(BorderType.TopLeft);

        if (x == Dimensions - 1 && y == Dimensions - 1)
            return FindFirstBorderTile(BorderType.TopRight);

        // Haut
        if (y == 0)
        {
            if (TryGetCellTile(x, y + 1, out Tile tileBelow))
            {
                return FindBorderTileMatching(BorderType.Top, tile =>
                    ContainsTile(tile.UpNeighbours, tileBelow));
            }

            return FindFirstBorderTile(BorderType.Top);
        }

        // Bas
        if (y == Dimensions - 1)
        {
            if (TryGetCellTile(x, y - 1, out Tile tileAbove))
            {
                return FindBorderTileMatching(BorderType.Bottom, tile =>
                    ContainsTile(tile.DownNeighbours, tileAbove));
            }

            return FindFirstBorderTile(BorderType.Bottom);
        }

        // Gauche
        if (x == 0)
        {
            if (TryGetCellTile(x + 1, y, out Tile tileRight))
            {
                return FindBorderTileMatching(BorderType.Left, tile =>
                    ContainsTile(tile.RightNeighbours, tileRight));
            }

            return FindFirstBorderTile(BorderType.Left);
        }

        // Droite
        if (x == Dimensions - 1)
        {
            if (TryGetCellTile(x - 1, y, out Tile tileLeft))
            {
                return FindBorderTileMatching(BorderType.Right, tile =>
                    ContainsTile(tile.LeftNeighbours, tileLeft));
            }

            return FindFirstBorderTile(BorderType.Right);
        }

        return null;
    }

    /// <summary>
    /// Tente de récupérer la tuile finale d'une cellule donnée.
    /// </summary>
    /// <param name="x">Position X de la cellule.</param>
    /// <param name="y">Position Y de la cellule.</param>
    /// <param name="tile">Tuile trouvée.</param>
    /// <returns>True si une tuile valide a été trouvée.</returns>
    private bool TryGetCellTile(int x, int y, out Tile tile)
    {
        tile = null;

        if (x < 0 || y < 0 || x >= Dimensions || y >= Dimensions)
            return false;

        int index = x + y * Dimensions;
        Cell cell = gridComponents[index];

        if (cell == null || !cell.collapsed || cell.tileOptions == null || cell.tileOptions.Length == 0)
            return false;

        tile = cell.tileOptions[0];
        return tile != null;
    }

    /// <summary>
    /// Retourne la première tuile de bordure correspondant au type demandé.
    /// </summary>
    /// <param name="borderType">Type de bordure recherché.</param>
    /// <returns>Tuile trouvée ou null.</returns>
    private Tile FindFirstBorderTile(BorderType borderType)
    {
        for (int i = 0; i < tileObjects.Length; i++)
        {
            Tile tile = tileObjects[i];
            if (tile != null && tile.IsBorderTile && HasBorderType(tile, borderType))
            {
                return tile;
            }
        }

        return null;
    }

    /// <summary>
    /// Recherche une tuile de bordure d'un type donné qui respecte une condition précise.
    /// </summary>
    /// <param name="borderType">Type de bordure recherché.</param>
    /// <param name="predicate">Condition supplémentaire à respecter.</param>
    /// <returns>Tuile trouvée ou fallback sur la première tuile du type.</returns>
    private Tile FindBorderTileMatching(BorderType borderType, System.Func<Tile, bool> predicate)
    {
        for (int i = 0; i < tileObjects.Length; i++)
        {
            Tile tile = tileObjects[i];

            if (tile == null || !tile.IsBorderTile || !HasBorderType(tile, borderType))
                continue;

            if (predicate(tile))
                return tile;
        }

        return FindFirstBorderTile(borderType);
    }

    /// <summary>
    /// Vérifie si une tuile possède un type de bordure spécifique.
    /// </summary>
    /// <param name="tile">Tuile à analyser.</param>
    /// <param name="borderType">Type recherché.</param>
    /// <returns>True si le type est présent.</returns>
    private bool HasBorderType(Tile tile, BorderType borderType)
    {
        if (tile == null || tile.BorderTypes == null)
            return false;

        foreach (BorderType type in tile.BorderTypes)
        {
            if (type == borderType)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Vérifie si un tableau de tuiles contient une tuile cible.
    /// </summary>
    /// <param name="tiles">Tableau de tuiles à parcourir.</param>
    /// <param name="target">Tuile recherchée.</param>
    /// <returns>True si la tuile est trouvée.</returns>
    private bool ContainsTile(Tile[] tiles, Tile target)
    {
        if (tiles == null || target == null)
            return false;

        foreach (Tile tile in tiles)
        {
            if (tile == target)
                return true;
        }

        return false;
    }

    #endregion

    #region Enums

    /// <summary>
    /// Représente les différents côtés d'une cellule ou d'une tuile.
    /// </summary>
    private enum NeighborSide
    {
        Up,
        Right,
        Down,
        Left
    }

    #endregion
}
