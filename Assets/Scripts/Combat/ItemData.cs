using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    public SkillData skillAttached;

    public void DestorySelf()
    {
        //InventoryManager.Instance.RemoveItem(this);
    }
}
