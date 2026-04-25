# Balatro 모작

## 목차
### 1. 게임 설명
### 2. 기술 설명
### 3. 트러블 슈팅
### 4. 피드백
### 5. 로드맵
---
## 1. 게임 설명
### 장르: 로그라이크, 덱 빌딩
### 개발 환경: Unity (6000.3.9f1), C#
### 인게임 플레이 영상
<img width="992" height="556" alt="Calculator" src="https://github.com/user-attachments/assets/02997e2e-5944-475e-8999-1e73633474e4" /><br>
### 게임 시연 영상: https://www.youtube.com/watch?v=8y-Ay5d1ntY
#### 게임 설명
- 게임이 시작되면 8장의 카드가 드로우된다.
- 최대 5장의 카드를 클릭하여 선택할 수 있으며, 선택된 카드는 버리기 혹은 점수 계산하기를 진행할 수 있다.
- 버리기를 선택한 경우(Discard) 버린 카드 수 만큼 다시 드로우된다.
- 점수 계산하기를 선택하는 경우(Play Hand) 다음의 순서로 진행된다.
  1. 선택된 카드를 기준으로 족보를 판정한다(ex. 원페어, 트리플..)
  2. 판정된 족보의 기본 Chip, Mult를 설정한다.
  3. 족보에 기여한 카드에 부여된 Chip을 순차적으로 합산한다.
  4. 조커 슬롯에 배치된 조커 카드를 순차적으로 효과 적용하여 Chip, Mult에 이점을 부여한다.
  5. 최종 계산된 점수가 Round Score에 합산한다.
- Round Score가 상단 Scrore at least에 표시된 목표 점수에 도달하면 승리하며, 코인을 획득한다.
- Hands를 모두 소모하였음에도 도달하지 목표 점수에 도달하지 못하면 GameOver된다.
- 승리시 코인 획득 후, 상점으로 이동하여 카드를 구매할 수 있다. (조커 카드, 행성 카드, 바우처 카드)
---
### 2.1 기술 설명 - 주요 기술 스택
---
## 2-2. 기술 설명 - 씬 흐름

### 1. Title Scene
- 게임 실행 시 최초 진입되는 화면
- 게임 방법 안내(Tutorial), 게임 시작(Play), 음량 조절(Settings)로 구성

### 2. InGame Scene
- 최초 카드 생성 및 드로우 진행
- 이후 Player가 선택한 카드를 기반으로 버리기 및 점수 계산 가능
- 점수 계산 결과에 따른 Clear Scene 혹은 Gameover Scene으로 연결

### 3. Clear Scene
- 게임에 승리하면 코인 획득
- 캐시 아웃을 클릭하여 Shop Scene으로 연결

### 4. Shop Scene
- 보유한 코인을 기반으로 카드 구매 가능
- 조커 카드 구매시 상단 슬롯에 배치 및 이후 라운드부터 점수 계산에 이점 부여
- 행성 카드 구매시 우측 상단 슬롯에 배치 및 배치된 카드를 클릭하여 족보 강화 가능
- 바우처 카드 구매시 게임 내 자원(Hands, Discards, Coin)에 이점 부여

### 5. GameOver Scene
- 게임에 패배하면 이전까지 플레이한 기록이 표시
- 게임 재시작(New Run) 및 메인 메뉴(Main Menu)로 구성
---
## 2-3. 기술 설명 - 핵심 자료 구조

### 1. List (카드 덱 및 버린카드 저장)
- 데이터의 가변성 고려 및 셔플 알고리즘의 용이성
- 삭제, 추가로인한 크기 변화의 유연한 관리를 위해 사용
```
// 카드 생성 함수
public void CreateDeck()
{
    for (int i = 0; i < 52; i++)
    {
        Suit assignedSuit = (Suit)(i / 13);
        int assignedRank = (i % 13) + 2;
        if(assignedRank == 14) assignedRank = 1;
        int chip = CalculateBaseChip(assignedRank);

        PlayCard newCard = Instantiate(cardPrefab, this.transform).GetComponent<PlayCard>();

        newCard.Initalize(i.ToString(), "Playing Card", " Chip: " + chip, assignedSuit, assignedRank, chip);
        newCard.SetSprite(cardSprites[i]);
        fullDeck.Add(newCard);

        newCard.name = newCard.GetCardName();
        newCard.gameObject.SetActive(false);
    }
}
```

