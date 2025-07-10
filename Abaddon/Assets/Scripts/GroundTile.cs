using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tilemaps")]
public class GroundTile : RuleTile<GroundTile.Neighbor>
{
    public bool customField;
    public TileBase[] otherAllowedTiles;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int RuleTile = 3;
        public const int Specified = 4;
        public const int Nothing = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            // case Neighbor.Same: return CheckThis(tile);
            // case Neighbor.NotSame: return CheckNotThis(tile);
            case Neighbor.RuleTile:
                return CheckRuleTile(tile);
            case Neighbor.Specified:
                return CheckSpecified(tile);
            case Neighbor.Nothing:
                return CheckNothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    bool CheckRuleTile(TileBase tile)
    {
        return otherAllowedTiles.Contains(tile);
    }

    bool CheckSpecified(TileBase tile)
    {
        return otherAllowedTiles.Contains(tile);
    }

    bool CheckNothing(TileBase tile)
    {
        return tile == null;
    }
}
