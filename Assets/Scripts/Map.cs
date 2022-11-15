using System.Collections.Generic;
using UnityEngine;

public enum CustomTile
{
	Default,
	Floor,
	WallW,
	WallS,
	WallA,
	WallD,
	CorridorAD,
	CorridorWS
}

public enum RoomType
{
	None,
	Spawn,
	EndPoint,
	Piece
}

public class Room
{
	public RoomType Type;
	private readonly Vector2Int _start;
	private readonly Vector2Int _end;

	public Room(int xStart, int yStart, int xEnd, int yEnd)
	{
		Type = RoomType.None;
		_start = new Vector2Int(xStart, yStart);
		_end = new Vector2Int(xEnd, yEnd);
	}

	public Vector2Int GetCenter()
	{
		return (_start + _end) / 2;
	}
}

public class Map
{
	public CustomTile[,] TileMap { get; private set; }
	public List<Room> Rooms { get; private set; }

	private int 
		_roomMaxLength, 
		_roomMaxWidth, 
		_roomMinLength, 
		_roomMinWidth, 
		_mapMaxLength, 
		_mapMaxWidth, 
		_roomNum,
		_minCorridorLen,
		_maxCorridorLen;
	private bool _first;
	private static Map _instance;

	private Map () { }
	public static Map Instance {
		get { return _instance ??= new Map(); }
	}
	
	public void Init (int roomMaxLength,int roomMaxWidth,int roomMinLength,int roomMinWidth,int mapMaxLength,int mapMaxWidth,int roomNum,int minCorridorLen,int maxCorridorLen)
	{
		TileMap = new CustomTile[mapMaxWidth, mapMaxLength];
		Rooms = new List<Room>();
		_first = true;
		_roomMaxLength = roomMaxLength; 
		_roomMaxWidth = roomMaxWidth; 
		_roomMinLength = roomMinLength;
		_roomMinWidth = roomMinWidth;
		_mapMaxLength = mapMaxLength;
		_mapMaxWidth = mapMaxWidth;
		_roomNum = roomNum;
		_minCorridorLen = minCorridorLen;
		_maxCorridorLen = maxCorridorLen;
	}
	
	private bool IsInBounds_x (int x)
	{
		return (x >= 0) && (x <= _mapMaxWidth - 1);
	}

	private bool IsInBounds_y (int y)
	{
		return (y >= 0) && (y <= _mapMaxLength - 1);
	}
	
	private void SetCells (int xStart, int yStart, int xEnd, int yEnd, CustomTile cellType)
	{
		for (var i = xStart; i <= xEnd; i++)
			for (var j = yStart; j <= yEnd; j++) {
				TileMap [i, j] = cellType;
			}
	}
	
	private bool IsAreaUnused (int xStart, int yStart, int xEnd, int yEnd)
	{
		for (var i = xStart; i <= xEnd; i++)
			for (var j = yStart; j <= yEnd; j++)
				if (TileMap [i, j] != CustomTile.Default)
					return false;
		return true;
	}
	
	private void CreateRoom(int xStart, int yStart, int xEnd, int yEnd)
	{
		Rooms.Add(new Room(xStart + 1, yStart + 1, xEnd - 1, yEnd - 1));
		SetCells(xStart+1,yStart+1,xEnd-1,yEnd-1,CustomTile.Floor);
		for (var i = xStart + 1; i < xEnd ; i++) {
			TileMap [i, yStart] = CustomTile.WallA;
			TileMap [i, yEnd] = CustomTile.WallD;
		}
		for (var j = yStart + 1; j < yEnd; j++) {
			TileMap [xStart, j] = CustomTile.WallS;
			TileMap [xEnd, j] = CustomTile.WallW;
		}
	}

