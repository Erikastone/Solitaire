using System.Collections.Generic;
using UnityEngine;
// CBState �������� ��������� ���� � ��������� to..., ����������� �������� 
/*������������ CBState �������� ��������� ��������� ����� CardBartok
� ��������� to..., �������������� ������ ����� � ������� ���������������
������������� �������.*/
public enum CBState
{
    toDrawpile,
    drawpile,
    toHard,
    hand,
    toTarget,
    target,
    discard,
    to,
    idle
}
public class CardBartok : Card
{
    static public float MOVE_DURATION = 0.5f;
    static public string MOVE_EASING = Easing.InOut;
    static public float CARD_HEIGHT = 3.5f;
    static public float CARD_WIDTH = 2f;
    [Header("Set D")]
    public CBState state = CBState.drawpile;
    // ���� � �����������, ����������� ��� ����������� � ������������� �����
    public List<Vector3> bezierPts;
    public List<Quaternion> beizerRots;
    public float timeStart, timeDuration;
    //�� ���������� ����������� ����� ����� ���������
    //reportFinishTo.SendMessage()
    public GameObject reportFinishTo = null;
    // MoveTo ��������� ����������� ����� � ����� �������������� � ��������
    // ���������
    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        // ������� ����� ������ ��� ������������.
        // ���������� ����������� � �������� ������������ ����� ������� ������.
        bezierPts = new List<Vector3>();
        bezierPts.Add(transform.localPosition);// ������� ��������������
        bezierPts.Add(ePos);//����� ����������������

        beizerRots = new List<Quaternion>();
        beizerRots.Add(transform.rotation);// ������� ���� ��������
        /*���� ���������� timestart ������ 0, �������� � ��� ������� ����� (�����
���������� ������ �����������), ����� ����������� �������� � ������
timestart. �� ���� ���� ������ � ���������� timestart ���� ��������
��������, �������� �� 0, ��� �������� �� ����������. ��� �������� ��� ����������������
������ ������������ �������.*/
        beizerRots.Add(eRot);//�����

        if (timeStart == 0)
        {
            timeStart = Time.time;
        }
        //timeDuration ������ �������� ���� �        
        //�� �� ��������, �� ����� ��� ����� ���������
        timeDuration = MOVE_DURATION;/*������������� ��������������� ��������� CBState.to. ������� ����������
����� ��������� ����������� ���������, CBState.toHand ��� CBState.
toTarget*/
        state = CBState.to;
    }
    public void MoveTo(Vector3 ePos)/*������������� ������ MoveT� (), ������� �� ������� �������� ���� ��������.*/
    {
        MoveTo(ePos, Quaternion.identity);
    }
    private void Update()
    {
        switch (state)/*��� ��� ���������� switch ��������� ����������� �������������� �����
����������, ���� ��� �� ����������� ������� �������������� �����, ���
��������� to... (�� ���� toHand, toTarget � �. �.), ����� ����� ������������
�� ������ �������������� � ������, �������������� ����� � ��� �� ������
����.*/
        {
            case CBState.toHard:
            case CBState.toTarget:
            case CBState.drawpile:
            case CBState.to:
                float u = (Time.time - timeStart) / timeDuration;/*�������� ���������� float � ��������������� � ��������� �� 0 �� 1 � ��������
����������� ���� ����� CardBartok. ��� ����������� ��� ���������
�������, ���������� � ������� timestart, � �������� �����������������
����������� (��������, ���� timestart = 5, timeDuration = 10 � Time.time
= 11, ����� u = (11-5) / 10 = 0.6). ����� ���������� �������� � ���������� � ����� Easing. Ease(), � �������� Utils,cs, ��� ���������� �������� uC,
����� ���������� ����� ������������ ������ ����������� �����. ��������������
���������� �� ������� � ������� ������������ ��� ��������
������������ ���������� �.*/
                float uC = Easing.Ease(u, MOVE_EASING);/*������ � ��������� �������� � ��������� �� 0 �� 1. ����� ��������������
��������, ����� � < 0. � ���� ������ ����� ������ ���������� ��� ��������
� ����� ������� �������. ������ � < 0 ��������, ����� ���������� timeStart
��������� ����� � �������, ����� ������ ����������� � ���������.*/
                if (u < 0)
                {
                    transform.localPosition = bezierPts[0];
                    transform.rotation = beizerRots[0];
                    return;
                }
                else if (u >= 1)/*���� u >= 1, �������� � ����� ����� �� 1, ����� ����� �� ������������� ������
�������� �����. ����� ��� �������� ������������� �������, ����� ��������
������ ������������ ������������� � ������ ��������� CBState.*/
                {
                    uC = 1;
                    //��������� �� ��������� to... � ���������������
                    //�� ���������
                    if (state == CBState.toHard) state = CBState.hand;
                    if (state == CBState.toTarget) state = CBState.target;
                    if (state == CBState.toDrawpile) state = CBState.drawpile;
                    if (state == CBState.to) state = CBState.idle;
                    // ����������� � �������� ��������������
                    transform.localPosition = bezierPts[bezierPts.Count - 1];
                    transform.rotation = beizerRots[bezierPts.Count - 1];
                    //�������� timeStart � 0, ����� �� ��� ����� ���� ���������� ������� �����
                    timeStart = 0;
                    if (reportFinishTo != null)/*���� ����� ������� ������ ��� ��������� ������, ����� � �������
SendMessage() ���������� ������� ����� CBCallback � ���������� this.
����� ������ SendMessage() � reportFinishTo ����� �������� null, �����
� ������� ��� ����� CardBartok �� �������� ��������� � ����� �����������
����� �� �������� �������.*/
                    {
                        reportFinishTo.SendMessage("CBCallback", this);
                        reportFinishTo = null;
                    }
                    else
                    {
                        // ���� ������ �������� �� ����
                        // �������� ��� ��� ����.
                    }
                }
                else/*����� 0 <= u < 1, ������ ��������������� ����������� �� ������� �����
� ���������. ��� ���������� ����� ��������� ������������ �������
������������ ����� ������ �����. ���������� ������ ��������������
� ������ ���� �������� ����������� �������� ������� �������������
�������� ������ Utils. Bezier(). �������������� ���������� �� �������
� ������� ������� ����� ���������� �.*/
                {
                    // ���������� ����� ������������ (0 <= � < 1)
                    Vector3 pos = Utils.Bezier(uC, bezierPts);
                    transform.localPosition = pos;
                    Quaternion rotQ = Utils.Bezier(uC, beizerRots);
                    transform.rotation = rotQ;
                }
                break;
        }
    }
}
