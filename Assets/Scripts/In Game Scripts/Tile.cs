using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    public Vector3 worldPosition;
    public bool isOutside;
    public int gridX;
    public int gridY;

    public Tile(Vector2 _worldPos, bool _isOutside, int _gridX, int _gridY) {
        worldPosition = new Vector3(_worldPos.x, _worldPos.y, 0);
        isOutside = _isOutside;
        gridX = _gridX;
        gridY = _gridY;
    }
}
