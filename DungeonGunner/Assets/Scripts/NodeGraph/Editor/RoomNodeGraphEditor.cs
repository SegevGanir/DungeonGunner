using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
	private GUIStyle roomNodeStyle;
	private static RoomNodeGraphSO currentRoomNodeGraph;
	private RoomNodeTypeListSO roomNodeTypeList;

	// Node Layout values
	private const float nodeWidth = 160f;
	private const float nodeHeight = 75f;
	private const int nodePadding = 25;
	private const int nodeBorder = 12;

	[MenuItem("Room Node Graph Editor", menuItem = "Window / Dungeon Editor / Room Node Graph Editor")]

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
			// Process Events
			ProcessEvents(Event.current);

			//Draw room nodes
			DrawRoomNodes();
		}

		if (GUI.changed)
			Repaint();
	}

	private void ProcessEvents(Event currentEvent)
	{
		ProcessRoomNodeGraphEvents(currentEvents);
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

			default:
				break;
		}
	}
}
