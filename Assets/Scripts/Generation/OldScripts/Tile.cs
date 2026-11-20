using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Tile : MonoBehaviour
{
    [SerializeField] private Tile[] upNeighbours;
    [SerializeField] private Tile[] rightNeighbours;
    [SerializeField] private Tile[] downNeighbours;
    [SerializeField] private Tile[] leftNeighbours;

    [SerializeField] private bool isBorderTile;
    [SerializeField] private List<BorderType> borderTypes;

    public Tile[] UpNeighbours { get => upNeighbours; set => upNeighbours = value; }
    public Tile[] RightNeighbours { get => rightNeighbours; set => rightNeighbours = value; }
    public Tile[] DownNeighbours { get => downNeighbours; set => downNeighbours = value; }
    public Tile[] LeftNeighbours { get => leftNeighbours; set => leftNeighbours = value; }
    public bool IsBorderTile { get => isBorderTile; set => isBorderTile = value; }
    public List<BorderType> BorderTypes { get => borderTypes; set => borderTypes = value; }

    /// <summary>
    /// Initialise les rÃ©fÃ©rences et l'Ã©tat interne avant le dÃ©marrage du script.
    /// </summary>
    private void Awake()
    {

    }
}

    /// <summary>
    /// ReprÃ©sente les diffÃ©rents Ã©tats possibles pour border type.
    /// </summary>
public enum BorderType
{
    None,
    Top,
    Right,
    Bottom,
    Left,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}
