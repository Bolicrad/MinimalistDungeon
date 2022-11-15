using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour {
    private Transform _dungeon;
    private Transform _floor;
    private Transform _wall;
    private Transform _spawn;
    private Transform _end;
    private int _pieceNum;
    public Image triforceFrame;
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
	    _spawn = _floor.GetChild(startCount);
	    var playerInstance = Instantiate(player, _spawn.position, quaternion.identity);
	    playerInstance.transform.parent = _dungeon;
	    vCam.Follow = playerInstance.transform;

	    for (var i = 0; i < 3; i++)
	    {
		    int pieceCount = Random.Range(_floor.childCount * i / 3, _floor.childCount * (i + 1) / 3);
		    var pieceInstance = Instantiate(piece, _floor.GetChild(pieceCount).position, quaternion.identity);
		    pieceInstance.transform.parent = _dungeon;
	    }
    }
    public void GenerateDungeon()
    {
	    //UnityEngine.Random.InitState(randomSeed);
	    Map.Instance.Init(roomMaxLength, roomMaxWidth, roomMinLength, roomMinWidth, mapMaxLength, mapMaxWidth, roomNum,minCorridorLen,maxCorridorLen);//初始化参数
	    Map.Instance.MakeDungeon(step);
	    Create(Map.Instance.GetMap());
    }
    private void LocalInstantiate(GameObject prefab, Vector3 pos)
    {
	    GameObject g = Instantiate(prefab, pos, Quaternion.identity);
	    g.transform.SetParent(prefab == floor ? _floor : _wall);
    }

    private void GenerateEndpoint()
    {
	    Debug.Log("Generated End Point");
	    int endCount = Random.Range(0, _floor.childCount * 2 / 3);
	    _end = _floor.GetChild(endCount);
	    var endPointInstance = Instantiate(endPoint, _end.position, quaternion.identity);
	    endPointInstance.transform.parent = _dungeon;
    }

    public void AddPiece()
    {
	    if (_pieceNum < 3)
	    {
		    _pieceNum++;
		    triforceFrame.sprite = triforce[_pieceNum];
		    if (_pieceNum == 3)
		    {
			    GenerateEndpoint();
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