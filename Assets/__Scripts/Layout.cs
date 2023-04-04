using System.Collections.Generic;
using UnityEngine;
// ����� SlotDef �� ��������� MonoBehaviour, ������� ��� ���� �� ���������
// ��������� ��������� ���� �� �#.
[System.Serializable]
public class SlotDef
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stragger;
}
public class Layout : MonoBehaviour
{
    public PT_XMLReader xmlr;
    public PT_XMLHashtable xml;// ������������ ��� ��������� ������� � xml
    public Vector2 multiplier;// �������� �� ������ ���������
    // ������ SlotDef
    public List<SlotDef> slotDefs;// ��� ���������� SlotDef ��� ����� 0-3
    public SlotDef drawPile;
    public SlotDef discarPile;
    //������ ����� ���� �����
    public string[] sortingLayerNames = new string[] {"Row0","Row1",
    "Row2","Row3","Discard","Draw"};
    //��� ������� ���������� ��� ������ ����� LayoutXML.xml
    public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);//��������� XML
        xml = xmlr.xml["xml"][0];//� ������������ xml ��� ��������� ������� � XML
        // ��������� ���������, ������������ ���������� ����� �������
        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        // ��������� �����
        SlotDef tSD;
        // slotsX ������������ ��� ��������� ������� � ��������� <slot>
        PT_XMLHashList slotX = xml["slot"];
        for (int i = 0; i < slotX.Count; i++)
        {
            tSD = new SlotDef();// ������� ����� ��������� SlotDef
            if (slotX[i].HasAtt("type"))
            {
                // ���� <slot> ����� ������� type, ��������� ���
                tSD.type = slotX[i].att("type");
            }
            else
            {
                // ����� ���������� ��� ��� "slot"; ��� ��������� ����� � ����
                tSD.type = "slot";
            }
            // ������������� ��������� �������� � �������� ��������
            tSD.x = float.Parse(slotX[i].att("x"));
            tSD.y = float.Parse(slotX[i].att("y"));
            tSD.layerID = int.Parse(slotX[i].att("layer"));
            // ������������� ����� ���� layerlD � ����� layerName
            tSD.layerName = sortingLayerNames[tSD.layerID];/*���� layerName ���������� SlotDef ������������ ���������� ����������
����. � ��������� �������� Unity ��� ������� ���������� ����� ���� � ��
�� ���������� Z, ������� ���������� �������������� ���������� � �����,
����� ����������, ��� ������ ������������� �������.*/
            switch (tSD.type)
            {// ��������� �������������� ��������, �������� �� ��� �����
                case "slot":
                    tSD.faceUp = (slotX[i].att("faceup") == "1");
                    tSD.id = int.Parse(slotX[i].att("id"));
                    if (slotX[i].HasAtt("hiddenby"))
                    {
                        string[] hiding = slotX[i].att("hiddenby").Split(',');
                        foreach (string s in hiding)
                        {
                            tSD.hiddenBy.Add(int.Parse(s));
                        }
                    }
                    slotDefs.Add(tSD);
                    break;
                case "drawpile":
                    tSD.stragger.x = float.Parse(slotX[i].att("xstagger"));
                    drawPile = tSD;
                    break;
                case "discardpile":
                    discarPile = tSD;
                    break;
            }
        }
    }
}
