using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// ������������ �� ����� ���������� ����������� FloatingScore
public enum eFSState
{
    idel,
    pre,
    active,
    post
}
//FloatingScore ����� ������������ �� ������ �� ����������,
//������� ������������ ������ �����
public class FloatingScore : MonoBehaviour
{
    [Header("Set D")]
    public eFSState state = eFSState.idel;
    [SerializeField]
    protected int _score = 0;
    public string scoreString;
    //������ score ������������� ��� ����, _score � scorestring
    public int score
    {
        get { return _score; }
        set
        {
            _score = value;
            scoreString = _score.ToString("N0");
            GetComponent<Text>().text = scoreString;
        }//NO ������� �������� ���� � �����
    }
    public List<Vector2> bezierPts;// �����, ������������ ������ �����
    public List<float> fontsizes;// ����� ������ ����� ��� ���������������
                                 // ������
    public float timestart = -1f;
    public float timeDuration = -1f;
    public string easingCurve = Easing.InOut;// ������� �����������
                                             // �� Utils.cs
                                             // ������� ������, ��� �������� ����� ������ ����� SendMessage, ����� ����
                                             // ��������� FloatingScore �������� ��������
    public GameObject reportFinishTo = null;
    private RectTransform rectTrans;
    private Text txt;
    // ��������� FloatingScore � ��������� ��������
    // �������� ��������, ��� ��� ���������� eTimeS � eTimeD ����������
    // �������� �� ���������
    public void Init(List<Vector2> ePts, float eTimeS = 0, float eTimeD = 1)
    {
        rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.zero;
        txt = GetComponent<Text>();
        bezierPts = new List<Vector2>(ePts);
        if (ePts.Count == 1)
        {
            // ���� ������ ������ ���� �����
            // ...������ ������������� � ���.
            transform.position = ePts[0];
            return;
        }
        // ���� eTimeS ����� �������� �� ���������,
        // ��������� ������ �� �������� �������
        if (eTimeS == 0) eTimeS = Time.time;
        timestart = eTimeS;
        timeDuration = eTimeD;
        state = eFSState.pre;// ���������� ��������� pre - ����������
                             // ������ ��������
    }
    public void FSCallback(FloatingScore fs)
    {
        // ����� SendMessage ������� ��� �������,
        // ��� ������ �������� ���� �� ���������� ���������� FloatingScore
        score += fs.score;
    }
    private void Update()
    {
        // ���� ���� ������ ������ �� ������������, ������ �����
        if (state == eFSState.idel) return;
        // ��������� u �� ������ �������� ������� � ����������������� ��������
        // � ���������� �� 0 �� 1 (������)
        float u = (Time.time - timestart) / timeDuration;
        // ������������ ����� Easing �� Utils ��� ������������� �������� �
        float uC = Easing.Ease(u, easingCurve);
        if (u < 0)// ���� �<0, ������ �� ������ ���������
        {
            state = eFSState.pre;
            txt.enabled = false;//��������� ������ �����
        }
        else
        {
            if (u >= 1)// ���� u>=l, ����������� ��������
            {
                uC = 1;// ���������� uC=l, ����� �� ����� �� ������� �����
                state = eFSState.post;
                if (reportFinishTo != null)// ���� ������� ������ ������
                                           // ������������ SendMessage ��� ������ ������ FSCallback
                                           // � �������� ��� �������� ���������� � ���������.
                {
                    reportFinishTo.SendMessage("FSCallback", this);
                    // ����� �������� ��������� ���������� gameObject
                    Destroy(gameObject);
                }
                else// ���� �� ������
                    // ...�� ���������� ������� ���������. ������ �������� ���
                    // � �����.
                {
                    state = eFSState.idel;
                }
            }
            else
            {
                // ���� 0<=�<1, ������, ������� ��������� ������� � ��������
                state = eFSState.active;
                txt.enabled = true;//�������� ����� �����
            }
            // ������������ ������ ����� ��� ����������� � �������� �����
            Vector2 pos = Utils.Bezier(uC, bezierPts);
            // ������� ����� RectTransform ����� ������������ ��� ����������������
            // �������� ����������������� ���������� ������������ ������
            // ������� ������
            rectTrans.anchorMin = rectTrans.anchorMax = pos;
            if (fontsizes != null && fontsizes.Count > 0)
            {
                // ���� ������ fontsizes �������� ��������
                //...��������������� fontsize ����� ������� GUIText
                int size = Mathf.RoundToInt(Utils.Bezier(uC, fontsizes));
                GetComponent<Text>().fontSize = size;
            }
        }
    }
}