### 2.Array(드로우 카드, 조커 슬롯, 행성 카드 슬롯
- 게임 내 고정된 크기 제공
- 인덱스 기반의 UI 동기화가 이루어지기 때문에 사용
```
// 생성한 카드기반 드로우 함수
public void DrawHands()
{
    List<PlayCard> newCards = new List<PlayCard>();

    for (int i = 0; i < hands.Length; i++)
    {
        if (hands[i] == null)
        {
            PlayCard drawnCard = deck.DrawCardFromDeck();

            if (drawnCard != null)
            {
                hands[i] = drawnCard;

                drawnCard.transform.SetParent(transform);
                newCards.Add(drawnCard);

            }
        }
    }
}
```

### 3. Queue(점수 계산 시 조커 카드 순차 저장 및 적용)
- Balatro의 조커 카드 계산 매커니즘은 배치한 순서대로 효과가 적용하는 것을 고려
- 조커 카드를 Queue에 저장 후, 점수 계산이 이루어지면 Dequeue()를 통해 효과 적용 함수를 순차적으로 호출
```
// 조커 카드 Queue에 저장하는 함수
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

```

### 4. Dictionary(가장 많이 플레이한 Rank 판정, 행성 카드를 통한 Rank 강화시 강화된 Rank Level 파악)
- Player의 가장 많이 플레이한 Rank 판정을 위해 Dictionary<HandRanking, int>를 통하여 핸드 플레이마다 value 값을 증가
- 행성 카드를 통한 Rank 강화시 어느 카드가 얼마만큼 강화되었는지 파악 및 후속 강화 적용을 위하여 마찬가지로 Dictionary<HandRanking, int>를 통해 레벨업 적용
```
// 가장 많이 플레이한 Rank 측정
public void CheckPlayeRanking(HandRanking ranking)
{
    if(ranking == HandRanking.None) return;

    if(handPlayHistory.ContainsKey(ranking))
    {
        handPlayHistory[ranking]++;
    }
    else
    {
        handPlayHistory[ranking] = 1;
    }
}
```
---
## 2-4. 기술 설명 - 구조 및 설계

### 1. 클래스 계층화
- 상속을 통한 BaseCard의 카드 속성 규격화
- 자식 카드(PlayCard, JokerCard, VoucherCard, PlanetCard)에서는 고유한 핵심 속성을 정의
- 모든 카드의 동일한 초기화 적용을 통한 데이터 관리의 일관성 확보
```
// BaseCard.cs
public class BaseCard : MonoBehaviour
{
    [Header("CardInformation")]
    public string cardId;
    public string cardName;
    [TextArea(1, 3)] public string description;

    public void Initalize(string cardId, string cardName, string description)
    {
        this.cardId = cardId;
        this.cardName = cardName;
        this.description = description;
    }
}
---

// PlayCard.cs
public class PlayCard : BaseCard
{
    [Header("PlayingCard Infomation")]
    public Suit suit;
    public int rank;
    public int baseChip;

    }
    public void Initalize(string cardId, string cardName, string description, Suit suit, int rank, int baseChip)
    {
        base.Initalize(cardId, cardName, description);

        this.suit = suit;
        this.rank = rank;
        this.baseChip = baseChip;

        //Debug.Log($"[PlayerCard] Suit: {this.suit}, Rank: {this.rank}, BaseChip: {this.baseChip}");
    }

```

### 2. 정적 메소드(Static)를 통한 로직 분리
- 조커 카드의 점수 계산 규칙을 정적 클래스에서 통합 관리하도록 분리하여 연산 규칙 일관성 및 코드 가독성 확보
- 인스턴스화 과정 없이 전역 호출로 설계하여 구현의 편의성 증진
- 상태 비저장을 통해 여러번 호출되는 연산에서의 G.C 부담을 줄이고 실행 속도 증진
```
using System.Collections.Generic;

public static class JokerCalculator
{
    public static bool BuildMult(JokerCard joker, List<PlayCard> selectCard, Hand hand, int count)
    {
        if (selectCard == null) return false;

        if (hand.selectCardList.Count <= 4)
        {
            // Debug.Log($"4장 이하 조건 충족 확인");
            joker.executeCount++;
        }

        joker.addMult = 1 + count;

        return true;
    }
}
```

