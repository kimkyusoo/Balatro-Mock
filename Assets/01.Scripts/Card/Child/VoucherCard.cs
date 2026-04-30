using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class VoucherCard : BaseCard, IPointerEnterHandler, IPointerExitHandler
{
    [Header("VoucherCard Information")]
    public int sellPrice;
    public VoucherEffect voucherEffect;

    [Header("UI")]
    public Image voucherImage;
    public RectTransform visualRoot;
    public GameObject buyButtonUI;
    public TextMeshProUGUI priceText;
    public GameObject ShopUIGroup;

    [SerializeField] private AudioClip effectSound;

    public string[] voucherDescriptions =
    {
        "",
        "+1 핸드 추가",
        "+1 버리기 추가",
        "코인 15개 획득",
        "+1 핸드 추가, -1 버리기 감소",
        "+1 버리기 추가, -1 핸드 감소"
    };

    private void Awake()
    {
        if (buyButtonUI != null)
        {
            Button buyButton = buyButtonUI.GetComponent<Button>();
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(() =>
                {
                    ShopManager manager = FindFirstObjectByType<ShopManager>();
                    if (manager != null) manager.BuyVoucherProduct();
                });
            }
        }
    }

    public void SetupCard(string id, int sellPrice, int voucherEffectIndex)
    {

        Initalize(id, sellPrice, voucherEffectIndex);
        gameObject.SetActive(false);
    }

    public void Initalize(string id, int sellPrice, int voucherEffectIndex)
    {
        if (voucherEffectIndex == 0) return;

        base.Initalize(id, "VoucherCard", voucherDescriptions[voucherEffectIndex]);

        this.sellPrice = sellPrice;
        this.voucherEffect = (VoucherEffect)voucherEffectIndex;
    }

    public void SetSprite(Sprite sprite)
    {
        voucherImage.sprite = sprite;
    }

    public void OnClickCard()
    {
        ShopManager shopManager = UnityEngine.Object.FindFirstObjectByType<ShopManager>();
        if (shopManager != null)
        {
            shopManager.SelectVoucherProduct(this);
        }

    }

    public void VisualSelect(bool isSelected)
    {
        visualRoot.anchoredPosition += isSelected ? new Vector2(0, 30) : new Vector2(0, -30);
        if (isSelected)
        {
            if (priceText != null) priceText.text = $"${sellPrice}";
        }
    }
    public void SetButtonActive(bool active)
    {
        if (ShopUIGroup != null)
        {
            ShopUIGroup.SetActive(active);
        }
    }

    public void UseVoucheerEffect(VoucherEffect voucherEffect)
    {
        if(RoundManager.Instance != null) RoundManager.Instance?.AddVoucherEffect(voucherEffect);
    }

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
