using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

[EditorTool("Bone Tool",typeof(VRCPhysBoneColliderBase))]
class BoneColliderEditor : EditorTool
{
    // Serialize this value to set a default value in the Inspector.
    [SerializeField]
    Texture2D m_ToolIcon;

    GUIContent m_IconContent;

    void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "Bone Tool",
            tooltip = "Bone Tool"
        };
    }

    public override GUIContent toolbarIcon
    {
        get { return m_IconContent; }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (Selection.transforms.Length == 0) return;


        for (int i = 0; i < Selection.transforms.Length; i++)
        {

            VRCPhysBoneColliderBase[] senders = Selection.transforms[i].gameObject.GetComponents<VRCPhysBoneColliderBase>();
            for (int j = 0; j < senders.Length; j++)
            {
                VRCPhysBoneColliderBase sender = senders[j];

                EditorGUI.BeginChangeCheck();

                Vector3 position = (Vector3)(sender.transform.localToWorldMatrix * sender.position) + sender.transform.position;
                Quaternion rotation = sender.transform.rotation * sender.rotation;
                Vector2 scale = new Vector2(sender.radius, sender.height);
                position = Handles.PositionHandle(position, rotation);
                rotation = Handles.RotationHandle(rotation, position);
                if (sender.shapeType != VRCPhysBoneColliderBase.ShapeType.Plane)
                    scale.x = Handles.ScaleSlider(scale.x, position, rotation * Vector3.forward, rotation, HandleUtility.GetHandleSize(position) * 0.7f, HandleUtility.GetHandleSize(position));
                if (sender.shapeType == VRCPhysBoneColliderBase.ShapeType.Capsule)
                    scale.y = Handles.ScaleSlider(scale.y, position, rotation * Vector3.up, rotation, HandleUtility.GetHandleSize(position) * 0.7f, HandleUtility.GetHandleSize(position));

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(sender, "Move Contact");
                    sender.position = (Vector3)(sender.transform.worldToLocalMatrix * (position - sender.transform.position));
                    sender.rotation = (Quaternion.Inverse(sender.transform.rotation) * rotation).GetNormalized();
                    if (sender.shapeType != VRCPhysBoneColliderBase.ShapeType.Plane)
                        sender.radius = Mathf.Max(0.0000001f, scale.x);
                    if (sender.shapeType == VRCPhysBoneColliderBase.ShapeType.Capsule)
                        sender.height = Mathf.Max(0.0000001f, scale.y);
                }
            }
        }
    }
}