### 3. 싱글톤 패턴을 활용한 전역 관리
- RoundManager, SoundManager, SceneManager를 싱글톤으로 설계하여 씬, 스크립트에서 Instance로 참조 가능하도록 구조 구현
- DontDestroyOnLoad로 씬 전환에도 게임 진행 데이터를 유실 없이 보존
- 여러 스크립트에서 변화되는 게임 정보를 이벤트-구독 기반으로 실시간 갱신 가능하도록 구현하여 Decoupling 설계 및 데이터 정합성 유지
```
public class RoundManager : MonoBehaviour
{
  public static RoundManager Instance;
  private void Awake()
  {
      if (Instance == null)
      {
          Instance = this;
          transform.SetParent(null);

          DontDestroyOnLoad(gameObject);
      }
      else
      {
          Destroy(gameObject);
          return;
      }
  }
}
```

### 4. DOTween 기반의 애니메이션 연출
- 스크립트 제어 기반 애니메이션 구현
- 발라트로 특유의 순차적 계산(카드별 계산, 조커카드별 효과 적용..)을 Sequence를 통해 순차적으로 애니메이션을 적용하여 리드미컬한 연출 표현
- Punch, Fade In/Out 적용을 통한 카드, 게임 정보, 씬 전환 애니메이션 구현
```
using DG.Tweening; //DOTween 전용

public class VoucherCard : BaseCard
{
  public void PunchVoucher(Action onComplete)
  {
      if (visualRoot == null) return;
  
      visualRoot.DOKill();
  
      visualRoot.DOPunchPosition(new Vector3(10f, 0, 0), 0.5f, 15, 1f);
  
      visualRoot.DOPunchRotation(new Vector3(0, 0, 15f), 0.5f, 15, 1f)
            .OnComplete(() => {onComplete?.Invoke();});
      if (effectSound != null) SoundManager.Instance.PlaySfxOneShot(effectSound, 0.3f);
  
  }
}
```
---
## 3. 트러블 슈팅

### 3-1. 싱글톤 파괴
#### Issue
InGameScene에 배치한 싱글톤 RoundManager가 씬 전환시 파괴됨

#### Process
1. 싱글톤이 씬전환에도 유지되도록 하는 코드 DontDestroyOnLoad가 구현되지 않았는지 확인했지만 정상 구현됨

2. 모든 씬에 RoundManager를 배치해보았으나 여전히 파괴가 되었으며, 게임의 흐름상 불필요한 배치기때문에 다시 제거

3. Debug를 통해 오브젝트가 파괴되는것인지 유지는 되지만 다른 곳에 원인이 있는지 확인해본 결과 파괴되는것으로 재확인

#### Solution
InGameSecene Hierarchy에서 Manager 오브젝트 하위에 RoundManager를 배치하였고 씬 전환으로 인한 부모오브젝트의 파괴로 자식오브젝트도 같이 파괴
-> RoundManager 배치를 최상위 오브젝트로 재배치. 이후, 다른 싱글톤에서 반복 실수를 방지하기 위해 아래 코드 추가한 후, 테스트한 결과 Issue 해결
```
public class RoundManager : MonoBehaviour
{
  public static RoundManager Instance;
  private void Awake()
  {
      if (Instance == null)
      {
          Instance = this;
          transform.SetParent(null);

          DontDestroyOnLoad(gameObject);
      }
      else
      {
          Destroy(gameObject);
          return;
      }
  }
}
```
#### Retrospective
1. Hierarchy에 오브젝트 배치에 있어 계층의 종속에 대한 이해와 중요성을 인지
2. 메소드 내 로직상의 방어코드 외의 UI에 대한 방어코드 중요성을 인지

### 3-2. 개임 재시작에 따른 JokerCard 오브젝트 유지 오류
#### Issue
게임 재시작시 게임의 모든 정보를 초기화 하도록 함수를 구현하였지만 초기화 과정에서 슬롯에 배치된 조커카드가 삭제되지 않음

#### Process
1. 각각의 정보를 초기화하는 함수에 Debug를 통해 확인했지만 구현한 의도대로 코드가 동작된것으로 확인되어 데이터는 정상초기화됨. -> 이 단계에서 데이터 외에 오브젝트도 파괴해야함을 인지

2. 조커이미지(오브젝트) 제거를 위해 GameOver에 연결된 스크립트에서 OnDestroy 라이프사이클에서 오브젝트 Destroy를 시도하였으나 오브젝트가 여전히 유지.

3. 조커슬롯에서 초기화 함수를 진행할 때, 조커카드를 Destroy 시도하였지만 상황 유지

