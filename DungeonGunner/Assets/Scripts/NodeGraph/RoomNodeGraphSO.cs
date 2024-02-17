using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]

public class RoomNodeGraphSO : ScriptableObject
{
	[HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
	[HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
	[HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

	#region Editor Code

#if UNITY_EDITOR

	[HideInInspector] public RoomNodeSO roomNodeDrawLineFrom = null;
	[HideInInspector] public Vector2 linePosition;

	public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
	{
		roomNodeDrawLineFrom = node;
		linePosition = position;
	}

#endif

	#endregion Editor Code
}
