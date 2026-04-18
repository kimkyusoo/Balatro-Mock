using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    private PlayerInput playerInput;

    [Header("Action Names")]
    [SerializeField] private string selectCardActionName = "SelectCard";
    [SerializeField] private string throwAwayCardActionName = "ThrowAwayCard";
    [SerializeField] private string calculateCardActionName = "CalculateCard";
    [SerializeField] private string pointActionName = "Point"; 


    private InputAction selectCardAction;
    private InputAction throwAwayCardAction;
    private InputAction calculateCardAction;
    private InputAction pointAction;

    public bool SelectCardPressedThisFrame { get; private set; }
    public bool ThrowAwayCardPressedThisFrame { get; private set; }
    public bool CalculateCardPressedThisFrame { get; private set; }
    public Vector2 MousePosition { get; private set; }


    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        ResolveActions();

    }

    // update는 최대한 지양, 게임적인 매 프레임 반응속도를 원하는 구현에서만
    // 입력 관련 구현은 Update 지향
    private void Update()
    {
        SelectCardPressedThisFrame = selectCardAction != null && selectCardAction.WasPerformedThisFrame();
        ThrowAwayCardPressedThisFrame = throwAwayCardAction != null && throwAwayCardAction.WasPerformedThisFrame();
        CalculateCardPressedThisFrame = calculateCardAction != null && calculateCardAction.WasPerformedThisFrame();

        if (pointAction != null)
        {
            MousePosition = pointAction.ReadValue<Vector2>();
        }
    }

    private void ResolveActions()
    {
        if (playerInput == null || playerInput.actions == null)
        {
            Debug.LogError("[PlayerInputReader] PlayerInput 또는 Actions Asset이 비어있습니다.");
            return;
        }

        selectCardAction = FindAction(selectCardActionName);
        throwAwayCardAction = FindAction(throwAwayCardActionName);
        calculateCardAction = FindAction(calculateCardActionName);
        pointAction = FindAction(pointActionName);
    }

    private InputAction FindAction(string actionName)
    {
        if (string.IsNullOrEmpty(actionName))
        {
            return null;
        }

        InputAction action = playerInput.actions.FindAction(actionName, false);
        if (action == null)
        {
            Debug.Log($"[PlayerInputReader] Action을 찾지 못했습니다: {actionName}");
        }
        return action;
    }
}
