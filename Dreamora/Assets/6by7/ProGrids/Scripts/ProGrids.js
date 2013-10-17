#pragma strict


//editor prefs change-able vars
var gridLineColor_XY : Color;// = Color(.2,.65,.9,.125);
var gridLineColor_XZ : Color;// = Color(.2,.65,.6,.125);
var gridLineColor_YZ : Color;// = Color(.925,.22,.384,.125);
//

var showAngles : boolean = true;  

var showGrid : boolean = true;
var drawGrid_XZ : boolean = false;
var drawGrid_XY : boolean = false;
var drawGrid_YZ : boolean = false;
var snapToGrid : boolean = false;

var gridSnapSize_Base : float = .25;
var gridUnitsIndex : int = 1;
var gridSnapSize_Factored : float = .25;

var gridCenterPos : Vector3;

private var halfSegs_H : int;
private var halfSegs_V : int;
private var visSegs : int;
var superUnitSize : int[] = [10,  10, 10, 12];

private var rightRotation = Vector3(-1,0,0);
private var leftRotation = Vector3(1,0,0);
private var topRotation = Vector3(0,-1,0);
private var bottomRotation = Vector3(0,1,0);
private var frontRotation = Vector3(0,0,-1);
private var backRotation = Vector3(0,0,1);

var camView : Vector3;
var gridObjPos : Vector3;
var nearestSnapPos : Vector3;
var activePoint : Vector3;

function FindGridCenter()
{
	if(Camera.current)
	{
		nearestSnapPos = Camera.current.transform.position;

		if(camView == rightRotation || camView == leftRotation)
		{
			nearestSnapPos.x = nearestSnapPos.x + (10*camView.x);
		}
		if(camView == backRotation || camView == frontRotation)
		{
			nearestSnapPos.z = nearestSnapPos.z + (10*camView.z);
		}
		if(camView == topRotation || camView == bottomRotation)
		{
			nearestSnapPos.y = nearestSnapPos.y + (10*camView.y);
		}
	}
	
	nearestSnapPos.x = gridSnapSize_Factored * Mathf.Round(nearestSnapPos.x / gridSnapSize_Factored); //find it's x position, rounded to the snap amount 
	nearestSnapPos.y = gridSnapSize_Factored * Mathf.Round(nearestSnapPos.y / gridSnapSize_Factored); //find it's y position, rounded to the snap amount 
	nearestSnapPos.z = gridSnapSize_Factored * Mathf.Round(nearestSnapPos.z / gridSnapSize_Factored); //find it's z position, rounded to the snap amount
	
	gridCenterPos = nearestSnapPos; //set the grid center
	
}

function CalcScreenSegs(axisIndex : int)
{
	var bottomLeftW : Vector3;
	var topRightW : Vector3;
	
	bottomLeftW = camera.current.ScreenToWorldPoint(Vector3(0,0,0));
	topRightW = camera.current.ScreenToWorldPoint(Vector3(camera.current.pixelWidth,camera.current.pixelHeight, 0));
	
	var viewWidth : float;
	var viewHeight : float;
	
	if(axisIndex == 0)
	{
		viewWidth = (bottomLeftW.x-topRightW.x);
		viewHeight = (bottomLeftW.z-topRightW.z);
	}
	else if(axisIndex == 1)
	{
		viewWidth = (bottomLeftW.x-topRightW.x);
		viewHeight = (bottomLeftW.y-topRightW.y);
	}
	else if(axisIndex == 2)
	{
		viewWidth = (bottomLeftW.z-topRightW.z);
		//Debug.Log(bottomLeftW.z+", "+topRightW.z);
		viewHeight = (bottomLeftW.y-topRightW.y);
	}
	
	if(viewWidth < 0.0)
		viewWidth*=-1;
	if(viewHeight < 0.0)
		viewHeight*=-1;
		
	halfSegs_H = Mathf.Round((viewWidth/gridSnapSize_Factored)*.5)+1;
	halfSegs_V = Mathf.Round((viewHeight/gridSnapSize_Factored)*.5)+1;
	//Debug.Log(halfSegs_H+", "+halfSegs_V);
}


