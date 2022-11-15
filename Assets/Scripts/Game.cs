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

    private void StartGame()
    {
	    foreach (var room in Map.Instance.Rooms)
	    {
		    Vector3 pos = new Vector3(room.GetCenter().x * FloorLength, room.GetCenter().y * FloorLength);
		    switch (room.Type)
		    {
			    case RoomType.Spawn:
			    {
				    var playerInstance = Instantiate(player, pos, quaternion.identity);
				    _player = playerInstance.transform;
				    _player.parent = _dungeon;
				    vCam.Follow = _player;
				    break;
			    }
			    case RoomType.EndPoint:
			    {
				    var endPointInstance = Instantiate(endPoint, pos, quaternion.identity);
				    _end = endPointInstance.transform;
				    _end.parent = _dungeon;
				    _triforceSpriteRenderer = endPointInstance.GetComponent<SpriteRenderer>();
				    break;
			    }
			    case RoomType.Piece:
			    {
				    var pieceInstance = Instantiate(piece, pos, quaternion.identity);
				    pieceInstance.transform.parent = _dungeon;
				    break;
			    }
			    default:
			    {
				    //Generate Nothing
				    break;
			    }
		    }
	    }

    }

    private void GenerateDungeon()
    {
	    //Random.InitState(randomSeed);
	    Map.Instance.Init(roomMaxLength, roomMaxWidth, roomMinLength, roomMinWidth, mapMaxLength, mapMaxWidth, roomNum,minCorridorLen,maxCorridorLen);//初始化参数
	    Map.Instance.MakeDungeon(step);
	    Create(Map.Instance.TileMap);
	    
	    //Init Rooms
	    int length = Map.Instance.Rooms.Count;
	    
	    //Set Spawn Room
	    var spawnPos = Random.Range(0, length);
	    Map.Instance.Rooms[spawnPos].Type = RoomType.Spawn;
	    
	    //Set End Room
	    var endPos = Random.Range(0, length);
	    while (endPos == spawnPos)
	    {
		    endPos = Random.Range(0, length);
	    }
	    Map.Instance.Rooms[endPos].Type = RoomType.EndPoint;
	    
	    //Set Piece Rooms
	    for (var i = 0; i < 3; i++)
	    {
		    int piecePos = Random.Range(0, length);
		    while (Map.Instance.Rooms[piecePos].Type!=RoomType.None)
		    {
			    piecePos = Random.Range(0, length);
		    }
		    Map.Instance.Rooms[piecePos].Type = RoomType.Piece;
	    }

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

    private void Create(CustomTile[,]map){
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