	private void CreateCorridor(int xStart, int yStart, int xEnd, int yEnd,CustomTile t){
		if (t == CustomTile.CorridorWS)
		{
			for (var i = xStart; i <= xEnd; i++)
			{
				TileMap [i, yStart] = t;
				if (TileMap[i, yStart + 1] != CustomTile.Floor) TileMap[i, yStart + 1] = CustomTile.WallD;
				if (TileMap[i, yStart - 1] != CustomTile.Floor) TileMap[i, yStart - 1] = CustomTile.WallA;
			}
		}

		if (t == CustomTile.CorridorAD)
		{
			for (var j = yStart; j <= yEnd; j++)
			{
				TileMap [xStart, j] = t;
				if (TileMap[xStart + 1, j] != CustomTile.Floor) TileMap[xStart + 1, j] = CustomTile.WallW;
				if (TileMap[xStart - 1, j] != CustomTile.Floor) TileMap[xStart - 1, j] = CustomTile.WallS;
			}
		}
	}

	private bool MakeRoomAndCorridor(int x,int y){
		int xStart = -1, xEnd = -1, yStart = -1, yEnd = -1;
		var width = Random.Range (_roomMinWidth, _roomMaxWidth );
		var length = Random.Range (_roomMinLength, _roomMaxLength ); 
		if (_first) {
			xStart = _mapMaxWidth / 2 - width / 2;
			yStart = _mapMaxLength / 2 - length / 2;
			xEnd = xStart + width;
			yEnd = yStart + length;

			if (!IsInBounds_x(xStart) || !IsInBounds_x(xEnd) || !IsInBounds_y(yStart) ||
			    (!IsInBounds_y(yEnd))) return true;
			if (!IsAreaUnused(xStart, yStart, xEnd, yEnd)) return false;
			_first = false;
			CreateRoom (xStart, yStart, xEnd, yEnd);
			return true;
		}

		if ((TileMap[x, y] != CustomTile.WallA) && (TileMap[x, y] != CustomTile.WallW) &&
		    (TileMap[x, y] != CustomTile.WallS) && (TileMap[x, y] != CustomTile.WallD)) return false;
		var corridorLength = Random.Range (_minCorridorLen - 2, _maxCorridorLen - 1);
		int cXStart = -1, cXEnd = -1, cYStart = -1, cYEnd = -1;
		var away = Random.Range (1, length - 1);
		var type=CustomTile.Default;
		switch (TileMap [x, y]) {
			case(CustomTile.WallA):
				xStart = x - away;
				xEnd = x + width;
				yEnd = y - corridorLength - 1;
				yStart = yEnd - length;
				cYEnd = y;
				cYStart = y - corridorLength - 1;
				cXEnd = x;
				cXStart = x;
				type = CustomTile.CorridorAD;
				break;
			case(CustomTile.WallD):
				xStart = x - away;
				xEnd = x + width;
				yStart = y + corridorLength + 1;
				yEnd = yStart + length;
				cYStart = y;
				cYEnd = y + corridorLength + 1;
				cXEnd = x;
				cXStart = x;
				type = CustomTile.CorridorAD;
				break;
			case(CustomTile.WallW):
				yStart = y - away;
				yEnd = yStart + length;
				xStart = x + corridorLength + 1;
				xEnd = xStart + width;
				cXStart = x;
				cXEnd = x + corridorLength + 1;
				cYStart = y;
				cYEnd = y;
				type = CustomTile.CorridorWS;
				break;
			case(CustomTile.WallS):
				yStart = y - away;
				yEnd = yStart + length;
				xEnd = x - corridorLength - 1;
				xStart = xEnd - width;
				cXEnd = x;
				cXStart = x - corridorLength - 1;
				cYStart = y;
				cYEnd = y;
				type = CustomTile.CorridorWS;
				break;
		}
		
		if (!IsAreaUnused(xStart, yStart, xEnd, yEnd)) return false;
		CreateRoom (xStart, yStart, xEnd, yEnd);
		CreateCorridor(cXStart, cYStart, cXEnd, cYEnd, type);
		return true;

	}
	public void MakeDungeon(int step){
		var num=0;
		for (var i = 0; i < step; i++) {
			var x = Random.Range (0,_mapMaxWidth);
			var y = Random.Range (0,_mapMaxLength);
			if (MakeRoomAndCorridor(x,y)){
				num++;
			}
			if (num==_roomNum){
				break;
			}
		}
		if (num<_roomNum){
			Debug.Log ("Cannot generate enough room. Please Enlarge the Step Num");
		}
	}
}
