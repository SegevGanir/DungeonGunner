using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
	private GUIStyle roomNodeStyle;
	private static RoomNodeGraphSO currentRoomNodeGraph;
	private RoomNodeSO currentRoomNode = null;
	private RoomNodeTypeListSO roomNodeTypeList;

	// Node Layout values
	private const float nodeWidth = 160f;
	private const float nodeHeight = 75f;
	private const int nodePadding = 25;
	private const int nodeBorder = 12;

	// connecting line value
	private const float connectingLineWidth = 3f;

	[MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]

	private static void OpenWindow()
	{
		GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
	}

	private void OnEnable()
	{
		// Define node layout style
		roomNodeStyle = new GUIStyle();
		roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
		roomNodeStyle.normal.textColor = Color.white;
		roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		// Load Room node types
		roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
	}

	/// <summary>
	/// Open the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspector
	/// </summary>
	[OnOpenAsset(0)] // Need the namespace UnityEditor.Callbacks
	private static bool OnDoubleClickAsset(int instanceID, int line)
	{
		RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

		if (roomNodeGraph != null)
		{
			OpenWindow();

			currentRoomNodeGraph = roomNodeGraph;

			return true;
		}
		return false;
	}

	/// <summary>
	/// Draw Editor GUI
	/// </summary>
	private void OnGUI()
	{

		// If a scriptable object of type RoomNodeGraphSO has been selected then process
		if (currentRoomNodeGraph != null)
		{
			// Draw line if being dragged
			DrawDraggedLine();

			// Process Events
			ProcessEvents(Event.current);

			//Draw room nodes
			DrawRoomNodes();
		}

		if (GUI.changed)
			Repaint();
	}

	private void DrawDraggedLine()
	{
		if (currentRoomNodeGraph.linePosition != Vector2.zero)
		{
			// Draw line from node to line position
			Handles.DrawBezier(currentRoomNodeGraph.roomNodeDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
				currentRoomNodeGraph.roomNodeDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
		}
	}

	private void ProcessEvents(Event currentEvent)
	{
		// Get room node that mouse is over if it's null or not currently being dragged
		if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
		{
			currentRoomNode = IsMouseOverRoomNode(currentEvent);
		}

		// if mouse isn't over a room node
		if (currentRoomNode == null || currentRoomNodeGraph.roomNodeDrawLineFrom != null)
		{
			ProcessRoomNodeGraphEvents(currentEvent);
		}
		// else process room node events
		else
		{
			// process room node events
			currentRoomNode.ProcessEvents(currentEvent);
		}
	}

	/// <summary>
	/// Check to see if mouse is over a room node => if so then return the room node else return null
	/// </summary>
	private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
	{
		for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
		{
			if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
			{
				return currentRoomNodeGraph.roomNodeList[i];
			}
		}

		return null;
	}

	/// <summary>
	/// Process Room Node Graph Events
	/// </summary>
	private void ProcessRoomNodeGraphEvents(Event currentEvent)
	{
		switch (currentEvent.type) 
		{
			// Process Mouse Down Events
			case EventType.MouseDown:
				ProcessMouseDownEvent(currentEvent);
				break;

			case EventType.MouseUp:
				ProcessMouseUpEvent(currentEvent);
				break;

			// Process Mouse Drag Event
			case EventType.MouseDrag:
				ProcessMouseDragEvent(currentEvent);
				
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Process mouse down events on the room node graph (not over a node)
	/// </summary>
	private void ProcessMouseDownEvent(Event currentEvent)
	{
		// Process right click mouse down on a graph event (show context menu)
		if (currentEvent.button == 1)
		{
			ShowContextMenu(currentEvent.mousePosition);
		}
	}

	/// <summary>
	/// Show the context menu
	/// </summary>
	private void ShowContextMenu(Vector2 mousePosition)
	{
		GenericMenu menu = new GenericMenu();

		menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);

		menu.ShowAsContext();
	}

	/// <summary>
	/// Create a room node at the mouse position
	/// </summary>
	private void CreateRoomNode(object mousePositionObject)
	{
		CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
	}

	private void ProcessMouseUpEvent(Event currentEvent)
	{
		// if releasing the right mouse and is dragging a line
		if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeDrawLineFrom != null)
		{
			ClearLineDrag();
		}
	}

	/// <summary>
	/// Process mouse drag event
	/// </summary>
	private void ProcessMouseDragEvent(Event currentEvent)
	{
		// process right click drag event - then draw line
		if (currentEvent.button == 1)
		{
			ProcessRightMouseDragEvent(currentEvent);
		}
	}

	/// <summary>
	/// Process right mouse drag event - draw line
	/// </summary>
	private void ProcessRightMouseDragEvent(Event currentEvent)
	{
		if (currentRoomNodeGraph.roomNodeDrawLineFrom != null)
		{
			DragConnectingLine(currentEvent.delta);
			GUI.changed = true;
		}
	}

	/// <summary>
	/// Drag connecting lines from room node
	/// </summary>
	public void DragConnectingLine(Vector2 delta)
	{
		currentRoomNodeGraph.linePosition += delta;
	}

	/// <summary>
	/// Create a room node at the mouse position - overloaded to also pass in RoomNodeType
	/// </summary>
	private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
	{
		Vector2 mousePosition = (Vector2)mousePositionObject;

		// create room node scriptable object asset
		RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

		// add room node to current room node graph room node list
		currentRoomNodeGraph.roomNodeList.Add(roomNode);

		// set room node values
		roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

		// add room node to room node graph scriptable object asset database
		AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

		AssetDatabase.SaveAssets();
	}

	/// <summary>
	/// Clear line drag from a room node
	/// </summary>
	private void ClearLineDrag()
	{
		currentRoomNodeGraph.roomNodeDrawLineFrom = null;
		currentRoomNodeGraph.linePosition = Vector2.zero;
		GUI.changed = true;
	}

	/// <summary>
	/// Draw room nodes in the graph window
	/// </summary>
	private void DrawRoomNodes()
	{
		// Loop through all the room nodes and draw them
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			roomNode.Draw(roomNodeStyle);
		}

		GUI.changed = true;
	}
}