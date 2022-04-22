using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

[EditorTool("Contact Sender Tool", typeof(ContactBase))]
class ContactBaseEditor : EditorTool
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
            text = "Contact Tool",
            tooltip = "Contact Tool"
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

            ContactBase[] senders = Selection.transforms[i].gameObject.GetComponents<ContactBase>();
            for (int j = 0; j < senders.Length; j++)
            {
                ContactBase sender = senders[j];

                EditorGUI.BeginChangeCheck();

                Vector3 position = (Vector3)(sender.transform.localToWorldMatrix * sender.position) + sender.transform.position;
                Quaternion rotation = sender.transform.rotation * sender.rotation;
                Vector2 scale = new Vector2(sender.radius, sender.height);
                position = Handles.PositionHandle(position, rotation);
                rotation = Handles.RotationHandle(rotation, position);
                scale.x = Handles.ScaleSlider(scale.x, position,rotation * Vector3.forward, rotation, HandleUtility.GetHandleSize(position)*0.7f, HandleUtility.GetHandleSize(position));
                if(sender.shapeType == ContactBase.ShapeType.Capsule)
                    scale.y = Handles.ScaleSlider(scale.y, position, rotation * Vector3.up, rotation, HandleUtility.GetHandleSize(position) * 0.7f, HandleUtility.GetHandleSize(position));

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(sender, "Move Contact");
                    sender.position = (Vector3)(sender.transform.worldToLocalMatrix * (position - sender.transform.position));
                    sender.rotation = (Quaternion.Inverse(sender.transform.rotation) * rotation).GetNormalized();
                    sender.radius = Mathf.Max(0.0000001f, scale.x);
                    if (sender.shapeType == ContactBase.ShapeType.Capsule)
                        sender.height = Mathf.Max(0.0000001f, scale.y);
                }
            }
        }
    }
}