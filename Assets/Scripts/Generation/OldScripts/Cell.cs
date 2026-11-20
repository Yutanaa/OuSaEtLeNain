using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// GÃ¨re le comportement de cell dans la scÃ¨ne.
    /// </summary>
public class Cell : MonoBehaviour
{
    public bool collapsed;
    public Tile[] tileOptions;
    public bool isBorder;
    public int x;
    public int y;
    /// <summary>
    /// CrÃ©e cell.
    /// </summary>
    /// <param name="collapseState">Valeur utilisÃ©e par la mÃ©thode.</param>
    /// <param name="tiles">Tuile ou collection de tuiles Ã  traiter.</param>
    public void CreateCell(bool collapseState, Tile[] tiles)
    {
        collapsed = collapseState;
        tileOptions = tiles;
    }

    /// <summary>
    /// Reconstruit cell.
    /// </summary>
    /// <param name="tiles">Tuile ou collection de tuiles Ã  traiter.</param>
    public void RecreateCell(Tile[] tiles)
    {      
        // Ne pas recréer les cellules de bordure
        if (!isBorder)
        {
            tileOptions = tiles;
        }
    }
    /// <summary>
    /// Assigne border tile.
    /// </summary>
    /// <param name="borderTile">Valeur utilisÃ©e par la mÃ©thode.</param>
    /// <param name="container">Valeur utilisÃ©e par la mÃ©thode.</param>
    public void AssignBorderTile(Tile borderTile, Transform container)
    {
        if (isBorder && borderTile != null && borderTile.IsBorderTile)
        {
            tileOptions = new Tile[] { borderTile };
            collapsed = true;
            Instantiate(borderTile, transform.position, borderTile.transform.rotation, container);
        }
    }
}
