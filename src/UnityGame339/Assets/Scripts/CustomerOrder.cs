using UnityEngine;

[CreateAssetMenu(fileName = "NewOrder", menuName = "Cursed Coffee/Customer Order")]
public class CustomerOrder : ScriptableObject
{
    public string orderText;
    public Sprite drinkIcon;
}
