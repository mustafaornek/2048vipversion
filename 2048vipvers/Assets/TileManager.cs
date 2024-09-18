using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // LayoutRebuilder için gerekli
using System.Linq;


public class TileManager : MonoBehaviour
{

    public static int GridSize = 4;
    private readonly Transform[,] _tilePositions = new Transform[GridSize, GridSize];

    private readonly Tile[,] _tiles = new Tile[GridSize, GridSize];
    [SerializeField] private Tile tilePrefab;

    private bool _isAnimating;

    [SerializeField] private TileSettings tileSettings;


    // Start is called before the first frame update
    void Start()
    {
        GetTilePositions();
        TrySpawnTile();
        TrySpawnTile();

        
        UpdateTilePositions(true);
    }

    // Update is called once per frame
    void Update()
    {
        var xInput = Input.GetAxisRaw("Horizontal");
        var yInput = Input.GetAxisRaw("Vertical");
        
        TryMove(Mathf.RoundToInt(xInput), Mathf.RoundToInt(yInput));
    }
    
    private void GetTilePositions(){

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        int x = 0;
        int y = 0;
        foreach ( Transform transform in this.transform)
        {
            _tilePositions[x,y] = transform;
            x++;
            if(x >= GridSize)
            {
                x = 0;
                y++;

            }
        }
    }

private bool TrySpawnTile(){

    List<Vector2Int> availableSpots = new List<Vector2Int>();

    for (int x = 0; x < GridSize; x++)
    for (int y = 0; y < GridSize; y++)
    {
        if(_tiles[x,y] == null)
            availableSpots.Add(new Vector2Int(x,y));
    }

    if(!availableSpots.Any())
        return false;
        
    int randomIndex = Random.Range(0, availableSpots.Count);
    Vector2Int spot = availableSpots[randomIndex];

    var tile = Instantiate(tilePrefab, transform.parent);
    tile.SetValue(GetRandomValue());
    _tiles[spot.x, spot.y] = tile;

    return true;

   
}

 private int GetRandomValue()
    {
        var rand = Random.Range(0f,1f);
        if (rand <= .8f)
            return 2;
        else 
            return 4;
    }

private void UpdateTilePositions(bool instant)
{
    if(!instant)
    {
        _isAnimating = true;
        StartCoroutine(WaitForTileAnimation());
    }

    

    for (int x=0; x< GridSize; x++)
    for (int y=0; y< GridSize; y++)
        if(_tiles[x,y] != null)
            _tiles[x,y].SetPosition(_tilePositions[x,y].position, instant);
}

private IEnumerator WaitForTileAnimation(){
        yield return new WaitForSeconds(tileSettings.AnimationTime);
        _isAnimating = false;
    }

    private bool _tilesUpdated;
private void TryMove(int x, int y)
{
    if(x == 0 && y ==0 )
        return;

    if(Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
    {
        Debug.LogWarning($"Invalid move {x} , {y}");
        return;
    }

    _tilesUpdated = false;
    if (x == 0)
    {
        if(y>0)
            TryMoveUp();
        else
            TryMoveDown();
    }
    else
    {
        if(x < 0)
            TryMoveLeft();
            else
            TryMoveRight();
    }
    if(_tilesUpdated)
    UpdateTilePositions(false);
}

private void TryMoveRight()
{
   for (int y = 0; y < GridSize; y++)
   for (int x = GridSize-1; x>= 0; x--)
   {
        if (_tiles[x,y] == null) continue;

        for (int x2 = GridSize-1; x2 > x; x2--)
        {
            if (_tiles[x2, y] != null) continue;

            _tilesUpdated = true; 
            _tiles[x2,y] = _tiles[x,y];
            _tiles[x,y] = null;
            break;
        }
   }
}
private void TryMoveLeft()
{
        for (int y = 0; y < GridSize; y++)
            for (int x = 0; x < GridSize; x++)
            {
                if (_tiles[x, y] == null) continue;
                for (int x2 = 0; x2 < x; x2++)
                {
                    if (_tiles[x2, y] != null) continue;

                    _tiles[x2, y] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
  
}
private void TryMoveUp()
{
    for (int x = 0; x < GridSize; x++)
        for (int y = 0; y < GridSize; y++)
        {
            if (_tiles[x, y] == null) continue;
            for (int y2 = 0; y2 < y; y2++)
            {
                if (_tiles[x, y2] != null) continue;

                _tiles[x, y2] = _tiles[x, y];
                _tiles[x, y] = null;
                break;
            }
        }

}    
private void TryMoveDown()
{
    for (int x = 0; x  < GridSize; x++)
        for (int y = GridSize - 1; y >= 0; y--)
        {
            if (_tiles[x, y] == null) continue;
            for (int y2 = GridSize - 1; y2 > y; y2--)
            {
                if (_tiles[x, y2] !=  null) continue;

                _tiles[x, y2] =_tiles[x, y];
                _tiles[x, y] = null;
                break;

            }
        }
   
}
}
