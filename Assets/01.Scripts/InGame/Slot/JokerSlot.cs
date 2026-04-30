using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JokerSlot : MonoBehaviour
{
    public static JokerSlot Instance;

    [Header("JokerSlot")]
    public JokerCard[] hasJokerCards = new JokerCard[5];
    public Queue<JokerCard> jokerEffect = new Queue<JokerCard>();
    public int hasJokerCount;
    public TextMeshProUGUI hasJokerCountText;

    [Header("JokerImage")]
    public Sprite[] jokerSprites;
    public GameObject jokerPrefab;
    public Transform slotPosition;

    public Transform jokerSlotArea;

    public static event Action<int, float> jokerChipChanged;

    private void Awake()
    {
        transform.SetParent(null);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetupSlotPosition(Transform anchor)
    {
        RectTransform myRect = GetComponent<RectTransform>();
        RectTransform anchorRect = anchor as RectTransform;


        if (myRect != null && anchorRect != null)
        {
            myRect.position = anchorRect.position;
            myRect.sizeDelta = anchorRect.sizeDelta;
            myRect.localScale = anchorRect.localScale;
        }

        gameObject.SetActive(true);
    }

    public void AddJoker(JokerCard joker)
    {
        //Debug.Log($"AddJoker, 동작확인");
        // 잘못된 구매
        if (joker == null) return;

        // 조커 슬롯 Full
        if (hasJokerCount == 5) return;

        for (int i = 0; i < hasJokerCards.Length; i++)
        {
            if (hasJokerCards[i] == null)
            {
                hasJokerCards[i] = joker;
                hasJokerCount++;

                hasJokerCards[i].transform.SetParent(this.jokerSlotArea);

                hasJokerCards[i].SetCardState(true);

                hasJokerCards[i].VisualSelect(false);

                hasJokerCards[i].transform.localPosition = Vector3.zero;
                hasJokerCards[i].transform.localScale = Vector3.one;

                break;
            }
        }
        UpdateHasJokerCount();
    }

    public void RemoveJoker(JokerCard joker)
    {
        if (joker == null) return;

        for (int i = 0; i < hasJokerCards.Length; i++)
        {
            if (hasJokerCards[i] == joker)
            {
                joker.gameObject.SetActive(false);
                joker.transform.SetParent(null);
                hasJokerCards[i] = null;
                hasJokerCount--;
                break;
            }
        }
        UpdateHasJokerCount();
    }

    public void AddJokerSequence(Sequence sequence, JokerScoreRecord score, Action onUpdateUI)
    {
        if (hasJokerCount == 0) return;

        jokerEffect.Clear();
        FillJokerEffect();

        while (jokerEffect.Count > 0)
        {
            JokerCard jokerCard = jokerEffect.Dequeue();
            if (jokerCard == null) continue;

            if (jokerCard.CheckAndCalculate(out int chip, out float mult))
            {
                sequence.AppendCallback(() => {
                    score.totalChip += chip; 
                    score.totalMult += mult;

                    jokerCard.PunchJokerCard(); 
                    onUpdateUI?.Invoke();      
                });

                sequence.AppendInterval(0.2f); 
            }
        }
    }

    private void FillJokerEffect()
    {
        for (int i = 0; i < hasJokerCards.Length; i++)
        {
            if (hasJokerCards[i] != null)
            {
                jokerEffect.Enqueue(hasJokerCards[i]);
            }
        }
    }


    private void UpdateHasJokerCount()
    {
        if (hasJokerCountText != null)
        {
            if(hasJokerCount == 0) hasJokerCountText.text = $"0";

            hasJokerCountText.text = $"{hasJokerCount}/5";
        }
    }

    // 트러블슈팅
    // 상황: RestartGame 진행시 Round정보와 JokerSlot의 정보를 초기화시키고 싶었고 SceneSwitch에서 SetRoundInfo()와 ResetJokerSlot()함수를 호출하였다.
    // 라운드정보는 정상 초기화되었지만 조커정보는 유지되고있다.
    // 원인 분석: 호출에 이상이 있는지 확인하였으나 호출은 정상적으로 이루어졌다.
    // 인스펙터를 확인해보니 JokerSlot의 정보는 구현한 코드대로 동작하였다( = 초기화가 이루어졌다)
    // 하지만 JokerSlot의 JokerCard 이미지가 남아있다.
    // 해결시도 1. JokerSlot이 싱글톤이니 게임오버에서 OnDestroy()라이프사이클에서 Destroy()처리를 시도하였다. x
    // 해결시도 2. ResetJokerSlot()에서 JokerCard프리팹을 Destroy시도하였다.x
    // 해결시도 3. ResetJokerSlot()에서 JokerCard에 대하여 SetActive(false)를 통해 이미지를 안보이도록 처리하려고 하였다. x
    // 해결시도 4. hasJokerCards를 초기화하기 이전에 순회하여 JokerCard의 UnSetSprite(sprite = null처리)를 호출하였지만 런타임환경에서 멈추었다. x
    // 해결: 배치된 JokerCard는 jokerSlotArea 하위에 배치되어 사라지지않은거기 때문에 하위 transform을 찾아 Destroy처리해야한다는 AI의 답변이 있어 foreach를 통해 제거처리하였다.
    // ==> 데이터는 초기화되지만 오브젝트를 제거하기 위해서는 부모 오브젝트를 찾아 그 하위의 오브젝트를 파괴해야 의도한대로 초기화를 진행할 수 있다.

    public void ResetJokerSlot()
    {
        jokerEffect.Clear();
        hasJokerCount = 0;
        hasJokerCards = new JokerCard[5];

        if(jokerSlotArea != null)
        {
            foreach(Transform child in jokerSlotArea)
            {
                for (int i = jokerSlotArea.childCount - 1; i >= 0; i--)
                {
                    Destroy(jokerSlotArea.GetChild(i).gameObject);
                }
            }
        }
        UpdateHasJokerCount();
    }
}
