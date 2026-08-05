using UnityEngine;

public class MainScene : MonoBehaviour
{
    public void Gacha()
    {
        Debug.Log("Gacha!");
        var item = RarityCalculator.Instance.Gacha(out var rarity);
        Debug.Log($"{item.Egg_Name} {rarity:F5}");
        StoreInventory(item);
    }

    private void StoreInventory(Item item)
    {
        //자동판매 혹은 제거 조건에 따라서 제외하기.. 자동판매는 따로 슬롯이 있으면 될듯
        InventoryManager.Instance.AddItem(
            GameManager.Instance.UserInfo.id,
            item.ID,
            (success) =>
            {
                if (success)
                    Debug.Log($"인벤토리 저장 완료: {item.Egg_Name}");
            });
    }

    public void Quit()
    {
        StartCoroutine(GameManager.Instance.SaveAndQuit());
    }
}