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
	CorridorWS,
	Door,
	UpStairs,
	DownStairs
}

public class Map
{
	private CustomTile[,] _fullMap;
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

	private Map ()
	{
	}
	
	public static Map Instance {
		get { return _instance ??= new Map(); }
	}
	
	public void Init (int roomMaxLength,int roomMaxWidth,int roomMinLength,int roomMinWidth,int mapMaxLength,int mapMaxWidth,int roomNum,int minCorridorLen,int maxCorridorLen)
	{
		_fullMap = new CustomTile[mapMaxWidth, mapMaxLength];
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
				_fullMap [i, j] = cellType;
			}
	}
	
	private bool IsAreaUnused (int xStart, int yStart, int xEnd, int yEnd)
	{
		for (var i = xStart; i <= xEnd; i++)
			for (var j = yStart; j <= yEnd; j++)
				if (_fullMap [i, j] != CustomTile.Default)
					return false;
		return true;
	}
	
	private void CreateRoom(int xStart, int yStart, int xEnd, int yEnd){
		for (var i = xStart + 1; i < xEnd ; i++)
			for (var j = yStart + 1; j < yEnd; j++)
				_fullMap [i, j] = CustomTile.Floor;
		for (var i = xStart + 1; i < xEnd ; i++) {
			_fullMap [i, yStart] = CustomTile.WallA;
			_fullMap [i, yEnd] = CustomTile.WallD;
		}
		for (var j = yStart + 1; j < yEnd; j++) {
			_fullMap [xStart, j] = CustomTile.WallS;
			_fullMap [xEnd, j] = CustomTile.WallW;
		}
	
	}

	private void CreateCorridor(int xStart, int yStart, int xEnd, int yEnd,CustomTile t){
		if (t == CustomTile.CorridorWS)
		{
			for (var i = xStart; i <= xEnd; i++)
			{
				_fullMap [i, yStart] = t;
				if (_fullMap[i, yStart + 1] != CustomTile.Floor) _fullMap[i, yStart + 1] = CustomTile.WallD;
				if (_fullMap[i, yStart - 1] != CustomTile.Floor) _fullMap[i, yStart - 1] = CustomTile.WallA;
			}
		}

		if (t == CustomTile.CorridorAD)
		{
			for (var j = yStart; j <= yEnd; j++)
			{
				_fullMap [xStart, j] = t;
				if (_fullMap[xStart + 1, j] != CustomTile.Floor) _fullMap[xStart + 1, j] = CustomTile.WallW;
				if (_fullMap[xStart - 1, j] != CustomTile.Floor) _fullMap[xStart - 1, j] = CustomTile.WallS;
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

		if ((_fullMap[x, y] != CustomTile.WallA) && (_fullMap[x, y] != CustomTile.WallW) &&
		    (_fullMap[x, y] != CustomTile.WallS) && (_fullMap[x, y] != CustomTile.WallD)) return false;
		var corridorLength = Random.Range (_minCorridorLen - 2, _maxCorridorLen - 1);
		int cXStart = -1, cXEnd = -1, cYStart = -1, cYEnd = -1;
		var away = Random.Range (1, length - 1);
		var type=CustomTile.Default;
		switch (_fullMap [x, y]) {
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
			Debug.Log ("Cannot Generate room. Please Enlarge the Step Num");
		}
	}
	public CustomTile[,] GetMap(){
		return(_fullMap);
	}
}
