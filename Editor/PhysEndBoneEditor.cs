using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

[EditorTool("PhysBones Tool", typeof(VRCPhysBoneBase))]
class PhysEndBoneEditor : EditorTool
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
            text = "PhysBones Tool",
            tooltip = "PhysBones Tool"
        };
    }

    public override GUIContent toolbarIcon
    {
        get { return m_IconContent; }
    }

    // This is called for each window that your tool is active in. Put the functionality of your tool here.
    public override void OnToolGUI(EditorWindow window)
    {
        if (Selection.transforms.Length == 0) return;

        for (int i = 0; i < Selection.transforms.Length; i++)
        {

            VRCPhysBoneBase[] senders = Selection.transforms[i].gameObject.GetComponents<VRCPhysBoneBase>();
            for (int j = 0; j < senders.Length; j++)
            {
                VRCPhysBoneBase sender = senders[j];
                Transform t = sender.rootTransform != null ? GetTransformAtChild(sender.rootTransform) : GetTransformAtChild(sender.transform);
                EditorGUI.BeginChangeCheck();

                Vector4 pos4 = t.localToWorldMatrix * sender.endpointPosition;
                Vector3 position = new Vector3(pos4.x, pos4.y, pos4.z) + t.position;
                position = Handles.PositionHandle(position, t.rotation);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(sender, "Move Contact");
                    pos4 = t.worldToLocalMatrix * (position - t.position);
                    sender.endpointPosition = new Vector3(pos4.x, pos4.y, pos4.z);
                }
            }
        }
    }

    public Transform GetTransformAtChild(Transform rootTransform)
    {
        if(rootTransform.childCount == 0)
        {
            return rootTransform;
        }

        Transform t = rootTransform.GetChild(0);
        return GetTransformAtChild(t);
    }
}