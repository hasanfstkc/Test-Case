using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public GameObject[] _tilePrefab;
    void Start()
    {
        float x=transform.position.x;
        float y= transform.position.y;
        int randomTile = Random.Range(0, _tilePrefab.Length);
        GameObject tile = Instantiate(_tilePrefab[randomTile], new Vector3(x,y+10,0), Quaternion.identity);
        var fruitComp = tile.GetComponent<Fruit>();
        fruitComp._x = x;
        fruitComp._y = y;
        tile.transform.parent = this.transform;
        tile.name = gameObject.name + "Child";
    }
}