4. 조커카드에 대하여 SetActive(false)를 통해 이미지를 안보이도록 시도하였지만 여전히 유지

5. 조커카드 정보를 담고있는 hasJokerCards를 초기화하기 이전에 순회하여 Sprite를 null 처리를 시도하였지만 해결되지 않았고 추후 Sprite를 다시 연결해야할 수도 있기 때문에 접근이 잘못 되었음을 파악

#### Solution
조커카드가 jokerSlotArea 하위에 배치되어 생성되는데 조커슬롯이 해당 이슈 당시 싱글톤으로 구현되어 파괴되지 않았다보니  하위에 배치된 오브젝트 파괴되지 않은 것으로 추측
-> 조커 슬롯 하위 오브젝트를 순회하여 파괴하도록 코드 수정 후 테스트한 결과 Issue 해결
```
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
```

#### Retrospective
1. 초기화의 개념은 단순한 데이터 초기화가 아닌 물리적인 오브젝트의 파괴도 고려하는것임을 학습
2. 이후 다른 오류에서 단순 코드 상의 오류만 확인하는 것이 아닌 Hierarchy에 배치상의 오류도 고려하는것이 중요함을 학습
---
## 4. 피드백 반영
개발에 있어 담당 개발자가 직접 테스트할 경우 무의식적으로 회피적인 테스트를 할 수 있기에 제3자를 통한 테스트를 진행하였고 다음의 오류사항이 있어 수정 진행

### 1. 점수 계산중의 카드 RayCast 비활성화
#### Issue
점수 계산 도중 카드 클릭 활성화로 인하여 핸드의 남아있는 카드와 계산중인 카드가 클릭되어 오류 발생.

#### Solution
점수 계산도중에는 SelectCard, Hands의 RayCast를 비활성화되도록 처리

### 2. 튜토리얼 부재
#### Issue
테스트시 게임 설명 부재로 인한 테스트 진행의 어려움 발생 및 게임 플레이의 핵심 정보인 카드 족보의 이해도 부족으로 인해 튜토리얼의 필요성 인식

#### Solution
1. Title Scene(실행시 최초 화면)에서 Tutorial 버튼 생성 및 연결하여 게임 진행 방식을 안내
2. InGame, Clear, Shop Scene에서 ESC키 입력시 메뉴 활성화. 메뉴에 Rank정보, 음량 조절, 게임 재시작 등 기능을 추가 Rank정보의 경우 Tutorial과 동일한 방식으로 구현
---
## 5. 로드맵
해당 항목은 최종 개발이 완료된 이후 회고하였을 때, 원작 Balatro와 비교하여 부족한 기능을 기반으로 지속적인 유지보수 및 추가 구현을 진행하기 위한 예정사항을 작성함

### 1. Refactoring
- 현재 동일한 동작을 하지만 파라미터가 다르다는 이유로 분리한 함수를 공통처리 필요
- 상속 구조를 통해 더 많은 로직을 BaseCard에 통합하여 재사용성 증진
- 속성 정리 및 함수 순서 변경등을 통한 코드 가독성 증가를 목표로함

### 2. 조커카드 효과 다양화 및 카드 계산 흐름 보완
- 원작에 비해 부족한 수의 조커카드 효과를 제공하고 있어 게임의 재미를 위해 더 다양한 조커카드 추가 예정
- 일부 조커카드의 경우 족보에 기여한 카드별 계산 도중 조커 효과가 발동되는데 현재는 미구현되어 있어 추가 예정

### 3. TaroCard 추가
- 기존 PlayCard에 다양한 효과(카드 점수 강화, 삭제, 변경..)를 부여하는 TaroCard 미구현에 따른 추가 예정

### 4. GameMode 다양화
- 현재는 고정된 수치로 게임 시작시 자원 제공
- 원작을 참고하여 게임 시작 전, 다양한 자원의 이점을 주는 모드를 선택가능하도록 구현 예정

### 5. Collction 추가
- 게임내 업적 및 사용했던 카드의 달성도를 플레이어에게 제공하여 게임에 더 몰입할 수 있도록 구현 예정

---
안녕하세요. 게임 개발자를 희망하는 김규수라고합니다.
아직 부족한 점이 많아 피드백 주실 사항이 있다면 언제든 환영입니다! 수용하고 꼭 보완하겠습니다.
감사합니다!