function OnDrawGizmos()
{
	if(showGrid)
	{
		//EditorWindow.GetWindow.<ProGridsWindow>();
		
		//turn off all grids
		drawGrid_YZ = false;
		drawGrid_XY = false;
		drawGrid_XZ = false;
		
		//auto-set the grid based on view
		if(Camera.current.orthographic)
		{
			//get the current camera view direction
			camView = Camera.current.transform.TransformDirection(Vector3.forward);
			
			if(camView == rightRotation || camView == leftRotation)
				drawGrid_YZ = true;
			if(camView == backRotation || camView == frontRotation)
				drawGrid_XY = true;
			if(camView == topRotation || camView == bottomRotation)
				drawGrid_XZ = true;			
		}
		
		var x : float;
		var y : float;
		var z : float;
		if(drawGrid_XZ)
		{
			//draw 45 angle
			Gizmos.color = Color(.5,.5,0,.5);
			if(activePoint != null && showAngles)
			{
				Gizmos.DrawRay(Vector3(activePoint.x-100, activePoint.y, activePoint.z-100), (Vector3(1,0,1)*200));
				Gizmos.DrawRay(Vector3(activePoint.x-100, activePoint.y, activePoint.z+100), (Vector3(1,0,-1)*200));
				Gizmos.DrawRay(Vector3(activePoint.x, activePoint.y, activePoint.z+100), (Vector3(0,0,-1)*200));
				Gizmos.DrawRay(Vector3(activePoint.x-100, activePoint.y, activePoint.z), (Vector3(1,0,0)*200));
			}
			
			//Draw main grid
			FindGridCenter();
			CalcScreenSegs(0);
			Gizmos.color = gridLineColor_XZ;
			visSegs = 10;
			for(x=(halfSegs_H*-1);x<halfSegs_H+1;x++)
			{
				Gizmos.DrawRay(Vector3(gridCenterPos.x+(x*gridSnapSize_Factored), gridCenterPos.y, gridCenterPos.z-(halfSegs_V*gridSnapSize_Factored)), Vector3.forward*halfSegs_V*2*gridSnapSize_Factored);
			}
			for(z=(halfSegs_V*-1);z<halfSegs_V+1;z++)
			{
				Gizmos.DrawRay(Vector3(gridCenterPos.x-(halfSegs_H*gridSnapSize_Factored), gridCenterPos.y, gridCenterPos.z+(z*gridSnapSize_Factored)), Vector3.right*halfSegs_H*2*gridSnapSize_Factored);
			}
		}
		if(drawGrid_XY)
		{
			//draw 45 angle
			Gizmos.color = Color(.5,.5,0,.5);
			if(activePoint != null && showAngles)
			{
				Gizmos.DrawRay(Vector3(activePoint.x-100, activePoint.y-100, activePoint.z), (Vector3(1,1,0)*200));
				Gizmos.DrawRay(Vector3(activePoint.x-100, activePoint.y+100, activePoint.z), (Vector3(1,-1,0)*200));
				Gizmos.DrawRay(Vector3(activePoint.x, activePoint.y+100, activePoint.z), (Vector3(0,-1,0)*200));
				Gizmos.DrawRay(Vector3(activePoint.x-100,  activePoint.y,activePoint.z), (Vector3(1,0,0)*200));
			}
			
			FindGridCenter();
			CalcScreenSegs(1);
			Gizmos.color = gridLineColor_XY;
			for(x=(halfSegs_H*-1);x<halfSegs_H+1;x++)
			{
				/*
				//set the darker lines every major unit				
				if((gridCenterPos.x+(x*gridSnapSize_Factored)) % 10 == 0)
					Gizmos.color.a = gridLineColor_XY.a * 1.75;
				else
					Gizmos.color.a = gridLineColor_XY.a;
				*/
				
				//draw the horizontal lines
				Gizmos.DrawRay(Vector3(gridCenterPos.x+(x*gridSnapSize_Factored), gridCenterPos.y-(halfSegs_V*gridSnapSize_Factored), gridCenterPos.z), Vector3.up*halfSegs_V*2*gridSnapSize_Factored);
			}
			for(y=(halfSegs_V*-1);y<halfSegs_V+1;y++)
			{
				/*
				//set the darker lines every major unit
				if((gridCenterPos.y+(y*gridSnapSize_Factored)) % 10 == 0)
					Gizmos.color.a = gridLineColor_XY.a * 1.75;
				else
					Gizmos.color.a = gridLineColor_XY.a;
				*/
				
				//draw the vertical lines
				Gizmos.DrawRay(Vector3(gridCenterPos.x-(halfSegs_H*gridSnapSize_Factored), gridCenterPos.y+(y*gridSnapSize_Factored), gridCenterPos.z), Vector3.right*halfSegs_H*2*gridSnapSize_Factored);
			}
		}
		if(drawGrid_YZ)
		{
			//draw 45 angle
			Gizmos.color = Color(.5,.5,0,.5);
			if(activePoint != null && showAngles)
			{
				Gizmos.DrawRay(Vector3(activePoint.x, activePoint.y-100, activePoint.z-100), (Vector3(0,1,1)*200));
				Gizmos.DrawRay(Vector3(activePoint.x, activePoint.y+100, activePoint.z-100), (Vector3(0,-1,1)*200));
				Gizmos.DrawRay(Vector3(activePoint.x, activePoint.y+100, activePoint.z), (Vector3(0,-1,0)*200));
				Gizmos.DrawRay(Vector3(activePoint.x,  activePoint.y,activePoint.z-100), (Vector3(0,0,1)*200));
			}
			
			FindGridCenter();
			CalcScreenSegs(2);
			Gizmos.color = gridLineColor_YZ;
			for(z=(halfSegs_H*-1);z<halfSegs_H+1;z++)
			{
				Gizmos.DrawRay(Vector3(gridCenterPos.x, gridCenterPos.y-(halfSegs_V*gridSnapSize_Factored), gridCenterPos.z+(z*gridSnapSize_Factored)), Vector3.up*halfSegs_V*2*gridSnapSize_Factored);
			}
			for(y=(halfSegs_V*-1);y<halfSegs_V+1;y++)
			{
				Gizmos.DrawRay(Vector3(gridCenterPos.x, gridCenterPos.y+(y*gridSnapSize_Factored), gridCenterPos.z-(halfSegs_H*gridSnapSize_Factored)), Vector3.forward*halfSegs_H*2*gridSnapSize_Factored);
			}			
		}
	}
}