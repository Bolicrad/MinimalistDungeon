using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour {
    private Transform _dungeon;
    private Transform _floor;
    private Transform _wall;
    private Transform _player;
    private Transform _end;
    private int _pieceNum;
    public Image triforceFrame;
    private SpriteRenderer _triforceSpriteRenderer;
    public Sprite[] triforce;
    public int randomSeed=1;
    public int 
        roomMaxLength=5, 
        roomMaxWidth=5,
        roomMinLength=3,
        roomMinWidth=3,
        mapMaxLength=100,
        mapMaxWidth=100,
        roomNum=5,
        minCorridorLen=1,
        maxCorridorLen=3,
        step=10000;

    
    private const float FloorLength = 1.0f;
    public GameObject endPoint,piece,player,floor,corridor, wallS, wallW, wallA, wallD;
    public CinemachineVirtualCamera vCam;
    
    void Start () {
	    _dungeon = GameObject.Find ("Dungeon").transform;
	    _floor = GameObject.Find ("Dungeon/Floor").transform;
	    _wall = GameObject.Find ("Dungeon/Wall").transform;
	    GenerateDungeon();
	    StartGame();
    }

    public void StartGame()
    {
	    int startCount = Random.Range(0, _floor.childCount / 3);
	    var playerInstance = Instantiate(player, _floor.GetChild(startCount).position, quaternion.identity);
	    _player = playerInstance.transform;
	    _player.parent = _dungeon;
	    vCam.Follow = _player;

	    int endCount = Random.Range(0, _floor.childCount);
	    var endPointInstance = Instantiate(endPoint, _floor.GetChild(endCount).position, quaternion.identity);
	    _triforceSpriteRenderer = endPointInstance.GetComponent<SpriteRenderer>();
	    _end = endPointInstance.transform;
	    _end.parent = _dungeon;
	    
	    for (var i = 0; i < 3; i++)
	    {
		    int pieceCount = Random.Range(_floor.childCount * i / 3, _floor.childCount * (i + 1) / 3);
		    var pieceInstance = Instantiate(piece, _floor.GetChild(pieceCount).position, quaternion.identity);
		    pieceInstance.transform.parent = _dungeon;
	    }
    }
    public void GenerateDungeon()
    {
	    //Random.InitState(randomSeed);
	    Map.Instance.Init(roomMaxLength, roomMaxWidth, roomMinLength, roomMinWidth, mapMaxLength, mapMaxWidth, roomNum,minCorridorLen,maxCorridorLen);//初始化参数
	    Map.Instance.MakeDungeon(step);
	    Create(Map.Instance.GetMap());
    }
    private void LocalInstantiate(GameObject prefab, Vector3 pos)
    {
	    GameObject g = Instantiate(prefab, pos, Quaternion.identity);
	    g.transform.SetParent(prefab == floor ? _floor : _wall);
    }

    private void EnableEndpoint()
    {
	    Debug.Log("Enabled End Point");
	    _end.GetComponent<Collider2D>().enabled = true;
    }

    public void AddPiece()
    {
	    if (_pieceNum < 3)
	    {
		    _pieceNum++;
		    triforceFrame.sprite = triforce[_pieceNum];
		    _triforceSpriteRenderer.sprite = triforce[_pieceNum];
		    if (_pieceNum == 3)
		    {
			    EnableEndpoint();
		    }
	    }
    }


    public void Create(CustomTile[,]map){
		for (var i = 0; i < map.GetLength (0); i++)
			for (var j = 0; j < map.GetLength (1); j++) {
				switch (map [i, j]) { 
					case (CustomTile.CorridorAD):
					case (CustomTile.CorridorWS):
						LocalInstantiate(corridor,new Vector3(i*FloorLength,j*FloorLength));
						break;
					case (CustomTile.Floor): 
						LocalInstantiate (floor,new Vector3(i*FloorLength,j*FloorLength)); 
						break; 
					case (CustomTile.WallS): 
						LocalInstantiate (wallS,new Vector3(i*FloorLength,j*FloorLength)); 
						break;
					case (CustomTile.WallW): 
						LocalInstantiate (wallW,new Vector3(i*FloorLength,j*FloorLength)); 
						break; 
					case (CustomTile.WallA): 
						LocalInstantiate (wallA,new Vector3(i*FloorLength,j*FloorLength)); 
						break; 
					case (CustomTile.WallD): 
						LocalInstantiate (wallD,new Vector3(i*FloorLength,j*FloorLength)); 
						break; 
					default: 
						break; 
				}
			}
	}
}