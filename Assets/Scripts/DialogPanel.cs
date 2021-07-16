using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class DialogPanel : MonoBehaviour
{
    private List<TextVO> list;
    private RectTransform panel; //대화 창 패널

    public TMP_Text dialogText; // 텍스트매시 프로의 다이얼로그 창
    private WaitForSeconds shortWs = new WaitForSeconds(0.2f); //글자가 찍히는 속도

    private bool clickToNext = false; // 다음 대화로 넘기기 위한 클릭이 나타났는가?
    private bool isOpen = false; //대화창이 열렸는가?

    public GameObject nextIcon; //다음으로 넘기는 아이콘
    public GameObject typeEffectParticle; //타이핑 이펙트 파티클
    public Image profileImage; //프로필
    public AudioClip typeClip; //타이핑하는 소리

    private int currentIndex; //현재 대화 인덱스
    private RectTransform textTransform; //텍스트 창의 크기

    private Dictionary<int, Sprite> imageDictionary = new Dictionary<int, Sprite>();

    private void Awake()
    {
        panel = GetComponent<RectTransform>();
        textTransform = dialogText.GetComponent<RectTransform>();
    }

    public void StartDialog(List<TextVO> list)
    {
        this.list = list;
        ShowDialog();
    }

    public void ShowDialog()
    {
        currentIndex = 0;
        panel.DOScale(new Vector3(1, 1, 1), 0.8f).OnComplete(() =>
        {
            GameManager.TimeScale = 0f;

            TypeIt(list[currentIndex]);
            isOpen = true;
        });
    }

    public void TypeIt(TextVO vo)
    {
        int idx = vo.icon;
        //이미지 딕셔너리에서 이미지를 찾아다가 보여주는 로직을 만들어야 해.

        if (!imageDictionary.ContainsKey(idx))
        {
            Sprite img = Resources.Load<Sprite>($"profile{idx}");
            imageDictionary.Add(idx, img);
        }

        profileImage.sprite = imageDictionary[idx];

        dialogText.text = vo.msg;
        nextIcon.SetActive(false);
        clickToNext = false;
        StartCoroutine(Typing());
    }

    IEnumerator Typing()
    {
        dialogText.ForceMeshUpdate(); //이게 텍스트 정보
        dialogText.maxVisibleCharacters = 0;
        // 20글자
        int totalVisibleChar = dialogText.textInfo.characterCount; //쓰여진 텍스트의 글자 수 전체
        for (int i = 1; i <= totalVisibleChar; i++)
        {
            dialogText.maxVisibleCharacters = i;

            //Vector3 pos = dialogText.textInfo.characterInfo[i - 1].bottomRight;
            //Vector3 tPos = textTransform.TransformPoint(pos);

            //사운드재생 

            if (clickToNext)
            {
                dialogText.maxVisibleCharacters = totalVisibleChar;
                break;
            }
            yield return shortWs;
        }
        //여기까지 왔다면 한개의 텍스트가 재생된거
        currentIndex++;
        clickToNext = true;
        nextIcon.SetActive(true);
    }

    private void Update()
    {
        if (!isOpen) return;

        //텍스트 하나가 다 재생되었고 스페이스 키가 눌린경우에 해당
        if (Input.GetButtonDown("Jump") && clickToNext)
        {
            if (currentIndex >= list.Count)
            {
                panel.DOScale(new Vector3(0, 0, 1), 0.8f).OnComplete(() =>
                 {
                     // 게임매니저의 시간 조절기능을 만들어야 돼
                     GameManager.TimeScale = 1f;
                     isOpen = false;
                 });
            }
            else
            {
                TypeIt(list[currentIndex]);
            }
        }
        else if (Input.GetButtonDown("Jump"))
        {
            clickToNext = true;
        }
    }
}